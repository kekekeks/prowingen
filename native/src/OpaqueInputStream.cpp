#include "util.h"
#include "OpaqueInputStream.h"

OpaqueInputStream::OpaqueInputStream(ProwingenOpaqueInputStreamHandler handler)
{
    _handler = handler;
    _inside_handler = false;
    _this = std::shared_ptr<OpaqueInputStream>(this);
    _disposed = false;
}

void OpaqueInputStream::OnBody(std::unique_ptr<folly::IOBuf> ubuffer) {
    if(_handler == NULL || _disposed || ubuffer.get() == NULL)
        return;

    _lock.lock();
    if(_handler == NULL || _disposed) {
        _lock.unlock();
        return;
    }
    _inside_handler = true;


    folly::IOBuf* pBody = ubuffer.get();
    if(pBody == NULL)
        return;

    if(pBody == pBody->next())
    {
        IOBufInfo nfo = {pBody->data(), pBody->length()};
        _handler(1, &nfo);
    }
    else
    {
        auto buffer = pBody;
        auto buffers = std::vector<IOBufInfo>();
        while(1) {
            IOBufInfo nfo = {pBody->data(), pBody->length()};
            buffers.push_back(nfo);
            if(pBody == buffer->next())
                break;
            buffer = buffer->next();
        }
        _handler(buffers.size(), buffers.data());
    }


    _inside_handler = false;
    _lock.unlock();
    if(_disposed)
        _this.reset();
}

void OpaqueInputStream::Dispose() {
    _lock.lock();
    _disposed = true;
    if(!_inside_handler)
        _this.reset();
    _lock.unlock();
}


std::shared_ptr<OpaqueInputStream> OpaqueInputStream::GetPtr() {
    return _this;
}




OpaqueInputStreamWrapper::OpaqueInputStreamWrapper()
{
    _eventBase = GetCurrentEventBase();
    _stream = NULL;
}

void OpaqueInputStreamWrapper::OnBody(std::unique_ptr<folly::IOBuf> buffer) {
    if(_stream)
        _stream->OnBody(std::move(buffer));
    else
    {
        if (_body) {
            _body->prependChain(std::move(buffer));
        } else {
            _body = std::move(buffer);
        }
    }
}

extern OpaqueInputStream* InitializeOpaqueInputStream(std::shared_ptr<OpaqueInputStreamWrapper> wrapper,  ProwingenOpaqueInputStreamHandler handler)
{
    auto pStream = new OpaqueInputStream(handler);
    std::shared_ptr<OpaqueInputStream> sptr = pStream->GetPtr();
    ExecOnEventBase(wrapper->_eventBase, [=](){
        wrapper->_stream = sptr;
        sptr->OnBody(std::move(wrapper->_body));
    });
    return pStream;
}

extern void ApiDisposeOpaqueInputStream(void*handle)
{
    ((OpaqueInputStream*)handle)->Dispose();
}
