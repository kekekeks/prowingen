#include "common.h"
#include <proxygen/lib/http/HTTPMessage.h>

ReqContext::ReqContext(std::unique_ptr<folly::IOBuf> body, std::unique_ptr<proxygen::HTTPMessage> headers)
{
    _body = std::move(body);
    _info.Url = headers->getURL().c_str();
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
