#include "common.h"

#define E(x) (void*)&x
void* MethodTable[] =
        {
            E(ApiDisposeRequest),E(ApiAppendHeader),E(ApiAppendBody),E(ApiCompleteResponse),E(ApiUpgradeResponse),E(ApiUpgradeToOpaqueInputStream),
                E(ApiDisposeOpaqueInputStream)
        };


HRESULT ProwingenFactory::GetMethodTablePtr(void***ret)
{
    *ret = MethodTable;
    return S_OK;
}