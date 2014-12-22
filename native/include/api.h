#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"
#ifndef API_H_INCLUDED
#define API_H_INCLUDED
#include "com.h"
#include <proxygen/httpserver/ResponseBuilder.h>

class ReqContext;
class RespContext;

struct IResponseWrapper : public IUnknown
{
    virtual HRESULT SetCode(RespContext*builder, uint16_t code, char* status) = 0;
    virtual HRESULT AppendHeader(RespContext*builder, char* key, char* value) = 0;
    virtual HRESULT AppendBody(RespContext*builder, void* data, int size, bool flush) = 0;
    virtual HRESULT Complete(RespContext*builder) = 0;
};

struct RequestInfo
{
    const char* Url;
};

typedef void (*ProwingenRequestHandler)(void*,void*);

struct IRequestWrapper : public IUnknown
{
    virtual HRESULT Dispose(ReqContext*ctx) = 0;
};

struct IHttpServer : public IUnknown
{
    virtual HRESULT AddAddress(char*host, uint16_t port, bool lookup) = 0;
    virtual HRESULT Start(char* exceptionBuffer) = 0;

};



struct IProwingenFactory : public IUnknown
{
    virtual HRESULT CreateServer(ProwingenRequestHandler requestHandler, IHttpServer**ppServer) = 0;
    virtual HRESULT CreateResponseWrapper(IResponseWrapper**ppv) = 0;
    virtual HRESULT CreateRequestWrapper(IRequestWrapper**ppv) = 0;
    virtual HRESULT SetProxygenThreadInit(void*newProc) = 0;
    virtual HRESULT CallProxygenThreadInit(void*arg) = 0;
};



extern const GUID IID_IHttpServer;
extern const GUID IID_IProwingenFactory;
extern const GUID IID_IRequestWrapper;
extern const GUID IID_IResponseWrapper;

#endif // API_H_INCLUDED

#pragma clang diagnostic pop