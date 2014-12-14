C# Bindings for proxygen
========================

The goal is to create OWIN server based on Facebook's [proxygen](https://github.com/facebook/proxygen/). With websockets and so on. For now I'm working on the bindings.

Currently I've managed to get 100K requests per second handling requests on *threads from ThreadPool* on my laptop. Mono's HttpListener performs way worse, 15K requests per second on the same hardware. [evhttp-sharp](https://github.com/kekekeks/evhttp-sharp) can only do this on it's own threads, i. e. ~90K rps without ThreadPool and ~35K rps with ThreadPool.


The plan
--------

- Basic infrastructure for handling requests with native wrapper (done)
- Thread pool support (done)
- Basic OWIN middleware
- Websockets
- Packaging (deb-packages + glue assembly to be published via NuGet)


How to compile
--------------

- Run `./bootstrap.sh` and wait until proxygen installer hangs on its tests, then Ctrl+C
- Run `./getnative.sh` (it collects .so-files and fixes rpath)
- Open project from `native` directory in Code::Blocks and compile it
- Now Sandbox should be runnable from MonoDevelop


Example
-------

```csharp
static void Handler(Request req, Response resp)
{
	using (req)
	{
		resp.SetCode (200, "OK");
		resp.AppendHeader ("Content-Type", "text/plain");
		resp.AppendBody (Encoding.UTF8.GetBytes (req.PathAndQuery + "\n"));
		resp.Complete ();
	}
}

public static void Main (string[] args)
{
	bool pool = args.Contains ("--pool");
	var server = Factory.CreateServer ((req, resp) =>
	{
		if (pool)
			ThreadPool.QueueUserWorkItem (_ => Handler (req, resp));
		else
			Handler (req, resp);
	});
	server.AddAddress ("127.0.0.1", 9001, false);
	server.Start ();
}
```


