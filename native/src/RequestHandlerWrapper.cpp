#include "common.h"
#include <proxygen/httpserver/RequestHandlerFactory.h>
#include <proxygen/httpserver/ResponseBuilder.h>

using namespace proxygen;
using namespace folly;


class RequestWrapper : public ComObject<IRequest, &IID_IRequest>
{
    std::unique_ptr<folly::IOBuf> _body;

public:
    RequestWrapper(std::unique_ptr<folly::IOBuf> body)
    {
        _body = std::move(body);
    }
};

class ResponseWrapper : public ComObject<IResponse, &IID_IResponse>
{
    ResponseHandler* _downstream;
    std::unique_ptr<ResponseBuilder> _builder;

public:
    ResponseWrapper(ResponseHandler* downstream)
    {
        _downstream = downstream;
        _builder = unique_ptr<ResponseBuilder>(new ResponseBuilder(downstream));

    }

    virtual HRESULT SetCode(int code, char* status)
    {
        _builder->status(code, "OK");
        return S_OK;
    }
    virtual HRESULT AppendHeader(char* key, char* value)
    {
        _builder->header(key, value);
        return S_OK;
    }

    virtual HRESULT AppendBody(void* data, int size, bool flush)
    {
        _builder->body(IOBuf::copyBuffer((char*)data, size));
        if(flush)
            _builder->send();
        return S_OK;
    }

    virtual HRESULT Complete()
    {
        _builder->sendWithEOM();
        return S_OK;
    }
};




class RequestHandlerWrapper : public RequestHandler
{
    std::unique_ptr<folly::IOBuf> _body;
    IRequestHandler*_handler;

public:
    RequestHandlerWrapper(IRequestHandler*handler)
    {
        _handler = handler;
        if(_handler)
            _handler->AddRef();

    }
    ~RequestHandlerWrapper()
    {
        if(_handler)
            _handler->Release();

    }

    void onRequest(std::unique_ptr<HTTPMessage> headers) noexcept override
    {
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
        auto requestWrapper = ComPtr<IRequest>(new RequestWrapper(std::move(_body)));
        auto responseWrapper = ComPtr<IResponse> (new ResponseWrapper(downstream_));
        _handler->OnRequest(requestWrapper, responseWrapper);
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


extern RequestHandler* CreateHandler(IRequestHandler*handler)
{
    return new RequestHandlerWrapper(handler);
}
