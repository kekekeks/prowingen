using Owin;
using System.Web.Http;
using Owin.WebSocket.Extensions;
using Owin.WebSocket;
using System.Text;

namespace WebApiSandbox
{
	public class SocketHandler : WebSocketConnection
	{
		static byte[] Hello = Encoding.UTF8.GetBytes("Hello");
		public override System.Threading.Tasks.Task OnOpen ()
		{
			return SendAsyncText (Hello, true);
		}
	}

}
