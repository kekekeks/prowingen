using System;
using Prowingen;
using System.Runtime.InteropServices;
namespace Sandbox
{
	class MainClass
	{
		[ComVisible(true)]
		class RequestHandler : IRequestHandler
		{

		}

		[ComVisible(true)]
		class RequestFactory : IRequestHandlerFactory
		{
			public IRequestHandler CreateHandler()
			{
				return new RequestHandler ();
			}
		}

		public static void Main (string[] args)
		{
			var server = Factory.CreateServer (new RequestFactory ());
			server.AddAddress ("127.0.0.1", 9001, false);
			server.Start ();
			Marshal.ReleaseComObject (server);
			server = null;
			GC.Collect (2);
			Console.WriteLine ("WAT");
		}
	}
}
