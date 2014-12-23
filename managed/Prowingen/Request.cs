using System;
using System.Runtime.InteropServices;
using System.IO;

namespace Prowingen
{
	public unsafe class Request : IDisposable
	{
		static readonly IRequestWrapper Wrapper = Prowingen.Factory.Native.Value.CreateRequestWrapper();
		IntPtr _native;

		RequestInfo* _req;

		string _pathAndQuery;
		public string PathAndQuery
		{
			get
			{
				CheckDisposed ();
				return _pathAndQuery ?? (_pathAndQuery = Marshal.PtrToStringAnsi (_req->Url));
			}
		}

		string _method;
		public string Method
		{
			get
			{
				CheckDisposed ();
				return _method ?? (_method = Marshal.PtrToStringAnsi (_req->Method));
			}
		}

		string _protocol;
		public string Protocol
		{
			get
			{
				CheckDisposed ();
				if (_protocol != null)
					return _protocol;
				var major = (_req->HttpVersion & 0xFFFF0000u) >> 16;
				var minor = (_req->HttpVersion & 0xFFFFu);
				return _protocol = "HTTP/" + major + "." + minor;
			}
		}

		public bool IsSecure {get; private set;}
		public Stream RequestStream { get; private set;}

		internal Request (IntPtr native)
		{
			_native = native;
			_req = (RequestInfo*)_native;
			RequestStream = new ProwingenRequestStream (this, _req);
			IsSecure = _req->IsSecure != 0;
		}



		public void Dispose ()
		{
			if (_native != IntPtr.Zero)
			{
				Wrapper.Dispose (_native);
				_native = IntPtr.Zero;
			}
		}

		internal void CheckDisposed()
		{
			if (_native == IntPtr.Zero)
				throw new ObjectDisposedException (this.GetType ().Name);
		}
	}
}

