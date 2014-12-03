#ifndef COMMON_H_INCLUDED
#define COMMON_H_INCLUDED
#include "api.h"
#include <folly/Memory.h>
#include <folly/Portability.h>
#include <folly/io/async/EventBaseManager.h>
#include <proxygen/httpserver/HTTPServer.h>
#include <proxygen/httpserver/RequestHandlerFactory.h>


extern proxygen::RequestHandler* CreateHandler(IRequestHandler*handler);
extern IResponseWrapper* CreateResponseWrapper();
#endif // COMMON_H_INCLUDED
