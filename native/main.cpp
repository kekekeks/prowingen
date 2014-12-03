#include "api.h"
#include "HttpServerWrapper.h"


#ifdef PATCHED_PROXYGEN
namespace proxygen
{


typedef void (*t_mono_thread_callback)(void*);
extern void ProxygenSetMonoThreadInitCallback(t_mono_thread_callback cb);
extern void ProxygenContinueThreadInit(void* arg);
}

using namespace proxygen;

#endif


class ProwingenFactory : public ComObject<IProwingenFactory, &IID_IProwingenFactory>
{
    virtual HRESULT CreateServer(IRequestHandler*handler, IHttpServer**ppServer)
    {
        *ppServer = new HttpServerWrapper(handler);
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

#ifdef PATCHED_PROXYGEN
    void SetThreadInitCallback(t_mono_thread_callback cb)
    {
        ProxygenSetMonoThreadInitCallback(cb);
    }
    void ContinueThreadInit (void*arg)
    {
        ProxygenContinueThreadInit(arg);
    }
#endif
}
