using System;
using System.Runtime.InteropServices;

namespace Prowingen
{
	public unsafe class Request : IDisposable
	{
		static readonly IRequestWrapper Wrapper = Prowingen.Factory.Native.Value.CreateRequestWrapper();
		IntPtr _native;
		[StructLayout(LayoutKind.Sequential)]
		struct RequestInfo
		{
			public IntPtr Url;
		}
		RequestInfo* _req;

		internal Request (IntPtr native)
		{
			_native = native;
			_req = (RequestInfo*)_native;
		}

		string _pathAndQuery;
		public string PathAndQuery 
		{
			get
			{
				return _pathAndQuery = _pathAndQuery ?? Marshal.PtrToStringAnsi (_req->Url);
			}
		}

		public void Dispose ()
		{
			if (_native != IntPtr.Zero)
			{
				Wrapper.Dispose (_native);
				_native = IntPtr.Zero;
			}
		}
	}
}

