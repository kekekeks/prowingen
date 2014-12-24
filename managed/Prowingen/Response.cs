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
		ResponseInfo* _resp;

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
				//_resp->Code = (ushort)v;
			}
		}

		internal Response(IntPtr native)
		{
			_native = native;
			_resp = (ResponseInfo*)native;
			StatusCode = HttpStatusCode.OK;
			OutputStream = new ProwingenResponseStream (this);
			Headers = new ProwingenResponseHeaders (this);
		}

		static string[] StatusCodes = new string[0xffff];

		static Response()
		{
			var statusCodes = (int[])Enum.GetValues (typeof(HttpStatusCode));
			foreach (var code in statusCodes)
				StatusCodes [code] = ((HttpStatusCode)code).ToString ();
		}

		void OnWrite()
		{
			Check ();
			bool headersWereSent = HeadersAreSent;

			if (!headersWereSent)
			{
				if(SendingHeaders != null)
					SendingHeaders (this, new EventArgs ());

				foreach (var hdr in Headers.Dictionary)
					foreach (var hdrdata in hdr.Value)
						Wrapper.AppendHeader (_native, hdr.Key, hdrdata);
				HeadersAreSent = true;
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
			if(data == null || size == 0)
			{
				if (flush)
					Wrapper.AppendBody (_native, IntPtr.Zero, 0, true);
				return;
			}
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

		internal void Complete(byte[] data, int offset, int size)
		{
			if (_native != IntPtr.Zero)
			{
				OnWrite ();
				if (data == null)
					Wrapper.Complete (_native, IntPtr.Zero, 0);
				else
				{
					fixed(byte* sptr = data)
					{
						byte* ptr = sptr;
						ptr += offset;
						Wrapper.Complete (_native, new IntPtr ((void*)ptr), size);
					}
				}
			}
			_native = IntPtr.Zero;
		}



	}
}

