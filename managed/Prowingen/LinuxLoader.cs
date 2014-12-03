using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace Prowingen
{
	class LinuxLoader
	{
		// ReSharper disable InconsistentNaming
		[DllImport("libdl.so.2")]
		private static extern IntPtr dlopen(string path, int flags);
		[DllImport ("libdl.so.2")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);
		[DllImport ("libdl.so.2")]
		private static extern IntPtr dlerror ();
		// ReSharper restore InconsistentNaming
		static string DlError()
		{
			return Marshal.PtrToStringAuto(dlerror());
		}
		public IntPtr LoadLibrary(string basePath, string dll)
		{
			if (basePath != null)
				dll = System.IO.Path.Combine(basePath, dll);
			var handle = dlopen(dll, 1);
			if (handle == IntPtr.Zero)
				throw new Win32Exception(DlError());
			return handle;
		}
		public IntPtr GetProcAddress(IntPtr dll, string proc)
		{
			var ptr = dlsym(dll, proc);
			if (ptr == IntPtr.Zero)
				throw new Win32Exception(DlError());
			return ptr;
		}
	}
}

