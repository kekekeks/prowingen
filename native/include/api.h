#ifndef API_H_INCLUDED
#define API_H_INCLUDED
#include "com.h"

struct IRequestHandler : public IUnknown
{


};

struct IRequestHandlerFactory : public IUnknown
{
    virtual HRESULT CreateHandler(IRequestHandler**pHandler) = 0;
};


struct IHttpServer : public IUnknown
{
    virtual HRESULT AddAddress(char*host, int port, bool lookup) = 0;
    virtual HRESULT Start() = 0;

};



struct IProwingenFactory : public IUnknown
{
    virtual HRESULT CreateServer(IRequestHandlerFactory*factory, IHttpServer**ppServer) = 0;
};



extern const GUID IID_IHttpServer;
extern const GUID IID_IProwingenFactory;
extern const GUID IID_IRequestHandlerFactory;
extern const GUID IID_IRequestHandler;

#ifdef INCLUDE_GUID
// [Guid("918cfa9b-a766-41e7-9c4b-330954d01b47")]
const GUID IID_IHttpServer = { 0x918cfa9b, 0xa766, 0x41e7, { 0x9c, 0x4b, 0x33, 0x9, 0x54, 0xd0, 0x1b, 0x47 } };

// [Guid("e4ea9822-30c8-4319-9d80-5a0e48de0be5")]
const GUID IID_IProwingenFactory = { 0xe4ea9822, 0x30c8, 0x4319, { 0x9d, 0x80, 0x5a, 0xe, 0x48, 0xde, 0xb, 0xe5 } };

// [Guid("c09bb425-a496-45c3-b6bc-a7f0060b6064")]
const GUID IID_IRequestHandlerFactory = { 0xc09bb425, 0xa496, 0x45c3, { 0xb6, 0xbc, 0xa7, 0xf0, 0x6, 0xb, 0x60, 0x64 } };

// [Guid("6307a020-22f4-462f-aee0-15e7bc555c4d")]
const GUID IID_IRequestHandler_ = { 0x6307a020, 0x22f4, 0x462f, { 0xae, 0xe0, 0x15, 0xe7, 0xbc, 0x55, 0x5c, 0x4d } };
#endif


#endif // API_H_INCLUDED
