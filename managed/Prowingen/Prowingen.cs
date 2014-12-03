using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Prowingen
{
	public static class Factory
	{
		delegate void CreateFactoryProc (out IntPtr pUnk);

		static IProvingenFactory Init ()
		{
			var path = new[]
			{
				"libprowingen.so",
				"../../../../native/bin/Debug/libprowingen.so",

			}.Select (Path.GetFullPath).Where (File.Exists).First ();

			var linuxLoader = new LinuxLoader ();
			var lib = linuxLoader.LoadLibrary (null, path);
			var pProc = linuxLoader.GetProcAddress (lib, "CreateFactory");

			var createFactory = Marshal.GetDelegateForFunctionPointer<CreateFactoryProc> (pProc);

			IntPtr pUnk;
			createFactory (out pUnk);
			var factory = (IProvingenFactory)Marshal.GetObjectForIUnknown (pUnk);
			return factory;
		}

		internal static readonly Lazy<IProvingenFactory> Native = new Lazy<IProvingenFactory> (Init);

		class RequestHandler : IRequestHandler
		{
			readonly Action<Response> _cb;
			public RequestHandler (Action<Response> cb)
			{
				_cb = cb;
			}

			public void OnRequest(IntPtr pResponse)
			{
				var wrapper = new Response (pResponse);
				_cb (wrapper);
			}
		}

		public static IHttpServer CreateServer (Action<Response> cb)
		{
			return Native.Value.CreateServer (new RequestHandler (cb));
		}
	}
}

