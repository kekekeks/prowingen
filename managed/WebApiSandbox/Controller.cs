using System;
using System.Web.Http;
using System.Collections.Generic;
using System.Net.Http;

namespace WebApiSandbox
{
	public class TestController: ApiController
	{
		[HttpGet, Route("")]
		public object HelloWorld()
		{
			return new HttpResponseMessage{ Content = new StringContent (Request.Method.ToString () + " " + Request.RequestUri.ToString ()) };
		}

	}
}

