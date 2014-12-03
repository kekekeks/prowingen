using System;
using Prowingen;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace Sandbox
{
	class MainClass
	{
		[ComVisible(true)]
		class RequestHandler : IRequestHandler
		{
			public void OnRequest(IRequest req, IResponse resp)
			{
				resp.SetCode (200, "OK");
				resp.AppendHeader ("Content-Type", "text/plain");
				resp.AppendBody (Encoding.UTF8.GetBytes ("Hello world!\n"));
				resp.Complete ();
			}
		}

		public static void Main (string[] args)
		{
			var server = Factory.CreateServer (new RequestHandler ());
			server.AddAddress ("127.0.0.1", 9001, false);
			server.Start ();

		}
	}
}
