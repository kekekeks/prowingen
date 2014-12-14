#ifndef API_H_INCLUDED
#define API_H_INCLUDED
#include "com.h"
#include <proxygen/httpserver/ResponseBuilder.h>

class ReqContext;
class RespContext;

struct IResponseWrapper : public IUnknown
{
    virtual HRESULT SetCode(RespContext*builder, int code, char* status) = 0;
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
    virtual HRESULT AddAddress(char*host, int port, bool lookup) = 0;
    virtual HRESULT Start() = 0;

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
extern const GUID IID_IRequestHandler;
extern const GUID IID_IRequestWrapper;
extern const GUID IID_IResponseWrapper;

#ifdef INCLUDE_GUID
// [Guid("918cfa9b-a766-41e7-9c4b-330954d01b47")]
const GUID IID_IHttpServer = { 0x918cfa9b, 0xa766, 0x41e7, { 0x9c, 0x4b, 0x33, 0x9, 0x54, 0xd0, 0x1b, 0x47 } };

// [Guid("e4ea9822-30c8-4319-9d80-5a0e48de0be5")]
const GUID IID_IProwingenFactory = { 0xe4ea9822, 0x30c8, 0x4319, { 0x9d, 0x80, 0x5a, 0xe, 0x48, 0xde, 0xb, 0xe5 } };

// [Guid("ab57d7ab-8825-4d9c-9c7b-d03c2f2884ee")]
const GUID IID_IRequestWrapper = { 0xab57d7ab, 0x8825, 0x4d9c, { 0x9c, 0x7b, 0xd0, 0x3c, 0x2f, 0x28, 0x84, 0xee } };

// [Guid("24e1a430-27f6-4fd4-a0a4-95e9dea8d51a")]
const GUID IID_IResponseWrapper = { 0x24e1a430, 0x27f6, 0x4fd4, { 0xa0, 0xa4, 0x95, 0xe9, 0xde, 0xa8, 0xd5, 0x1a } };
#endif


#endif // API_H_INCLUDED
