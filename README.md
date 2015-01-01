prOWINgen - C# Bindings for proxygen
========================

The goal is to create OWIN server based on Facebook's [proxygen](https://github.com/facebook/proxygen/). With websockets and so on. For now I'm working on the bindings.

Currently I've managed to get 100K requests per second handling requests on *threads from ThreadPool* on my laptop with raw listener. Mono's HttpListener performs way worse, 15K requests per second on the same hardware. [evhttp-sharp](https://github.com/kekekeks/evhttp-sharp) can only do this on it's own threads, i. e. ~90K rps without ThreadPool and ~35K rps with ThreadPool. With WebApi I can get 20-22K rps for now, not sure if that's sufficient, but still faster than anything else. Anyway, I'm doing that mostly to get websocket support.

The plan
--------


|Feature|Status
| ------------- |:-------------:|
|Infrastructure for handling requests with native wrapper|Done|
|Thread pool support|Done|
|OWIN support|Done|
|Performance optimization|Some|
|[OWIN Opaque Stream](http://owin.org/spec/extensions/owin-OpaqueStream-Extension-v0.2.0.htm)|In progress|
|[OWIN WebSocket](http://owin.org/spec/extensions/owin-WebSocket-Extension-v0.4.0.htm)|In progress|
|deb-packages|Planned|
|HTTPS/SPDY endpoints|Planned|


It seems that proxygen doesn't support websockets yet, but it does support protocol upgrade, so I'll use managed implementation for now.

How to compile
--------------

- Run `./bootstrap.sh`
- Run `./buildnative.sh` (it also collects .so-files from proxygen and fixes rpath)
- Run `xbuild` in `managed` or just open `managed.sln` in MonoDevelop

Examples
-------

**OWIN**

Follow this [tutorial for WebApi](http://www.asp.net/web-api/overview/hosting-aspnet-web-api/use-owin-to-self-host-web-api) or find one for your framework. Then either set `startOptions.ServerFactory` or `OWIN_SERVER` environment variable to `Prowingen.Owin`. Since there is no packaging yet, libraries should be in the same directory.


```csharp
using(WebApp.Start<Startup>(new StartOptions("http://127.0.0.1:9002")
{
	ServerFactory = "Prowingen.Owin"
}))
{
	Console.ReadLine ();
}
```

**Raw listener**

```csharp
static void Handler(Request req, Response resp)
{
	using (req)
	using(var writer = new StreamWriter(resp.OutputStream))
	{
		resp.StatusCode = System.Net.HttpStatusCode.OK;
		resp.Headers.Add ("Content-Type", "text/plain");
		writer.WriteLine (req.PathAndQuery);
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


