using System;
using Prowingen;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Linq;

namespace Sandbox
{
	class MainClass
	{
		static void Handler(Request req, Response resp)
		{
			using (req)
			{
				resp.SetCode (200, "OK");
				resp.AppendHeader ("Content-Type", "text/plain");
				resp.AppendBody (Encoding.UTF8.GetBytes (req.PathAndQuery + "\n"));
				resp.Complete ();
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
			server.AddAddress ("127.0.0.1", 9001, false);
			server.Start ();

		}
	}
}
