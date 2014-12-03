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

		static readonly Lazy<IProvingenFactory> _factory = new Lazy<IProvingenFactory> (Init);

		public static IHttpServer CreateServer (IRequestHandler handlerFactory)
		{
			return _factory.Value.CreateServer (handlerFactory);

		}

	}
}

