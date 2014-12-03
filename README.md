C# Bindings for proxygen
========================

Its not even an alpha, far from usable. But it gives me 45K requests per second on my laptop and potentially can support SPDY and websockets, since proxygen does support them.

How to compile
--------------

- Run `./bootstrap.sh` and wait until proxygen installer hangs on its tests, then Ctrl+C
- Run `./getnative.sh` (it collects .so-files and fixes rpath)
- Open project from `native` directory in Code::Blocks and compile it
- Now Sandbox should be runnable from MonoDevelop

