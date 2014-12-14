#include "common.h"

using namespace proxygen;
using namespace folly;


RespContext::RespContext(proxygen::ResponseHandler*responseHandler)
{
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
    virtual HRESULT SetCode(RespContext*context, uint16_t code, char* status)
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
        auto pBuffer = IOBuf::copyBuffer((char*)data, (size_t)size).release();
        ExecOnEventBase(context->eventBase, [=] ()
        {
            auto buffer = unique_ptr<IOBuf>(pBuffer);
            context->response->body(std::move(buffer));
            if(flush)
                context->response->send();
        });
        return S_OK;
    }

    virtual HRESULT Complete(RespContext*context)
    {
        ExecOnEventBase(context->eventBase, [=] ()
        {
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

