#ifndef HTTPSERVERWRAPPER_H
#define HTTPSERVERWRAPPER_H
#include "api.h"
#include <folly/Memory.h>
#include <folly/Portability.h>
#include <folly/io/async/EventBaseManager.h>
#include <proxygen/httpserver/HTTPServer.h>
#include <proxygen/httpserver/RequestHandlerFactory.h>

class HttpServerWrapper : public ComObject<IHttpServer, &IID_IHttpServer>
{
    public:
        HttpServerWrapper(IRequestHandlerFactory*factory);
        virtual ~HttpServerWrapper();
        virtual HRESULT AddAddress(char*host, int port, bool lookup) override;
        virtual HRESULT Start() override;
    protected:
    private:
        proxygen::HTTPServer* _server;
        std::vector<proxygen::HTTPServer::IPConfig> _ips;
};

#endif // HTTPSERVERWRAPPER_H
