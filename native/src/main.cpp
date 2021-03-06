#include "common.h"
#include <unistd.h>
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

folly::ThreadLocalPtr<EventBaseHolder> ThreadEventBase;
HRESULT ProwingenFactory::CallProxygenThreadInit(void*arg)
{
    auto ev =(folly::EventBase*)arg;
    auto holder = ThreadEventBase.get();
    if(holder == nullptr)
        ThreadEventBase.reset(holder = new EventBaseHolder());
    holder->EventBase = ev;
    OriginalThreadProcPtr(ev);
    holder->EventBase = nullptr;
    return S_OK;
}



static void exit_handler (int code, void*)
{
    _exit(code);
}

extern "C"
{
#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"
    __attribute__ ((visibility ("default"))) int CreateFactory(IUnknown**pUnk)
    {
        InitStatusCodes();
        on_exit(&exit_handler, NULL);
        *pUnk = new ProwingenFactory();
        return 0;
    }
#pragma clang diagnostic pop
}
