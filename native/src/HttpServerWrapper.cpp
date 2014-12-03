#include "HttpServerWrapper.h"
#include <proxygen/httpserver/RequestHandler.h>
#include <proxygen/httpserver/ResponseBuilder.h>

using namespace proxygen;
using namespace folly;
using Protocol = HTTPServer::Protocol;



class RequestHandlerFactoryWrapper : public RequestHandlerFactory
{
private:
    IRequestHandler* _handler;
public:
    RequestHandlerFactoryWrapper(IRequestHandler* handler)
    {
        _handler = handler;

    }
    ~RequestHandlerFactoryWrapper()
    {
        _handler->Release();

    }

    void onServerStart() noexcept override {
    }
    void onServerStop() noexcept override {
    }
    RequestHandler* onRequest(RequestHandler*, HTTPMessage*) noexcept override {
        return CreateHandler(_handler);
        return 0;
    }
};


HttpServerWrapper::HttpServerWrapper(IRequestHandler*handler)
{
    HTTPServerOptions options;
    options.idleTimeout = std::chrono::milliseconds(60000);
    options.threads = 8;
    options.handlerFactories = RequestHandlerChain()
        .addThen(std::unique_ptr<RequestHandlerFactory>(new RequestHandlerFactoryWrapper(handler)))
        .build();
    _server = new HTTPServer(std::move(options));
}

HRESULT HttpServerWrapper::AddAddress(char*host, int port, bool lookup)
{
    std::string shost = std::string(host);
    _ips.push_back(HTTPServer::IPConfig(SocketAddress(shost, port, lookup), Protocol::HTTP));
    return S_OK;
}

HRESULT HttpServerWrapper::Start()
{
    _server->bind(_ips);
    _server->start();
    return S_OK;
}

HttpServerWrapper::~HttpServerWrapper()
{
    delete _server;
}
