#include "common.h"
#include <proxygen/httpserver/RequestHandlerFactory.h>
#include <proxygen/httpserver/ResponseBuilder.h>

using namespace proxygen;
using namespace folly;




class RequestHandlerWrapper : public RequestHandler
{
    std::unique_ptr<folly::IOBuf> _body;
    std::unique_ptr<HTTPMessage> _headers;
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
        _handler->OnRequest(new ReqContext(std::move(_body), std::move(_headers)), new ResponseBuilder(downstream_));
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



class ResponseWrapper : public ComObject<IResponseWrapper, &IID_IResponseWrapper>
{

public:
    virtual HRESULT SetCode(proxygen::ResponseBuilder*builder, int code, char* status)
    {
        builder->status(code, "OK");
        return S_OK;
    }
    virtual HRESULT AppendHeader(proxygen::ResponseBuilder*builder, char* key, char* value)
    {
        builder->header(key, value);
        return S_OK;
    }

    virtual HRESULT AppendBody(proxygen::ResponseBuilder*builder, void* data, int size, bool flush)
    {
        builder->body(IOBuf::copyBuffer((char*)data, size));
        if(flush)
            builder->send();
        return S_OK;
    }

    virtual HRESULT Complete(proxygen::ResponseBuilder*builder)
    {
        builder->sendWithEOM();
        delete builder;
        return S_OK;
    }
};



extern RequestHandler* CreateHandler(IRequestHandler*handler)
{
    return new RequestHandlerWrapper(handler);
}

extern IResponseWrapper* CreateResponseWrapper()
{
    return new ResponseWrapper();
}

