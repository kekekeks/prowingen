using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Prowingen
{
	public static class Factory
	{

		delegate void ThreadInitProc (IntPtr arg);
		[DllImport("/home/kekekeks/Projects/prowingen/native/bin/Debug/libprowingen.so")]
		static extern int CreateFactory(out IntPtr pUnk);

		/*
		[DllImport("/home/kekekeks/Projects/prowingen/native/bin/Debug/libprowingen.so")]
		static extern void SetThreadInitCallback (ThreadInitProc cb);
		[DllImport("/home/kekekeks/Projects/prowingen/native/bin/Debug/libprowingen.so")]
		static extern void ContinueThreadInit (IntPtr arg);

		static void ThreadInit (IntPtr ptr)
		{
			if (handle.IsAllocated)
				ContinueThreadInit (ptr);
		}

		static GCHandle handle;
		*/


		static IProvingenFactory Init()
		{
			IntPtr pUnk;
			CreateFactory (out pUnk);
			var factory = (IProvingenFactory) Marshal.GetObjectForIUnknown (pUnk);
			/*ThreadInitProc cb = ThreadInit;
			handle = GCHandle.Alloc (cb, GCHandleType.Pinned);
			//SetThreadInitCallback (cb);*/
			return factory;
		}

		static readonly Lazy<IProvingenFactory> _factory = new Lazy<IProvingenFactory>(Init);

		public static IHttpServer CreateServer(IRequestHandler handlerFactory)
		{
			return _factory.Value.CreateServer (handlerFactory);

		}

	}
}

