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

		public string PathAndQuery { get; private set; }
		public Stream RequestStream { get; private set;}

		internal Request (IntPtr native)
		{
			_native = native;
			_req = (RequestInfo*)_native;
			PathAndQuery = Marshal.PtrToStringAnsi (_req->Url);
			RequestStream = new ProwingenRequestStream (this, _req);
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

