using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using YinYang.Session;

namespace YinYang.Community
{
	public sealed class CommunityMiddleware : OwinMiddleware
	{
		public CommunityMiddleware(OwinMiddleware next) : base(next)
		{
		}

		public override async Task Invoke(IOwinContext context)
		{
			using (CommunityContext community = new CommunityContext())
			{
				context.SetCommunity(community);
				var session = context.GetSession();
				if (session.SteamID == null)
				{
					context.Response.Cookies.Delete("YinYang.SteamID");
				}
				else
				{

					context.Response.Cookies.Append("YinYang.SteamID", session.SteamID.ToSteamID64().ToString(), new CookieOptions { Expires = DateTime.Now });
				}

				await Next.Invoke(context);
			}
		}
	}
}