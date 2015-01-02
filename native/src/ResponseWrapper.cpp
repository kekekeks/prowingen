#include "common.h"

using namespace proxygen;
using namespace folly;


RespContext::RespContext(proxygen::ResponseHandler*responseHandler)
{
    _info.StatusCode = 200;
    _beforeHeadersSucceded = false;
    response = make_unique<CustomResponseBuilder>(responseHandler);
    _rawResponse = responseHandler;
    _upgraded = false;
    eventBase = GetCurrentEventBase();
    DCHECK(eventBase!=NULL);
}

inline void BeforeHeaders(RespContext *context) {
    if (context->_beforeHeadersSucceded)
        return;
    context->response->status(context->_info.StatusCode, HttpStatusCodes[context->_info.StatusCode]);
    context->_beforeHeadersSucceded = true;

};

extern void ApiAppendHeader(RespContext*context, char* key, char* value) {
    BeforeHeaders(context);
    context->response->header(key, value);

};

extern void ApiUpgradeResponse(RespContext*context)
{
    ExecOnEventBase(context->eventBase, [=]() {
        BeforeHeaders(context);
        context->_upgraded = true;
        context->response->acceptUpgradeRequest(CustomResponseBuilder::UpgradeType::HTTP_UPGRADE);
    });
}

extern void ApiAppendBody(RespContext*context, void* data, int size, bool flush) {
    IOBuf *pBuffer = 0;
    if (data != NULL && size != 0)
        pBuffer = IOBuf::copyBuffer((char *) data, (size_t) size).release();
    else if(context->_upgraded)
        return;
    ExecOnEventBase(context->eventBase, [=]() {
        BeforeHeaders(context);
        if(context->_upgraded)
        {
            if(pBuffer!=NULL && size != 0)
            context->_rawResponse->sendBody(std::move(unique_ptr<IOBuf>(pBuffer)));
        }
        else {
            if (pBuffer != NULL) {
                auto buffer = unique_ptr<IOBuf>(pBuffer);
                context->response->body(std::move(buffer));
            }
            if (flush)
                context->response->send();
        }
    });
};

extern void ApiCompleteResponse(RespContext*context, void* data, int size) {
    IOBuf *pBuffer = 0;
    if (data != NULL && size != 0)
        pBuffer = IOBuf::copyBuffer((char *) data, (size_t) size).release();
    ExecOnEventBase(context->eventBase, [=]() {
        if(context->_upgraded)
        {
            context->_rawResponse->sendEOM();
        }
        else {
            BeforeHeaders(context);
            if (pBuffer != NULL) {
                auto buffer = unique_ptr<IOBuf>(pBuffer);
                context->response->body(std::move(buffer));
            }
            context->response->sendWithEOM();
        }
        delete context;
    });
};