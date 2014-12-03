using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Prowingen
{
	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("e4ea9822-30c8-4319-9d80-5a0e48de0be5")]
	public interface IProvingenFactory
	{
		[MethodImplAttribute (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IHttpServer CreateServer (IRequestHandlerFactory factory);
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
	[Guid("c09bb425-a496-45c3-b6bc-a7f0060b6064")]
	public interface IRequestHandlerFactory
	{
		IRequestHandler CreateHandler();
	}


	[ComImport, ComVisible (true), InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("6307a020-22f4-462f-aee0-15e7bc555c4d")]
	public interface IRequestHandler
	{
	}

}