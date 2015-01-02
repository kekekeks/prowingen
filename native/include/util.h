#pragma once
#include <folly/Memory.h>
#include <folly/Portability.h>
#include <folly/io/async/EventBaseManager.h>
#include <proxygen/httpserver/HTTPServer.h>
#include <proxygen/httpserver/RequestHandlerFactory.h>


struct EventBaseHolder
{
    folly::EventBase* EventBase;
};
extern folly::ThreadLocalPtr<EventBaseHolder> ThreadEventBase;

inline folly::EventBase*GetCurrentEventBase()
{
    auto holder = ThreadEventBase.get();
    if(holder== nullptr)
        return nullptr;
    return  holder->EventBase;
}

inline void ExecOnEventBase(folly::EventBase*base, folly::Cob cb)
{
    auto currentEventBase = GetCurrentEventBase();
    if(currentEventBase != base)
        base->runInEventBaseThread(cb);
    else
        cb();

}