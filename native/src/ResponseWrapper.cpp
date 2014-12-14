#include "common.h"
#include <proxygen/httpserver/ResponseBuilder.h>

using namespace proxygen;
using namespace folly;


RespContext::RespContext(proxygen::ResponseHandler*responseHandler)
{
    response = make_unique<ResponseBuilder>(responseHandler);
}


class ResponseWrapper : public ComObject<IResponseWrapper, &IID_IResponseWrapper>
{

public:
    virtual HRESULT SetCode(RespContext*context, int code, char* status)
    {
        context->response->status(code, "OK");
        return S_OK;
    }
    virtual HRESULT AppendHeader(RespContext*context, char* key, char* value)
    {
        context->response->header(key, value);
        return S_OK;
    }

    virtual HRESULT AppendBody(RespContext*context, void* data, int size, bool flush)
    {
        context->response->body(IOBuf::copyBuffer((char*)data, size));
        if(flush)
            context->response->send();
        return S_OK;
    }

    virtual HRESULT Complete(RespContext*context)
    {
        context->response->sendWithEOM();
        delete context;
        return S_OK;
    }
};


HRESULT ProwingenFactory::CreateResponseWrapper(IResponseWrapper**ppv)
{
    *ppv=new ResponseWrapper();
    return S_OK;
}

