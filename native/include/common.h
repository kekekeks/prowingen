#ifndef COMMON_H_INCLUDED
#define COMMON_H_INCLUDED
#include "api.h"
#include <folly/Memory.h>
#include <folly/Portability.h>
#include <folly/io/async/EventBaseManager.h>
#include <proxygen/httpserver/HTTPServer.h>
#include <proxygen/httpserver/RequestHandlerFactory.h>


class ReqContext
{
    public:
        RequestInfo _info;
        std::unique_ptr<folly::IOBuf> _body;
        ReqContext(std::unique_ptr<folly::IOBuf> body, std::unique_ptr<proxygen::HTTPMessage> headers);
};

extern proxygen::RequestHandler* CreateHandler(IRequestHandler*handler);
extern IResponseWrapper* CreateResponseWrapper();
extern IRequestWrapper* CreateRequestWrapper();
#endif // COMMON_H_INCLUDED
