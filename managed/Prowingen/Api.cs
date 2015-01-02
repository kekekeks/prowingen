using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Prowingen
{

	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("e4ea9822-30c8-4319-9d80-5a0e48de0be5")]
	unsafe interface IProvingenFactory
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IHttpServer CreateServer (IntPtr factory);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void SetThreadInitProc (IntPtr newProc);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void CallThreadInitProc (IntPtr arg);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IntPtr* GetMethodTablePtr ();
	}


	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("918cfa9b-a766-41e7-9c4b-330954d01b47")]
	unsafe interface IHttpServer
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void AddAddress ([MarshalAs (UnmanagedType.LPStr)]string host, ushort port, bool resolve);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Start (void* charBuffer);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Stop ();
	}
		
	delegate void RequestHandlerDelegate(IntPtr request, IntPtr response);
	unsafe delegate void OpaqueInputStreamHandlerDelegate(int count, IoBufInfo*buffers);
	#pragma warning disable 649
	unsafe class Wrappers
	{
		public delegate void DisposeDelegate (IntPtr p);
		public DisposeDelegate DisposeRequest;
		public delegate void AppendHeaderDelegate(IntPtr r, [MarshalAs (UnmanagedType.LPStr)]string key, [MarshalAs (UnmanagedType.LPStr)]string value);
		public AppendHeaderDelegate AppendHeader;
		public delegate void AppendBodyDelegate(IntPtr r, IntPtr data, int size, bool flush);
		public AppendBodyDelegate AppendBody;
		public delegate void CompleteDelegate(IntPtr r, IntPtr data, int size);
		public CompleteDelegate Complete;
		public delegate void UpgradeDelegate(IntPtr r);
		public UpgradeDelegate Upgrade;
		public delegate IntPtr UpgradeToOpaqueInputStreamDelegate(IntPtr context, IntPtr handler);
		public UpgradeToOpaqueInputStreamDelegate UpgradeToOpaqueInputStream;
		public DisposeDelegate DisposeOpaqueInputStream;
	}

	[StructLayout(LayoutKind.Sequential)]
	unsafe struct IoBufInfo
	{
		public byte* Data;
		public ulong Size;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct HttpHeader
	{
		public IntPtr Key;
		public IntPtr Value;
	}

	[StructLayout(LayoutKind.Sequential)]
	unsafe struct RequestInfo
	{
		public IntPtr Url;
		public long BufferCount;
		public IoBufInfo* Buffers;
		public IntPtr Method;
		public ulong HttpVersion;
		public int IsSecure;
		public long HeaderCount;
		public HttpHeader* Headers;
		public int IsUpgradable;
	}


	[StructLayout(LayoutKind.Sequential)]
	unsafe struct ResponseInfo
	{
		public ushort Code;

	}

}