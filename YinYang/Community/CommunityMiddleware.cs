using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using YinYang.Session;

namespace YinYang.Community
{
	public sealed class CommunityMiddleware : Middleware
	{
		public async Task HandleRequestAsync(IOwinContext context, RequestHandlerDelegate continuation)
		{
			using (CommunityContext community = new CommunityContext("YinYang.Community"))
			{
				context.SetCommunity(community);
				var session = context.GetSession();
				if (session.SteamID == null)
				{
					context.Response.Cookies.Delete("YinYang.SteamID");
				}
				else
				{
					context.Response.Cookies.Append("YinYang.SteamID", session.SteamID.ToSteamID64().ToString());
				}
				await continuation(context);
			}
		}
	}
}
