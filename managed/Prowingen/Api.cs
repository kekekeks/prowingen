﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Prowingen
{

	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("e4ea9822-30c8-4319-9d80-5a0e48de0be5")]
	interface IProvingenFactory
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IHttpServer CreateServer (IRequestHandler factory);
	}


	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("918cfa9b-a766-41e7-9c4b-330954d01b47")]
	public interface IHttpServer
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void AddAddress ([MarshalAs (UnmanagedType.LPStr)]string host, int port, bool resolve);
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Start();
	}
		
	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("6307a020-22f4-462f-aee0-15e7bc555c4d")]
	public interface IRequestHandler
	{
		void OnRequest (IRequest request, IResponse response);
	}

	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("24e1a430-27f6-4fd4-a0a4-95e9dea8d51a")]
	public interface IResponse
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void SetCode(int code, [MarshalAs (UnmanagedType.LPStr)]string status);

		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void AppendHeader([MarshalAs (UnmanagedType.LPStr)]string key, [MarshalAs (UnmanagedType.LPStr)]string value);

		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void AppendBody(IntPtr data, int size, bool flush);

		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Complete();
	}

	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("ab57d7ab-8825-4d9c-9c7b-d03c2f2884ee")]
	public interface  IRequest
	{

	}

}