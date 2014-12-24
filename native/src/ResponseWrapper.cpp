#include "common.h"

using namespace proxygen;
using namespace folly;


RespContext::RespContext(proxygen::ResponseHandler*responseHandler)
{
    _info.StatusCode = 200;
    _headersSent = false;
    response = make_unique<ResponseBuilder>(responseHandler);
    eventBase = folly::EventBaseManager::get()->getExistingEventBase();
}


static void ExecOnEventBase(folly::EventBase*base, folly::Cob cb)
{
    auto currentEventBase = folly::EventBaseManager::get()->getExistingEventBase();
    if(currentEventBase != base)
        base->runInEventBaseThread(cb);
    else
        cb();

}

class ResponseWrapper : public ComObject<IResponseWrapper, &IID_IResponseWrapper>
{

public:
    virtual HRESULT AppendHeader(RespContext*context, char* key, char* value)
    {
        context->response->header(key, value);
        return S_OK;
    }

    inline void OnWrite(RespContext*context)
    {
        if(context->_headersSent)
            return;
        context->response->status(context->_info.StatusCode, HttpStatusCodes[context->_info.StatusCode]);

    }

    virtual HRESULT AppendBody(RespContext*context, void* data, int size, bool flush)
    {
        OnWrite(context);
        IOBuf* pBuffer = 0;
        if(data != NULL && size != 0)
            pBuffer = IOBuf::copyBuffer((char*)data, (size_t)size).release();
        ExecOnEventBase(context->eventBase, [=] ()
        {
            if(pBuffer != NULL) {
                auto buffer = unique_ptr<IOBuf>(pBuffer);
                context->response->body(std::move(buffer));
            }
            if(flush)
                context->response->send();
        });
        return S_OK;
    }

    virtual HRESULT Complete(RespContext*context, void* data, int size)
    {
        OnWrite(context);
        IOBuf* pBuffer = 0;
        if(data != NULL && size != 0)
            pBuffer = IOBuf::copyBuffer((char*)data, (size_t)size).release();
        ExecOnEventBase(context->eventBase, [=] ()
        {
            if(pBuffer != NULL) {
                auto buffer = unique_ptr<IOBuf>(pBuffer);
                context->response->body(std::move(buffer));
            }
            context->response->sendWithEOM();
            delete context;
        });
        return S_OK;
    }
};


HRESULT ProwingenFactory::CreateResponseWrapper(IResponseWrapper**ppv)
{
    *ppv=new ResponseWrapper();
    return S_OK;
}

