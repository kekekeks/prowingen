#ifndef HTTPSERVERWRAPPER_H
#define HTTPSERVERWRAPPER_H
#include "common.h"

class HttpServerWrapper : public ComObject<IHttpServer, &IID_IHttpServer>
{
    public:
        HttpServerWrapper(ProwingenRequestHandler handler);
        virtual ~HttpServerWrapper();
        virtual HRESULT AddAddress(char*host, uint16_t port, bool lookup) override;
        virtual HRESULT Start(char* exceptionBuffer) override;
        virtual HRESULT Stop() override;
    protected:
    private:
        proxygen::HTTPServer* _server;
        std::vector<proxygen::HTTPServer::IPConfig> _ips;
        ProwingenRequestHandler _handler;
};



#endif // HTTPSERVERWRAPPER_H
