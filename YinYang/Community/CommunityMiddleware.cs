using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YinYang.Session;

namespace YinYang.Community
{
	public sealed class CommunityMiddleware : Middleware
	{
		public async Task HandleRequestAsync(HttpRequest request, RequestHandlerDelegate continuation)
		{
			using (CommunityContext community = new CommunityContext("YinYang.Community"))
			{
				request.SetCommunity(community);

				var session = request.GetSession();

				if (session.SteamID == null)
				{
					Cookie steamIDCookie = new Cookie("YinYang.SteamID", "0");
					steamIDCookie.Expires = DateTime.Now + TimeSpan.FromMinutes(1);
					//request.Response.SetCookie(steamIDCookie);
				}
				else
				{
					Cookie steamIDCookie = new Cookie("YinYang.SteamID", session.SteamID.ToSteamID64().ToString());
					steamIDCookie.Expires = DateTime.Now + TimeSpan.FromMinutes(1);
					request.Response.SetCookie(steamIDCookie);
				}

				await continuation(request);
			}
		}
	}
}
