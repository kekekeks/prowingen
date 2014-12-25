#ifndef COMMON_H_INCLUDED
#define COMMON_H_INCLUDED
#include "api.h"
#include <folly/Memory.h>
#include <folly/Portability.h>
#include <folly/io/async/EventBaseManager.h>
#include <proxygen/httpserver/HTTPServer.h>
#include <proxygen/httpserver/RequestHandlerFactory.h>

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
        ReqContext(std::unique_ptr<folly::IOBuf> body, std::unique_ptr<proxygen::HTTPMessage> message);
};

class RespContext
{
public:
    ResponseInfo _info;
    bool _headersSent;
    std::unique_ptr<proxygen::ResponseBuilder> response;
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
