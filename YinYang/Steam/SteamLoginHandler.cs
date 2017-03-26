using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using YinYang.Authentication;
using YinYang.Community;
using YinYang.Session;

namespace YinYang.Steam
{
	internal sealed class SteamLoginHandler : RequestHandler
	{
		private const string SteamClaimNamespace = "http://steamcommunity.com/openid/id/";
		private const string FailureMessage = "Invalid Identity";

		public async Task HandleRequestAsync(HttpRequest request)
		{
			LightOpenID openID = InitializeOpenID(request);
			switch (request.Request.Url.AbsolutePath)
			{
				case "/login":
					await AuthenticationRedirect(request, openID);
					break;

				case "/login/landing":
					await HandleLanding(request, openID);
					break;
			}
		}

		private static async Task AuthenticationRedirect(HttpRequest request, LightOpenID openID)
		{
			string s = await openID.GetAuthUrl("http://steamcommunity.com/openid/");
			request.Response.Redirect(s);
		}

		private static LightOpenID InitializeOpenID(HttpRequest request)
		{
			var openID = new LightOpenID(request.Request.Url);
			openID.Realm = new Uri(request.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped));
			openID.ReturnUrl = new Uri(openID.Realm, "/login/landing");
			return openID;
		}

		private async Task HandleLanding(HttpRequest request, LightOpenID openID)
		{
			using (StreamWriter content = new StreamWriter(request.Response.OutputStream))
			{
				bool valid = await openID.Validate();

				if (!valid)
				{
					request.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					await content.WriteAsync(FailureMessage);
					return;
				}

				if (openID.ClaimedID == null || !openID.ClaimedID.StartsWith(SteamClaimNamespace))
				{
					request.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					await content.WriteAsync(FailureMessage);
					return;
				}

				long steamID64;
				if (!long.TryParse(openID.ClaimedID.Substring(SteamClaimNamespace.Length), out steamID64))
				{
					request.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					await content.WriteAsync(FailureMessage);
					return;
				}

				var steamID = new SteamID(steamID64);
				if (!steamID.IsValid())
				{
					request.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					await content.WriteAsync(FailureMessage);
					return;
				}

				await HandleValidLogin(request, content, steamID);
			}
		}

		private async Task HandleValidLogin(HttpRequest request, StreamWriter content, SteamID steamID)
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

				session.UserAccount = account;
			}
			else
			{
				Console.WriteLine($"Deleting account for: {steamID64}");
				community.Accounts.Remove(account);
				await community.SaveChangesAsync();
			}

			request.Response.Redirect(request.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));
		}
	}
}