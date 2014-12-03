#include "api.h"
#include "HttpServerWrapper.h"

class ProwingenFactory : public ComObject<IProwingenFactory, &IID_IProwingenFactory>
{
    virtual HRESULT CreateServer(IRequestHandlerFactory*factory, IHttpServer**ppServer)
    {
        *ppServer = new HttpServerWrapper(factory);
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
