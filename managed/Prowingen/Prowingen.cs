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
				"bin/Debug/libprowingen.so"

			}.Select (Path.GetFullPath).Where (File.Exists).First ();

			var linuxLoader = new LinuxLoader ();
			var lib = linuxLoader.LoadLibrary (null, path);
			var pProc = linuxLoader.GetProcAddress (lib, "CreateFactory");

			var createFactory = Marshal.GetDelegateForFunctionPointer<CreateFactoryProc> (pProc);

			IntPtr pUnk;
			createFactory (out pUnk);
			var factory = (IProvingenFactory)Marshal.GetObjectForIUnknown (pUnk);


			var originalThreadInit = factory.SetThreadInitProc (Marshal.GetFunctionPointerForDelegate (MonoStackFrameDelegate));
			ProwingenThreadInitDelegate = Marshal.GetDelegateForFunctionPointer<ProwingenThreadInit> (originalThreadInit);

			return factory;
		}

		delegate void ProwingenThreadInit(IntPtr ptr);
		static ProwingenThreadInit MonoStackFrameDelegate = MonoStackFrame;
		static ProwingenThreadInit ProwingenThreadInitDelegate;

		//We need this to prevent GC from finalizing worker threads
		static void MonoStackFrame(IntPtr arg)
		{
			try
			{
				System.Threading.Thread.CurrentThread.Name = "proxygen worker thread";
				ProwingenThreadInitDelegate (arg);
			}
			catch(Exception e)
			{
				//We've broken proxygen's internal state. Just give up and die.
				Console.Error.WriteLine (e);
				System.Diagnostics.Process.GetCurrentProcess ().Kill ();
			}

		}


		internal static readonly Lazy<IProvingenFactory> Native = new Lazy<IProvingenFactory> (Init);

		class RequestHandler : IRequestHandler
		{
			readonly Action<Request, Response> _cb;
			public RequestHandler (Action<Request, Response> cb)
			{
				_cb = cb;
			}

			public void OnRequest(IntPtr pRequest, IntPtr pResponse)
			{
				_cb (new Request (pRequest), new Response (pResponse));
			}
		}

		public static IHttpServer CreateServer (Action<Request, Response> cb)
		{
			return Native.Value.CreateServer (new RequestHandler (cb));
		}
	}
}

