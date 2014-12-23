using Owin;
using System.Web.Http;

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

			appBuilder.UseWebApi (config); 
		}
	}
}
