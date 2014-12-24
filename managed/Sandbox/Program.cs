using System; 
using Prowingen;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace Sandbox
{
	class MainClass
	{
		static void Handler(Request req, Response resp)
		{
			using (req)
			using(resp.OutputStream)
			{
				resp.StatusCode = System.Net.HttpStatusCode.OK;
				//resp.Headers.Add ("Content-Type", "text/plain");
				var buf = Encoding.UTF8.GetBytes (req.PathAndQuery);
				resp.OutputStream.Write (buf, 0, buf.Length);
			}
		}

		public static void Main (string[] args)
		{

			bool pool = args.Contains ("--pool");
			using (var server = Factory.CreateServer ((req, resp) =>
			{
				if (pool)
					ThreadPool.QueueUserWorkItem (_ => Handler (req, resp));
				else
					Handler (req, resp);
			}))
			{
				if (args.Contains ("--gc"))
					new Thread (() =>
					{
						Thread.Sleep (1000);
						while (true)
							GC.Collect (2);
					}).Start ();
				server.AddAddress ("127.0.0.1", 9001, false);
				server.Start ();
				//Console.WriteLine ("Started on 127.0.0.1:9001");
				//Console.ReadLine ();
				Process.Start (new ProcessStartInfo ("wrk", "-c 800 -d 5s -t 10 http://localhost:9001"){ UseShellExecute = false }).WaitForExit ();
				//Console.WriteLine (new WebClient ().DownloadString ("http://localhost:9001/lalala"));
			}
			Console.WriteLine ("Stopped");
		}
	}
}
