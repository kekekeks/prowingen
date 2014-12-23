#include <dejagnu.h>
#include "common.h"

ReqContext::ReqContext(std::unique_ptr<folly::IOBuf> body, std::unique_ptr<proxygen::HTTPMessage> headers)
{
    _body = std::move(body);
    _headers = std::move(headers);
    _info.Url = _headers->getURL().c_str();

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
    auto ver = _headers->getHTTPVersion();
    _info.HttpVersion = (ver.first<<16) + ver.second;
    _info.Method = _headers->getMethodString().c_str();
    _info.IsSecure = _headers->isSecure();

}

class RequestWrapper : public ComObject<IRequestWrapper, &IID_IRequestWrapper>
{
public:
    virtual HRESULT Dispose(ReqContext*req)
    {
        delete req;
        return S_OK;
    }
};


HRESULT ProwingenFactory::CreateRequestWrapper(IRequestWrapper**ppv)
{
    *ppv=new RequestWrapper();
    return S_OK;
}
