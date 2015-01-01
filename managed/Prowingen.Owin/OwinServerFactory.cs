using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prowingen.Owin
{
	public static class OwinServerFactory
	{
		public static void Initialize(IDictionary<string, object> properties)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");
			properties["owin.Version"] = "1.0";
			properties ["opaque.Version"] = "1.0";
			properties ["websocket.Version"] = "1.0";
		}

		public static IDisposable Create(Func<IDictionary<string, object>, Task> app,
			IDictionary<string, object> properties)
		{
			if (app == null)
				throw new ArgumentNullException("app");
			if (properties == null)
				throw new ArgumentNullException("properties");

			return new Prowingen.Owin.ProwingenOwinServer (app, properties);
		}

		class Dummy : IDisposable
		{
			#region IDisposable implementation

			public void Dispose ()
			{
				throw new NotImplementedException ();
			}

			#endregion


		}
	}
}

