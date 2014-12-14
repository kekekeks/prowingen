#ifndef HTTPSERVERWRAPPER_H
#define HTTPSERVERWRAPPER_H
#include "common.h"

class HttpServerWrapper : public ComObject<IHttpServer, &IID_IHttpServer>
{
    public:
        HttpServerWrapper(ProwingenRequestHandler handler);
        virtual ~HttpServerWrapper();
        virtual HRESULT AddAddress(char*host, int port, bool lookup) override;
        virtual HRESULT Start() override;
    protected:
    private:
        proxygen::HTTPServer* _server;
        std::vector<proxygen::HTTPServer::IPConfig> _ips;
};



#endif // HTTPSERVERWRAPPER_H
