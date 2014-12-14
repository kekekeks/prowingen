#include "common.h"

namespace proxygen
{
    typedef void (*HttpThreadProcProto)(folly::EventBase* evBase);
    extern HttpThreadProcProto HttpThreadProcPtr;
}
using namespace proxygen;


HRESULT ProwingenFactory::SetProxygenThreadInit(void*newProc, void**oldProc)
{
    *oldProc = (void*)HttpThreadProcPtr;
    HttpThreadProcPtr = (HttpThreadProcProto)newProc;
    return S_OK;
}

extern "C"
{
    int CreateFactory(IUnknown**pUnk)
    {
        *pUnk = new ProwingenFactory();
        return 0;
    }
}
