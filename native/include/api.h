#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"
#ifndef API_H_INCLUDED
#define API_H_INCLUDED
#include "com.h"
#include <proxygen/httpserver/ResponseBuilder.h>

class ReqContext;
class RespContext;



struct IOBufInfo
{
    const void*data;
    uint64_t size;
};

struct HttpHeader
{
    const char* Key;
    const char* Value;
};

struct RequestInfo
{
    const char* Url;
    uint64_t bufferCount;
    IOBufInfo*buffers;
    const char* Method;
    uint64_t HttpVersion;
    int IsSecure;
    uint64_t HeaderCount;
    HttpHeader*Headers;
};

struct ResponseInfo
{
    ushort StatusCode;
};

typedef void (*ProwingenRequestHandler)(void*,void*);



struct IHttpServer : public IUnknown
{
    virtual HRESULT AddAddress(char*host, uint16_t port, bool lookup) = 0;
    virtual HRESULT Start(char* exceptionBuffer) = 0;
    virtual HRESULT Stop() = 0;

};



struct IProwingenFactory : public IUnknown
{
    virtual HRESULT CreateServer(ProwingenRequestHandler requestHandler, IHttpServer**ppServer) = 0;
    virtual HRESULT SetProxygenThreadInit(void*newProc) = 0;
    virtual HRESULT CallProxygenThreadInit(void*arg) = 0;
    virtual HRESULT GetMethodTablePtr(void***pTable) = 0;
};



extern void ApiDisposeRequest(ReqContext*ctx);
extern void ApiAppendHeader(RespContext*builder, char* key, char* value);
extern void ApiAppendBody(RespContext*builder, void* data, int size, bool flush);
extern void ApiCompleteResponse(RespContext*builder, void* data, int size);


extern const GUID IID_IHttpServer;
extern const GUID IID_IProwingenFactory;

#endif // API_H_INCLUDED

#pragma clang diagnostic pop