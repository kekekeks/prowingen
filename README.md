C# Bindings for proxygen
========================

Its not even an alpha, far from usable. But it gives me 45K requests per second on my laptop and potentially can support SPDY and websockets, since proxygen does support them.

How to compile
--------------

1) Run `./bootstrap.sh` and wait until proxygen installer hangs on its tests, then Ctrl+C
2) Run `./getnative.sh` (it collects .so-files and fixes rpath)
3) Open project from `native` directory in Code::Blocks and compile it
4) Now Sandbox should be runnable from MonoDevelop

