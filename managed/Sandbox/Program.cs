﻿using System;
using Prowingen;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Sandbox
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var server = Factory.CreateServer ((req, resp) =>
			{
				using (req)
				{
					resp.SetCode (200, "OK");
					resp.AppendHeader ("Content-Type", "text/plain");
					resp.AppendBody (Encoding.UTF8.GetBytes (req.PathAndQuery + "\n"));
					resp.Complete ();
				}
			});
			server.AddAddress ("127.0.0.1", 9001, false);
			server.Start ();

		}
	}
}
