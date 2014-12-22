using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace Prowingen
{
	public unsafe class Response
	{
		static readonly IResponseWrapper Wrapper = Prowingen.Factory.Native.Value.CreateResponseWrapper();
		IntPtr _native;

		public event EventHandler SendingHeaders;
		public Stream OutputStream { get; private set;}
		public ProwingenResponseHeaders Headers {get; private set;}
		public bool HeadersAreSent {get; private set;}
		HttpStatusCode _statusCode;
		public HttpStatusCode StatusCode
		{
			get
			{
				return _statusCode;
			}
			set
			{
				var v = (int)value;
				if (v > 0x7fff || v < 0)
					throw new ArgumentException ("Invalid HTTP status code");
				_statusCode = value;
			}
		}

		internal Response(IntPtr native)
		{
			_native = native;
			StatusCode = HttpStatusCode.OK;
			OutputStream = new ProwingenResponseStream (this);
			Headers = new ProwingenResponseHeaders (this);
		}


		void OnWrite()
		{
			Check ();
			bool headersWereSent = HeadersAreSent;
			HeadersAreSent = true;
			if (!headersWereSent)
			{
				if(SendingHeaders != null)
					SendingHeaders (this, new EventArgs ());

				Wrapper.SetCode (_native, (ushort)StatusCode, StatusCode.ToString ());
				foreach (var hdr in Headers.Dictionary)
					foreach (var hdrdata in hdr.Value)
						Wrapper.AppendHeader (_native, hdr.Key, hdrdata);
			}
		}

		internal void OnHeader()
		{
			Check ();
			if (HeadersAreSent)
				throw new InvalidOperationException ("Headers are already sent");
		}

		internal void AppendBody(byte[] data, int offset, int size, bool flush = false)
		{
			OnWrite ();
			fixed(byte* sptr = data)
			{
				byte* ptr = sptr;
				ptr += offset;
				Wrapper.AppendBody(_native, new IntPtr((void*)ptr), size, flush);
			}
		}

		void Check()
		{
			if (_native == IntPtr.Zero)
				throw new ObjectDisposedException ("Request is already completed");
		}

		internal void Complete()
		{
			OnWrite ();
			if (_native != IntPtr.Zero)
				Wrapper.Complete (_native);
			_native = IntPtr.Zero;
		}



	}
}

