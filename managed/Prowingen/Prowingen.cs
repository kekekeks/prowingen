using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Prowingen
{
	public static class Factory
	{

		[DllImport("/home/kekekeks/Projects/prowingen/native/bin/Debug/libprowingen.so")]
		static extern int CreateFactory(out IntPtr pUnk);
		public static IHttpServer CreateServer(IRequestHandlerFactory handlerFactory)
		{
			IntPtr pUnk;
			CreateFactory (out pUnk);

			var factory = (IProvingenFactory) Marshal.GetObjectForIUnknown (pUnk);
			var server = factory.CreateServer (handlerFactory);
			Marshal.FinalReleaseComObject (factory);

			return server;

		}

	}
}

