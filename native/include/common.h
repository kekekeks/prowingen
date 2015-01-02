#ifndef COMMON_H_INCLUDED
#define COMMON_H_INCLUDED
#include "api.h"
#include "util.h"
#include "CustomResponseBuilder.h"
#include "OpaqueInputStream.h"
extern std::string HttpStatusCodes[];
extern void InitStatusCodes();

class ReqContext
{
    public:
        RequestInfo _info;
        std::unique_ptr<folly::IOBuf> _body;
        std::unique_ptr<proxygen::HTTPMessage> _message;
        std::vector<IOBufInfo> _buffers;
        std::vector<HttpHeader> _headers;

        //private for native implementation
        std::shared_ptr<OpaqueInputStreamWrapper> _opaqueStream;
        ReqContext(std::unique_ptr<folly::IOBuf> body, std::unique_ptr<proxygen::HTTPMessage> message, bool upgradable, std::shared_ptr<OpaqueInputStreamWrapper> opaqueStream);
};

class RespContext
{
public:
    ResponseInfo _info;
    bool _beforeHeadersSucceded;
    std::unique_ptr<CustomResponseBuilder> response;
    proxygen::ResponseHandler*_rawResponse;
    bool _upgraded;
    folly::EventBase*eventBase;
    RespContext(proxygen::ResponseHandler*responseHandler);
};

class ProwingenFactory : public ComObject<IProwingenFactory, &IID_IProwingenFactory>
{
public:
    virtual HRESULT CreateServer(ProwingenRequestHandler requestHandler, IHttpServer**ppServer);
    virtual HRESULT SetProxygenThreadInit(void*newProc);
    virtual HRESULT CallProxygenThreadInit (void*arg);
    virtual HRESULT GetMethodTablePtr(void***pTable);
};

extern proxygen::RequestHandler* CreateHandler(ProwingenRequestHandler requestHandler);




#endif // COMMON_H_INCLUDED
