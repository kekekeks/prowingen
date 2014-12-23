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
			//TODO: Stop
		}

		void OnRequest (Request req, Response resp)
		{

			ThreadPool.QueueUserWorkItem (async _ =>
			{
				using (req)
				using (resp.OutputStream)
				{
					EventHandler sendingHeaders = null;
					try
					{
						var env = new Dictionary<string, object> ();
						var reqHeaders = new Dictionary<string, string[]> ();
						env ["owin.RequestBody"] = req.RequestStream;
						env ["owin.RequestHeaders"] = reqHeaders;//TODO: actual request headers
						env ["owin.RequestMethod"] = "GET"; //TODO: actual request method

						reqHeaders.Add("Host",new[]{ "localhost:9002"});

						var pairs = req.PathAndQuery.Split (new[] { '?' }, 2);
						var path = Uri.UnescapeDataString (pairs [0]);
						var query = pairs.Length == 2 ? pairs [1] : string.Empty;
						env ["owin.RequestPath"] = path;
						env ["owin.RequestPathBase"] = "/";
						env ["owin.RequestProtocol"] = "HTTP/1.0";
						env ["owin.RequestQueryString"] = query;
						env ["owin.RequestScheme"] = "http"; //TODO: support for HTTPS/SPDY schemes


						var responseHeaders = new Dictionary<string, string[]> ();
						env ["owin.ResponseBody"] = resp.OutputStream;
						env ["owin.ResponseHeaders"] = responseHeaders;
						env ["owin.ResponseStatusCode"] = 200;

						var headerCallbacks = new List<Tuple<Action<object>, object>> ();
						env ["server.OnSendingHeaders"] = new Action<Action<object>, object> ((cb, state) => headerCallbacks.Add (Tuple.Create (cb, state)));


						sendingHeaders = delegate
						{
							resp.StatusCode = (System.Net.HttpStatusCode)(int)env ["owin.ResponseStatusCode"];
							foreach (var hdr in responseHeaders)
								resp.Headers.Add (hdr.Key, hdr.Value);
						};
						resp.SendingHeaders += sendingHeaders;
						await _app (env);
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

