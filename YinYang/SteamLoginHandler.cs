using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using YinYang.Authentication;

namespace YinYang
{
	internal sealed class SteamLoginHandler : RequestHandler
	{
		public async Task HandleRequest(HttpListenerContext context)
		{
			var openid = new LightOpenID(context.Request.Url);
			openid.Realm = new Uri("http://localhost/");
			openid.ReturnUrl = new Uri(openid.Realm, "/login/landing");
			switch (context.Request.Url.AbsolutePath)
			{
				case "/login":
					string s = await openid.GetAuthUrl();
					context.Response.Redirect(s);
					break;

				case "/login/landing":
					bool valid = await openid.Validate();

					using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
					{
						if (valid)
						{
							await writer.WriteAsync("Login Accepted");
						}
						else
						{
							await writer.WriteAsync("Login Invalid");
						}
					}

					break;
			}
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