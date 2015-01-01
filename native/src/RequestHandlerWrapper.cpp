#include "common.h"

using namespace proxygen;
using namespace folly;




class RequestHandlerWrapper : public RequestHandler
{
    std::unique_ptr<folly::IOBuf> _body;
    std::unique_ptr<HTTPMessage> _headers;
    ProwingenRequestHandler _handler;
    bool _upgrade;

public:
    RequestHandlerWrapper(ProwingenRequestHandler handler)
    {
        _handler = handler;
        _upgrade = false;
    }

    void onRequest(std::unique_ptr<HTTPMessage> headers) noexcept override
    {
        _headers = std::move(headers);
        if(_headers->getMethod() == HTTPMethod::GET)
        {
            auto upgrade = _headers->getHeaders().getNumberOfValues(HTTPHeaderCode::HTTP_HEADER_UPGRADE) != 0;
            _upgrade = upgrade;

            if(_upgrade)
                _handler(new ReqContext(std::move(_body), std::move(_headers), upgrade), new RespContext(downstream_));
        }
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
        if(!_upgrade) {
            _handler(new ReqContext(std::move(_body), std::move(_headers), false), new RespContext(downstream_));
        }
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


