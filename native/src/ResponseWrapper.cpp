#include "common.h"

using namespace proxygen;
using namespace folly;


folly::EventBase*GetCurrentEventBase()
{
    auto holder = ThreadEventBase.get();
    if(holder== nullptr)
        return nullptr;
    return  holder->EventBase;
}

RespContext::RespContext(proxygen::ResponseHandler*responseHandler)
{
    _info.StatusCode = 200;
    _beforeHeadersSucceded = false;
    response = make_unique<ResponseBuilder>(responseHandler);
    eventBase = GetCurrentEventBase();
    DCHECK(eventBase!=NULL);
}


static void ExecOnEventBase(folly::EventBase*base, folly::Cob cb)
{
    auto currentEventBase = GetCurrentEventBase();
    if(currentEventBase != base)
        base->runInEventBaseThread(cb);
    else
        cb();

}

inline void BeforeHeaders(RespContext *context) {
    if (context->_beforeHeadersSucceded)
        return;
    context->response->status(context->_info.StatusCode, HttpStatusCodes[context->_info.StatusCode]);

};

extern void ApiAppendHeader(RespContext*context, char* key, char* value) {
    BeforeHeaders(context);
    context->response->header(key, value);

};

extern void ApiAppendBody(RespContext*context, void* data, int size, bool flush) {
    IOBuf *pBuffer = 0;
    if (data != NULL && size != 0)
        pBuffer = IOBuf::copyBuffer((char *) data, (size_t) size).release();
    ExecOnEventBase(context->eventBase, [=]() {
        BeforeHeaders(context);
        if (pBuffer != NULL) {
            auto buffer = unique_ptr<IOBuf>(pBuffer);
            context->response->body(std::move(buffer));
        }
        if (flush)
            context->response->send();
    });
};

extern void ApiCompleteResponse(RespContext*context, void* data, int size) {
    IOBuf *pBuffer = 0;
    if (data != NULL && size != 0)
        pBuffer = IOBuf::copyBuffer((char *) data, (size_t) size).release();
    ExecOnEventBase(context->eventBase, [=]() {
        BeforeHeaders(context);
        if (pBuffer != NULL) {
            auto buffer = unique_ptr<IOBuf>(pBuffer);
            context->response->body(std::move(buffer));
        }
        context->response->sendWithEOM();
        delete context;
    });
};