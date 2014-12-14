using System;
using System.Runtime.InteropServices;

namespace Prowingen
{
	public class HttpServer
	{
		#pragma warning disable 0414
		private GCHandle _httpCallbackHandle;
		#pragma warning restore 0414

		private Action<Request, Response> _handler;
		private IHttpServer _server;
		internal HttpServer (Action<Request, Response> handler)
		{
			_handler = handler;
			var httpCallback = new RequestHandlerDelegate (OnRequest);
			_httpCallbackHandle = GCHandle.Alloc (httpCallback);
			var pFunc = Marshal.GetFunctionPointerForDelegate (httpCallback);
			_server = Factory.Native.Value.CreateServer (pFunc);
		}
			
		void OnRequest(IntPtr pRequest, IntPtr pResponse)
		{
			_handler (new Request (pRequest), new Response (pResponse));
		}


		public void AddAddress (string host, int port, bool resolve)
		{
			_server.AddAddress (host, port, resolve);
		}

		public void Start()
		{
			_server.Start ();
		}
	}
}

