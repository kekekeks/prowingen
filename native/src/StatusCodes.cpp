#include "common.h"

std::string HttpStatusCodes[0x10000];

void InitStatusCodes() {
    for (int c = 0; c < 0x10000; c++)
        HttpStatusCodes[c] = std::string();
    HttpStatusCodes[100] = "Continue";
    HttpStatusCodes[101] = "Switching Protocols";
    HttpStatusCodes[102] = "Processing";
    HttpStatusCodes[200] = "OK";
    HttpStatusCodes[201] = "Created";
    HttpStatusCodes[202] = "Accepted";
    HttpStatusCodes[203] = "Non-Authoritative Information";
    HttpStatusCodes[204] = "No Content";
    HttpStatusCodes[205] = "Reset Content";
    HttpStatusCodes[206] = "Partial Content";
    HttpStatusCodes[207] = "Multi-Status";
    HttpStatusCodes[208] = "Already Reported";
    HttpStatusCodes[226] = "IM Used";
    HttpStatusCodes[300] = "Multiple Choices";
    HttpStatusCodes[301] = "Moved Permanently";
    HttpStatusCodes[302] = "Found";
    HttpStatusCodes[303] = "See Other";
    HttpStatusCodes[304] = "Not Modified";
    HttpStatusCodes[305] = "Use Proxy";
    HttpStatusCodes[306] = "Switch Proxy";
    HttpStatusCodes[307] = "Temporary Redirect";
    HttpStatusCodes[308] = "Permanent Redirect";
    HttpStatusCodes[404] = "error on Polish Wikipedia";
    HttpStatusCodes[400] = "Bad Request";
    HttpStatusCodes[401] = "Unauthorized";
    HttpStatusCodes[402] = "Payment Required";
    HttpStatusCodes[403] = "Forbidden";
    HttpStatusCodes[404] = "Not Found";
    HttpStatusCodes[405] = "Method Not Allowed";
    HttpStatusCodes[406] = "Not Acceptable";
    HttpStatusCodes[407] = "Proxy Authentication Required";
    HttpStatusCodes[408] = "Request Timeout";
    HttpStatusCodes[409] = "Conflict";
    HttpStatusCodes[410] = "Gone";
    HttpStatusCodes[411] = "Length Required";
    HttpStatusCodes[412] = "Precondition Failed";
    HttpStatusCodes[413] = "Request Entity Too Large";
    HttpStatusCodes[414] = "Request-URI Too Long";
    HttpStatusCodes[415] = "Unsupported Media Type";
    HttpStatusCodes[416] = "Requested Range Not Satisfiable";
    HttpStatusCodes[417] = "Expectation Failed";
    HttpStatusCodes[418] = "I'm a teapot";
    HttpStatusCodes[419] = "Authentication Timeout";
    HttpStatusCodes[420] = "Method Failure";
    HttpStatusCodes[420] = "Enhance Your Calm";
    HttpStatusCodes[422] = "Unprocessable Entity";
    HttpStatusCodes[423] = "Locked";
    HttpStatusCodes[424] = "Failed Dependency";
    HttpStatusCodes[426] = "Upgrade Required";
    HttpStatusCodes[428] = "Precondition Required";
    HttpStatusCodes[429] = "Too Many Requests";
    HttpStatusCodes[431] = "Request Header Fields Too Large";
    HttpStatusCodes[440] = "Login Timeout";
    HttpStatusCodes[444] = "No Response";
    HttpStatusCodes[449] = "Retry With";
    HttpStatusCodes[450] = "Blocked by Windows Parental Controls";
    HttpStatusCodes[451] = "Unavailable For Legal Reasons";
    HttpStatusCodes[451] = "Redirect";
    HttpStatusCodes[494] = "Request Header Too Large";
    HttpStatusCodes[495] = "Cert Error";
    HttpStatusCodes[496] = "No Cert";
    HttpStatusCodes[497] = "HTTP to HTTPS";
    HttpStatusCodes[498] = "Token expired/invalid";
    HttpStatusCodes[499] = "Client Closed Request";
    HttpStatusCodes[499] = "Token required";
    HttpStatusCodes[500] = "Internal Server Error";
    HttpStatusCodes[501] = "Not Implemented";
    HttpStatusCodes[502] = "Bad Gateway";
    HttpStatusCodes[503] = "Service Unavailable";
    HttpStatusCodes[504] = "Gateway Timeout";
    HttpStatusCodes[505] = "HTTP Version Not Supported";
    HttpStatusCodes[506] = "Variant Also Negotiates";
    HttpStatusCodes[507] = "Insufficient Storage";
    HttpStatusCodes[508] = "Loop Detected";
    HttpStatusCodes[509] = "Bandwidth Limit Exceeded";
    HttpStatusCodes[510] = "Not Extended";
    HttpStatusCodes[511] = "Network Authentication Required";
    HttpStatusCodes[520] = "Origin Error";
    HttpStatusCodes[521] = "Web server is down";
    HttpStatusCodes[522] = "Connection timed out";
    HttpStatusCodes[523] = "Proxy Declined Request";
    HttpStatusCodes[524] = "A timeout occurred";
    HttpStatusCodes[598] = "Network read timeout error";
    HttpStatusCodes[599] = "Network connect timeout error";

}