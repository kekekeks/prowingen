using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

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

		IDictionary<string, string[]> _headers;
		public IDictionary<string, string[]> Headers
		{
			get
			{
				if (_headers != null)
					return _headers;
				CheckDisposed ();
				var dic = new Dictionary<string, object> ();
				for (var c = 0; c < _req->HeaderCount; c++)
				{
					var key = Marshal.PtrToStringAnsi (_req->Headers [c].Key);
					var value = Marshal.PtrToStringAnsi (_req->Headers [c].Value);

					object current;
					if (!dic.TryGetValue (key, out current))
						dic.Add(key, new string[]{ value }); //Its unlikely to have more than 1 header, so use array for the first one
					else
					{
						var arr = current as string[];
						List<string> lst;
						if (arr != null)
							lst = new List<string>{ arr [0] };
						else
							lst = (List<string>)current;
						lst.Add (value);
						dic [key] = lst;
					}
				}
				var rv = new Dictionary<string, string[]> ();
				foreach(var kp in dic)
				{
					var arr = kp.Value as string[] ?? ((List<string>)kp.Value).ToArray ();
					rv.Add (kp.Key, arr);
				}

				return _headers = rv;
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

