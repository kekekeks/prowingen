
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using OpaqueUpgrade = System.Action

	<System.Collections.Generic.IDictionary<string, object>, // Parameters
System.Func // OpaqueFunc callback

<System.Collections.Generic.IDictionary<string, object>, // Opaque environment

System.Threading.Tasks.Task // Complete

>

>;
namespace Prowingen
{
	public class OpaqueWebSocketSetup
	{

		public static void SetupEnvironment(IDictionary<string, object> environment)
		{
			var headers = (IDictionary<string, string[]>)environment["owin.RequestHeaders"];

			string[] upgrade, connection, key = null, version;
			var capable = headers.TryGetValue("Upgrade", out upgrade) && headers.TryGetValue("Connection", out connection) &&
				headers.TryGetValue("Sec-WebSocket-Key", out key) &&
				headers.TryGetValue("Sec-WebSocket-Version", out version) &&
				upgrade[0].Contains("websocket") && connection[0].Contains("Upgrade") && version[0] == "13";
			if (capable)
				environment["websocket.Accept"] =
					new Action<IDictionary<string, object>, Func<IDictionary<string, object>, Task>>((_, callback) =>
					{
						var respHeaders = (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];
						using (var sha = SHA1.Create())
							respHeaders["Sec-WebSocket-Accept"] = new[]
						{
							Convert.ToBase64String(
								sha.ComputeHash(Encoding.UTF8.GetBytes(key[0] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11")))
						};
						respHeaders["Connection"] = new[] {"Upgrade"};
						respHeaders["Upgrade"] = new[] {"Websocket"};
						environment["owin.ResponseStatusCode"] = 101;
						((OpaqueUpgrade) environment["opaque.Upgrade"])(null, opaqueEnv => new ProwingenWebSocket(opaqueEnv, environment).Handle(callback));
					});
			else
				//Remove existing implementation for debugging purposes
				environment.Remove("websocket.Accept");
		}

		public static Func<AppFunc, AppFunc> Middleware
		{
			get
			{
				return next => env =>
				{
					SetupEnvironment(env);
					return next(env);
				};
			}
		}

	}
}
