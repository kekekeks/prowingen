/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 *
 */
#pragma once

#include <folly/ScopeGuard.h>
#include <proxygen/httpserver/ResponseHandler.h>

    class CustomResponseBuilder {

    private:
        proxygen::ResponseHandler* const txn_{nullptr};

        std::unique_ptr<proxygen::HTTPMessage> headers_;
        std::unique_ptr<folly::IOBuf> body_;

        // If true, sends EOM.
        bool sendEOM_{false};
    public:
        CustomResponseBuilder(proxygen::ResponseHandler* txn): txn_(txn) {
        }

        CustomResponseBuilder& status(uint16_t code, std::string message) {
            if(!headers_)
                headers_ = folly::make_unique<proxygen::HTTPMessage>();
            headers_->setHTTPVersion(1, 1);
            headers_->setStatusCode(code);
            headers_->setStatusMessage(message);
            return *this;
        }

        template <typename T>
        CustomResponseBuilder& header(const std::string& headerIn, const T& value) {
            CHECK(headers_) << "You need to call `status` before adding headers";
            headers_->getHeaders().add(headerIn, value);
            return *this;
        }

        template <typename T>
        CustomResponseBuilder& header(proxygen::HTTPHeaderCode code, const T& value) {
            CHECK(headers_) << "You need to call `status` before adding headers";
            headers_->getHeaders().add(code, value);
            return *this;
        }

        CustomResponseBuilder& body(std::unique_ptr<folly::IOBuf> bodyIn) {
            if (bodyIn) {
                if (body_) {
                    body_->prependChain(std::move(bodyIn));
                } else {
                    body_ = std::move(bodyIn);
                }
            }

            return *this;
        }

        template <typename T>
        CustomResponseBuilder& body(T&& t) {
            return body(folly::IOBuf::maybeCopyBuffer(
                    folly::to<std::string>(std::forward<T>(t))));
        }

        CustomResponseBuilder& closeConnection() {
            return header(proxygen::HTTPHeaderCode::HTTP_HEADER_CONNECTION, "close");
        }

        void sendWithEOM() {
            sendEOM_ = true;
            send();
        }

        void send() {
            // Once we send them, we don't want to send them again
            SCOPE_EXIT { headers_.reset(); };

            // By default, chunked
            bool chunked = true;

            // If we have complete response, we can use Content-Length and get done
            if (headers_ && sendEOM_) {
                chunked = false;
            }

            if (headers_) {
                // We don't need to add Content-Length or Encoding for 1xx responses
                if (headers_->getStatusCode() >= 200) {
                    if (chunked) {
                        headers_->setIsChunked(true);
                    } else {
                        const auto len = body_ ? body_->computeChainDataLength() : 0;
                        headers_->getHeaders().add(
                                proxygen::HTTPHeaderCode::HTTP_HEADER_CONTENT_LENGTH,
                                folly::to<std::string>(len));
                    }
                }

                txn_->sendHeaders(*headers_);
            }

            if (body_) {
                if (chunked) {
                    txn_->sendChunkHeader(body_->computeChainDataLength());
                    txn_->sendBody(std::move(body_));
                    txn_->sendChunkTerminator();
                } else {
                    txn_->sendBody(std::move(body_));
                }
            }

            if (sendEOM_) {
                txn_->sendEOM();
            }
        }

        enum class UpgradeType {
            CONNECT_REQUEST = 0,
            HTTP_UPGRADE,
        };

        void acceptUpgradeRequest(UpgradeType upgradeType) {
            CHECK(headers_);
            if (upgradeType == UpgradeType::CONNECT_REQUEST) {
                headers_->constructDirectResponse({1, 1}, 200, "OK");
            } else {
                headers_->constructDirectResponse({1, 1}, 101, "Switching Protocols");
            }
            txn_->sendHeaders(*headers_);
        }

    };