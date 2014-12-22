using System; 
using Prowingen;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Linq;
using System.IO;

namespace Sandbox
{
	class MainClass
	{
		static void Handler(Request req, Response resp)
		{
			using (req)
			using(var writer = new StreamWriter(resp.OutputStream))
			{
				resp.StatusCode = System.Net.HttpStatusCode.OK;
				resp.Headers.Add ("Content-Type", "text/plain");
				writer.WriteLine (req.PathAndQuery);
			}
		}

		public static void Main (string[] args)
		{
			bool pool = args.Contains ("--pool");
			var server = Factory.CreateServer ((req, resp) =>
			{
				if (pool)
					ThreadPool.QueueUserWorkItem (_ => Handler (req, resp));
				else
					Handler (req, resp);
			});
			if (args.Contains ("--gc"))
				new Thread (() =>
				{
					Thread.Sleep (1000);
					while (true)
						GC.Collect (2);
				}).Start ();
			server.AddAddress ("127.0.0.1", 9001, false);
			server.Start ();

		}
	}
}
