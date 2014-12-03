#include "HttpServerWrapper.h"
#include <proxygen/httpserver/RequestHandler.h>
#include <proxygen/httpserver/ResponseBuilder.h>

using namespace proxygen;
using namespace folly;
using Protocol = HTTPServer::Protocol;

class RequestHandlerWrapper : public RequestHandler
{
    std::unique_ptr<folly::IOBuf> _body;
    IRequestHandler*_handler;

public:
    RequestHandlerWrapper(IRequestHandler*handler)
    {
        _handler = handler;
        _handler->AddRef();

    }
    ~RequestHandlerWrapper()
    {
        _handler->Release();

    }

    void onRequest(std::unique_ptr<HTTPMessage> headers) noexcept override {

    }

    void onBody(std::unique_ptr<folly::IOBuf> body) noexcept override {
        if (_body) {
            _body->prependChain(std::move(body));
        } else {
            _body = std::move(body);
        }
    }

    void onEOM() noexcept {
        ResponseBuilder(downstream_)
            .status(200, "OK")
            .header("Request-Number", "123")
            .body(std::move(_body))
            .sendWithEOM();
    }

    void onUpgrade(UpgradeProtocol protocol) noexcept
    {
    }
    void requestComplete() noexcept {
        delete this;
    }
    void onError(ProxygenError err) noexcept {
        delete this;
    }


};


class RequestHandlerFactoryWrapper : public RequestHandlerFactory
{
private:
    IRequestHandlerFactory* _factory;
public:
    RequestHandlerFactoryWrapper(IRequestHandlerFactory* factory)
    {
        _factory = factory;
        _factory->AddRef();
    }
    ~RequestHandlerFactoryWrapper()
    {
        _factory->Release();

    }

    void onServerStart() noexcept override {
    }
    void onServerStop() noexcept override {
    }
    RequestHandler* onRequest(RequestHandler*, HTTPMessage*) noexcept override {
        IRequestHandler* handler;
        if(0 == _factory->CreateHandler(&handler))
        {
            RequestHandler* wrapper = new RequestHandlerWrapper(handler);
            handler->Release();
            return wrapper;
        }
        return 0;
    }
};


HttpServerWrapper::HttpServerWrapper(IRequestHandlerFactory*factory)
{
    HTTPServerOptions options;
    options.idleTimeout = std::chrono::milliseconds(60000);
    options.handlerFactories = RequestHandlerChain()
        .addThen(std::unique_ptr<RequestHandlerFactory>(new RequestHandlerFactoryWrapper(factory)))
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
