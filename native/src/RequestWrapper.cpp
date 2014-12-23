#include <dejagnu.h>
#include "common.h"

ReqContext::ReqContext(std::unique_ptr<folly::IOBuf> body, std::unique_ptr<proxygen::HTTPMessage> headers)
{
    _body = std::move(body);
    _info.Url = headers->getURL().c_str();

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
