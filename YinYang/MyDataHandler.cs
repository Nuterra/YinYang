using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace YinYang
{
	internal sealed class MyDataHandler : RequestHandler
	{
		public Task HandleRequest(HttpListenerContext context)
		{
			switch(context.Request.HttpMethod)
			{
				case "GET":
					return Get(context);
				case "POST":
					return Post(context);
			}
			return Task.CompletedTask;
		}

		private Task Get(HttpListenerContext context)
		{
			using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
			{
				return writer.WriteAsync("git gud");
			}
		}

		private Task Post(HttpListenerContext context)
		{
			using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
			{
				return writer.WriteAsync("post malone");
			}
		}
	}
}