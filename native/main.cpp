#include "common.h"

namespace proxygen
{
    typedef void (*HttpThreadProcProto)(folly::EventBase* evBase);
    extern HttpThreadProcProto HttpThreadProcPtr;
}
using namespace proxygen;

static HttpThreadProcProto OriginalThreadProcPtr = 0;
HRESULT ProwingenFactory::SetProxygenThreadInit(void*newProc)
{
    if(OriginalThreadProcPtr == 0)
        OriginalThreadProcPtr = HttpThreadProcPtr;
    HttpThreadProcPtr = (HttpThreadProcProto)newProc;
    return S_OK;
}

HRESULT ProwingenFactory::CallProxygenThreadInit(void*arg)
{
    OriginalThreadProcPtr((folly::EventBase*)arg);
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
