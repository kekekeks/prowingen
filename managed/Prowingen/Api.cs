using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Prowingen
{

	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("e4ea9822-30c8-4319-9d80-5a0e48de0be5")]
	interface IProvingenFactory
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IHttpServer CreateServer (IntPtr factory);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IResponseWrapper CreateResponseWrapper ();
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IRequestWrapper CreateRequestWrapper ();
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void SetThreadInitProc (IntPtr newProc);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void CallThreadInitProc (IntPtr arg);
	}


	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("918cfa9b-a766-41e7-9c4b-330954d01b47")]
	interface IHttpServer
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void AddAddress ([MarshalAs (UnmanagedType.LPStr)]string host, ushort port, bool resolve);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Start();
	}
		
	delegate void RequestHandlerDelegate(IntPtr request, IntPtr response);

	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("24e1a430-27f6-4fd4-a0a4-95e9dea8d51a")]
	interface IResponseWrapper
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void SetCode(IntPtr r, ushort code, [MarshalAs (UnmanagedType.LPStr)]string status);

		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void AppendHeader(IntPtr r, [MarshalAs (UnmanagedType.LPStr)]string key, [MarshalAs (UnmanagedType.LPStr)]string value);

		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void AppendBody(IntPtr r, IntPtr data, int size, bool flush);

		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Complete(IntPtr r);
	}

	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("ab57d7ab-8825-4d9c-9c7b-d03c2f2884ee")]
	interface  IRequestWrapper
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Dispose (IntPtr p);
	}

}