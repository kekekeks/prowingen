using System;
using Microsoft.Owin.Hosting;

namespace WebApiSandbox
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//Environment.SetEnvironmentVariable("OWIN_SERVER", "Prowingen.Owin");
			using(WebApp.Start<Startup>("http://127.0.0.1:9002"))
			{
				Console.ReadLine ();
			}
		}
	}
}
