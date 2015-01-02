#pragma once

#include "api.h"
#include <folly/Memory.h>
#include <folly/Portability.h>
#include <folly/io/async/EventBaseManager.h>
class OpaqueInputStream
{
private:
    ProwingenOpaqueInputStreamHandler _handler;
    std::recursive_mutex _lock;
    bool _inside_handler;
    std::shared_ptr<OpaqueInputStream> _this;
    bool _disposed;
public:
    OpaqueInputStream(ProwingenOpaqueInputStreamHandler handler);
    void OnBody(std::unique_ptr<folly::IOBuf> buffer);
    void Dispose();
    std::shared_ptr<OpaqueInputStream> GetPtr();
};

class OpaqueInputStreamWrapper
{
public:
    folly::EventBase* _eventBase;
    unique_ptr<folly::IOBuf> _body;


    std::shared_ptr<OpaqueInputStream> _stream;
    OpaqueInputStreamWrapper();

    void OnBody(std::unique_ptr<folly::IOBuf> buffer);
};

extern OpaqueInputStream* InitializeOpaqueInputStream(std::shared_ptr<OpaqueInputStreamWrapper> wrapper,  ProwingenOpaqueInputStreamHandler handler);


