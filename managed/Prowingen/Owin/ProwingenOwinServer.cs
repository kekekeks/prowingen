using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Prowingen.Owin
{
	public class ProwingenOwinServer : IDisposable
	{
		readonly Func<IDictionary<string, object>, Task> _app;
		HttpServer _httpServer;

		public ProwingenOwinServer (Func<IDictionary<string, object>, Task> app, IDictionary<string, object> properties)
		{
			_app = app;
			_httpServer = new HttpServer (OnRequest);
			var addresses = (IList<IDictionary<string, object>>)properties ["host.Addresses"];

			//TODO: support for HTTPS/SPDY schemes
			//TODO: somehow use "path" key, for now ignoring
			foreach (var addr in addresses)
				_httpServer.AddAddress ((string)addr ["host"], int.Parse ((string)addr ["port"]), true);
			_httpServer.Start ();
		}


		public void Dispose ()
		{
			_httpServer.Dispose ();
		}

		void OnRequest (Request req, Response resp)
		{

			ThreadPool.QueueUserWorkItem (async _ =>
			{
				var owin = new Dictionary<string, object> ();
				using (req)
				using (resp.OutputStream)
				{
					EventHandler sendingHeaders = null;
					try
					{

						owin ["owin.RequestBody"] = req.RequestStream;
						owin ["owin.RequestHeaders"] = req.Headers;
						owin ["owin.RequestMethod"] = req.Method;

						var pairs = req.PathAndQuery.Split (new[] { '?' }, 2);
						var path = Uri.UnescapeDataString (pairs [0]);
						var query = pairs.Length == 2 ? pairs [1] : string.Empty;
						owin ["owin.RequestPath"] = path;
						owin ["owin.RequestPathBase"] = "";
						owin ["owin.RequestProtocol"] = req.Protocol;
						owin ["owin.RequestQueryString"] = query;
						owin ["owin.RequestScheme"] = req.IsSecure ? "https" : "http";


						var responseHeaders = new Dictionary<string, string[]> ();
						owin ["owin.ResponseBody"] = resp.OutputStream;
						owin ["owin.ResponseHeaders"] = responseHeaders;
						owin ["owin.ResponseStatusCode"] = 200;

						var headerCallbacks = new List<Tuple<Action<object>, object>> ();
						owin ["server.OnSendingHeaders"] = new Action<Action<object>, object> ((cb, state) => headerCallbacks.Add (Tuple.Create (cb, state)));


						sendingHeaders = delegate
						{
							foreach(var cb in headerCallbacks)
								cb.Item1(cb.Item2);
							resp.StatusCode = (System.Net.HttpStatusCode)(int)owin ["owin.ResponseStatusCode"];
							foreach (var hdr in responseHeaders)
								resp.Headers.Add (hdr.Key, hdr.Value);
						};
						resp.SendingHeaders += sendingHeaders;
						await _app (owin);
						resp.OutputStream.Close ();

					} catch (Exception e)
					{
						resp.SendingHeaders -= sendingHeaders;
						if (!resp.HeadersAreSent)
						{
							resp.Headers.Clear ();
							resp.Headers.Add ("Content-Type", "text/html");
							resp.StatusCode = System.Net.HttpStatusCode.InternalServerError;
							using (var wr = new StreamWriter (resp.OutputStream))
							{
								wr.WriteLine ("500 - Internal Server Error");
								wr.WriteLine (e.ToString ());
							}
						}
					}

				}

			});

		}
	}
}

