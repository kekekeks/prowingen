#include "common.h"

using namespace proxygen;
using namespace folly;




class RequestHandlerWrapper : public RequestHandler
{
    std::unique_ptr<folly::IOBuf> _body;
    std::unique_ptr<HTTPMessage> _headers;
    ProwingenRequestHandler _handler;

public:
    RequestHandlerWrapper(ProwingenRequestHandler handler)
    {
        _handler = handler;
    }

    void onRequest(std::unique_ptr<HTTPMessage> headers) noexcept override
    {
        _headers = std::move(headers);
    }

    void onBody(std::unique_ptr<folly::IOBuf> body) noexcept override
    {
        if (_body) {
            _body->prependChain(std::move(body));
        } else {
            _body = std::move(body);
        }
    }

    void onEOM() noexcept
    {
        _handler(new ReqContext(std::move(_body), std::move(_headers)), new RespContext(downstream_));
    }

    void onUpgrade(UpgradeProtocol protocol) noexcept
    {
    }

    void requestComplete() noexcept
    {
        delete this;
    }
    void onError(ProxygenError err) noexcept
    {
        delete this;
    }


};

extern RequestHandler* CreateHandler(ProwingenRequestHandler handler)
{
    return new RequestHandlerWrapper(handler);
}


