using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace Prowingen
{
	public class ProwingenWebSocketContext : WebSocketContext
	{
		WebSocket _socket;

		public ProwingenWebSocketContext (IDictionary<string, object> env, WebSocket socket)
		{
			_socket = socket;
		}

		#region implemented abstract members of WebSocketContext

		public override System.Net.CookieCollection CookieCollection
		{
			get
			{
				throw new NotSupportedException ();
			}
		}

		public override System.Collections.Specialized.NameValueCollection Headers
		{
			get
			{
				throw new NotSupportedException ();
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return false;
			}
		}

		public override bool IsLocal
		{
			get
			{
				throw new NotSupportedException ();
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				throw new NotSupportedException ();
			}
		}

		public override string Origin
		{
			get
			{
				throw new NotSupportedException ();
			}
		}

		public override Uri RequestUri
		{
			get
			{
				throw new NotSupportedException ();
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return "";
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				yield break;
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return "13";
			}
		}

		public override System.Security.Principal.IPrincipal User
		{
			get
			{
				return null;
			}
		}

		public override WebSocket WebSocket
		{
			get
			{
				return _socket;
			}
		}

		#endregion
	}
}

