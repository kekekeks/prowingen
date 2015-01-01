#include "HttpServerWrapper.h"

using namespace proxygen;
using namespace folly;
using Protocol = HTTPServer::Protocol;



class RequestHandlerFactoryWrapper : public RequestHandlerFactory
{
private:
    ProwingenRequestHandler _handler;
public:
    RequestHandlerFactoryWrapper(ProwingenRequestHandler handler)
    {
        _handler = handler;

    }
    void onServerStart() noexcept override {
    }
    void onServerStop() noexcept override {
    }
    RequestHandler* onRequest(RequestHandler*, HTTPMessage*) noexcept override {
        return CreateHandler(_handler);
    }
};


HttpServerWrapper::HttpServerWrapper(ProwingenRequestHandler handler)
{
    _server = NULL;
    _handler = handler;
}

HRESULT HttpServerWrapper::AddAddress(char*host, uint16_t port, bool lookup)
{
    std::string shost = std::string(host);
    _ips.push_back(HTTPServer::IPConfig(SocketAddress(shost, port, lookup), Protocol::HTTP));
    return S_OK;
}

HRESULT HttpServerWrapper::Start(char*exceptionBuffer)
{
    HTTPServerOptions options;
    options.idleTimeout = std::chrono::milliseconds(60000);
    options.threads = 8;
    options.handlerFactories = RequestHandlerChain()
        .addThen(std::unique_ptr<RequestHandlerFactory>(new RequestHandlerFactoryWrapper(_handler)))
        .build();
    _server = new HTTPServer(std::move(options));
    try {
        _server->bind(_ips);
        std::thread([=](){_server->start();}).detach();
        return S_OK;
    }
    catch (const std::exception& ex)
    {
        strcpy(exceptionBuffer, ex.what());
        return E_FAIL;
    }
    catch(...)
    {
        strcpy(exceptionBuffer, "Unknown failure");
        return  E_FAIL;
    }
}

HttpServerWrapper::~HttpServerWrapper()
{
    if(_server != NULL)
        delete _server;
}

HRESULT ProwingenFactory::CreateServer(ProwingenRequestHandler handler, IHttpServer**ppServer)
{
    *ppServer = new HttpServerWrapper(handler);
    return S_OK;
}

HRESULT HttpServerWrapper::Stop() {
    if(_server != NULL)
        _server->stop();
    return S_OK;
}
