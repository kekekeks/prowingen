#include <dejagnu.h>
#include "common.h"

ReqContext::ReqContext(std::unique_ptr<folly::IOBuf> body, std::unique_ptr<proxygen::HTTPMessage> message, bool upgradable, std::shared_ptr<OpaqueInputStreamWrapper> opaque)
{
    _body = std::move(body);
    _message = std::move(message);
    _opaqueStream = opaque;
    _info.Url = _message->getURL().c_str();

    folly::IOBuf* pBody = _body.get();
    auto buffer = pBody;
    while (pBody != NULL)
    {
        IOBufInfo info;
        info.size = buffer->length();
        info.data = (const void*)buffer->data();
        _buffers.push_back(info);
        if(pBody == buffer->next())
            break; //Last one

        buffer = buffer->next();
    }
    _info.bufferCount=_buffers.size();
    _info.buffers = _buffers.data();
    auto ver = _message->getHTTPVersion();
    _info.HttpVersion = (ver.first<<16) + ver.second;
    _info.Method = _message->getMethodString().c_str();
    _info.IsSecure = _message->isSecure();

    auto headers = _message->getHeaders();
    HttpHeader header;
    headers.forEach([&](const std::string& key, const std::string &value){
        header.Key = key.c_str();
        header.Value = value.c_str();
        _headers.push_back(header);
    });
    _info.HeaderCount = _headers.size();
    _info.Headers = _headers.data();
    _info.IsUpgradable = upgradable;
}

extern void ApiDisposeRequest(ReqContext*req) {
    delete req;
}

extern void* ApiUpgradeToOpaqueInputStream(ReqContext* ctx, ProwingenOpaqueInputStreamHandler handler)
{
    return InitializeOpaqueInputStream(ctx->_opaqueStream, handler);
}