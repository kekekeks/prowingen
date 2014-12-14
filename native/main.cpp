#include "api.h"
#include "HttpServerWrapper.h"

namespace proxygen
{
    typedef void (*HttpThreadProcProto)(folly::EventBase* evBase);
    extern HttpThreadProcProto HttpThreadProcPtr;
}
using namespace proxygen;

class ProwingenFactory : public ComObject<IProwingenFactory, &IID_IProwingenFactory>
{
    virtual HRESULT CreateServer(IRequestHandler*handler, IHttpServer**ppServer)
    {
        *ppServer = new HttpServerWrapper(handler);
        return S_OK;
    }

    virtual HRESULT CreateResponseWrapper(IResponseWrapper**ppv)
    {
        *ppv=::CreateResponseWrapper();
        return S_OK;
    }


    virtual HRESULT CreateRequestWrapper(IRequestWrapper**ppv)
    {
        *ppv=::CreateRequestWrapper();
        return S_OK;
    }

    virtual HRESULT SetProxygenThreadInit(void*newProc, void**oldProc)
    {
        *oldProc = (void*)HttpThreadProcPtr;
        HttpThreadProcPtr = (HttpThreadProcProto)newProc;
        return S_OK;
    }
};



extern "C"
{
    int CreateFactory(IUnknown**pUnk)
    {
        *pUnk = new ProwingenFactory();
        return 0;
    }
}
