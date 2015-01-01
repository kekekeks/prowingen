using Owin;
using System.Web.Http;
using Owin.WebSocket.Extensions;
using System.IO;
using Microsoft.Owin;
using System.Threading.Tasks;
using System;

namespace WebApiSandbox
{
	public class Startup
	{
		public void Configuration (IAppBuilder appBuilder)
		{ 
			// Configure Web API for self-host. 
			var config = new HttpConfiguration (); 

			config.MapHttpAttributeRoutes ();
			config.Routes.MapHttpRoute (
				name: "DefaultApi", 
				routeTemplate: "api/{controller}/{id}", 
				defaults: new { id = RouteParameter.Optional } 
			); 
			appBuilder.MapWebSocketRoute<SocketHandler> ("/ws");
			appBuilder.Use(Handler);
			appBuilder.UseWebApi (config); 
		}

		private static string BasePath;

		static Startup()
		{
			var path = typeof (Startup).Assembly.GetModules()[0].FullyQualifiedName;
			while (!Directory.Exists(Path.Combine(path, "web")))
				path = Path.GetFullPath(Path.Combine(path, ".."));
			BasePath = Path.GetFullPath(Path.Combine(path, "web"));
		}



		private async Task Handler(IOwinContext context, Func<Task> next)
		{
			var file = context.Request.Path.Value.Replace("/", "").Trim();
			if (file.Length == 0)
				file = "ws.html";
			file = Path.Combine(BasePath, file);
			if (!File.Exists (file))
			{
				await next ();
				return;
			}
			if (file.ToLower().EndsWith(".html"))
				context.Response.ContentType = "text/html";
			using (var stream = File.OpenRead(file))
				await stream.CopyToAsync(context.Response.Body);
		}
	}
}
