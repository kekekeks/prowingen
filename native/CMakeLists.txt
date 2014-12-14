cmake_minimum_required(VERSION 2.8.4)
project(prowingen)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11 -fPIC -fexceptions -Wall -pthread")

set(SOURCE_FILES
    include/api.h
    include/com.h
    include/common.h
    include/HttpServerWrapper.h
    src/HttpServerWrapper.cpp
    src/iid.cpp
    src/main.cpp
    src/RequestHandlerWrapper.cpp
    src/RequestWrapper.cpp
    src/ResponseWrapper.cpp)

include_directories(include ../proxygen/proxygen/fbthrift/thrift/folly ../proxygen/proxygen/fbthrift ../proxygen)


set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)

LINK_DIRECTORIES(${CMAKE_SOURCE_DIR}/../proxygen/proxygen/fbthrift/thrift/folly/folly/.libs/ ${CMAKE_SOURCE_DIR}/../proxygen/proxygen/httpserver/.libs/)


SET (CMAKE_SHARED_LINKER_FLAGS
    " ${CMAKE_EXE_LINKER_FLAGS}  -Wl,-rpath,'$$ORIGIN'")

add_library(prowingen SHARED ${SOURCE_FILES})
TARGET_LINK_LIBRARIES(prowingen proxygenhttpserver folly)