using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using YinYang.Authentication;
using YinYang.Community;
using YinYang.Session;

namespace YinYang.Steam
{
	internal sealed class SteamLoginHandler : RequestHandler
	{
		private const string SteamClaimNamespace = "http://steamcommunity.com/openid/id/";
		private const string FailureMessage = "Invalid Identity";

		public async Task HandleRequestAsync(IOwinContext request)
		{
			LightOpenID openID = InitializeOpenID(request);
			switch (request.Request.Uri.AbsolutePath)
			{
				case "/login":
					await AuthenticationRedirect(request, openID);
					break;

				case "/login/landing":
					await HandleLanding(request, openID);
					break;
			}
		}

		private static async Task AuthenticationRedirect(IOwinContext request, LightOpenID openID)
		{
			string s = await openID.GetAuthUrl("http://steamcommunity.com/openid/");
			request.Response.Redirect(s);
		}

		private static LightOpenID InitializeOpenID(IOwinContext request)
		{
			var openID = new LightOpenID(request.Request)
			{
				Realm = new Uri(request.Request.Uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped))
			};
			openID.ReturnUrl = new Uri(openID.Realm, "/login/landing");
			return openID;
		}

		private async Task HandleLanding(IOwinContext request, LightOpenID openID)
		{
			bool valid = await openID.Validate();

			if (!valid)
			{
				request.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await request.Response.WriteAsync(FailureMessage);
				return;
			}

			if (openID.ClaimedID == null || !openID.ClaimedID.StartsWith(SteamClaimNamespace))
			{
				request.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await request.Response.WriteAsync(FailureMessage);
				return;
			}

			long steamID64;
			if (!long.TryParse(openID.ClaimedID.Substring(SteamClaimNamespace.Length), out steamID64))
			{
				request.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await request.Response.WriteAsync(FailureMessage);
				return;
			}

			var steamID = new SteamID(steamID64);
			if (!steamID.IsValid())
			{
				request.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await request.Response.WriteAsync(FailureMessage);
				return;
			}

			await HandleValidLogin(request, steamID);
		}

		private async Task HandleValidLogin(IOwinContext request, SteamID steamID)
		{
			var session = request.GetSession();
			session.SteamID = steamID;

			var community = request.GetCommunity();
			long steamID64 = steamID.ToSteamID64();
			Account account = await community.Accounts.GetBySteamIDAsync(steamID64);

			if (account == null)
			{
				Console.WriteLine($"Creating account for: {steamID64}");
				account = community.Accounts.Create();
				account.SteamID = steamID64;
				account.Flags = AccountFlags.None;

				community.Accounts.Add(account);
				await community.SaveChangesAsync();
			}

			request.Response.Redirect(request.Request.Uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));
		}
	}
}