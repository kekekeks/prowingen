using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Prowingen
{
	public static class Factory
	{
		delegate void CreateFactoryProc (out IntPtr pUnk);

		unsafe static IProvingenFactory Init ()
		{
			var path = new[]
			{
				"libprowingen.so",
				"../../../../native/bin/libprowingen.so",
				"bin/Debug/libprowingen.so"

			}.Select (Path.GetFullPath).Where (File.Exists).First ();

			var linuxLoader = new LinuxLoader ();
			var lib = linuxLoader.LoadLibrary (null, path);
			var pProc = linuxLoader.GetProcAddress (lib, "CreateFactory");

			var createFactory = Marshal.GetDelegateForFunctionPointer<CreateFactoryProc> (pProc);

			IntPtr pUnk;
			createFactory (out pUnk);
			var factory = (IProvingenFactory)Marshal.GetObjectForIUnknown (pUnk);


			factory.SetThreadInitProc (Marshal.GetFunctionPointerForDelegate (MonoStackFrameDelegate));
			return factory;
		}

		delegate void ProwingenThreadInit(IntPtr ptr);
		static ProwingenThreadInit MonoStackFrameDelegate = MonoStackFrame;

		//We need this to prevent GC from finalizing worker threads
		static void MonoStackFrame(IntPtr arg)
		{
			try
			{
				System.Threading.Thread.CurrentThread.Name = "proxygen worker thread";
				Native.Value.CallThreadInitProc(arg);
			}
			catch(Exception e)
			{
				//We've broken proxygen's internal state. Just give up and die.
				Console.Error.WriteLine ("Crash in proxygen thread!");
				Console.Error.WriteLine (e);
				System.Diagnostics.Process.GetCurrentProcess ().Kill ();
			}

		}


		internal static readonly Lazy<IProvingenFactory> Native = new Lazy<IProvingenFactory> (Init);

		unsafe static Wrappers GetMethodTable()
		{
			var mtable = Native.Value.GetMethodTablePtr ();
			var wrappers = new Wrappers ();
			var fields = typeof(Wrappers).GetFields ();
			for (var c = 0; c < fields.Length; c++)
			{
				var mptr = mtable [c];
				var del = Marshal.GetDelegateForFunctionPointer (mptr, fields [c].FieldType);
				fields [c].SetValue (wrappers, del);
			}
			return wrappers;
		}

		internal static readonly Lazy<Wrappers> Wrappers = new Lazy<Wrappers>(GetMethodTable);


		public static HttpServer CreateServer (Action<Request, Response> cb)
		{
			return new HttpServer (cb);
		}
	}
}

