﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using YinYang.Authentication;

namespace YinYang.Steam
{
	internal sealed class SteamLoginHandler : RequestHandler
	{
		private const string SteamClaimNamespace = "http://steamcommunity.com/openid/id/";
		private const string FailureMessage = "Invalid Identity";

		public async Task HandleRequest(HttpListenerContext context)
		{
			LightOpenID openID = InitializeOpenID(context);
			switch (context.Request.Url.AbsolutePath)
			{
				case "/login":
					await AuthenticationRedirect(context, openID);
					break;

				case "/login/landing":
					await HandleLanding(context, openID);
					break;
			}
		}

		private static async Task AuthenticationRedirect(HttpListenerContext context, LightOpenID openID)
		{
			string s = await openID.GetAuthUrl("http://steamcommunity.com/openid/");
			context.Response.Redirect(s);
		}

		private static LightOpenID InitializeOpenID(HttpListenerContext context)
		{
			var openID = new LightOpenID(context.Request.Url);
			openID.Realm = new Uri(context.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped));
			openID.ReturnUrl = new Uri(openID.Realm, "/login/landing");
			return openID;
		}

		private async Task HandleLanding(HttpListenerContext context, LightOpenID openID)
		{
			using (StreamWriter content = new StreamWriter(context.Response.OutputStream))
			{
				bool valid = await openID.Validate();

				if (!valid)
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					await content.WriteAsync(FailureMessage);
					return;
				}

				if (openID.ClaimedID == null || !openID.ClaimedID.StartsWith(SteamClaimNamespace))
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					await content.WriteAsync(FailureMessage);
					return;
				}

				long steamID64;
				if (!long.TryParse(openID.ClaimedID.Substring(SteamClaimNamespace.Length), out steamID64))
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					await content.WriteAsync(FailureMessage);
					return;
				}

				var steamID = new SteamID(steamID64);

				if (!steamID.IsValid())
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					await content.WriteAsync(FailureMessage);
					return;
				}

				await content.WriteLineAsync($"Login accepted");
				await content.WriteLineAsync($"Universe: {steamID.Universe}");
				await content.WriteLineAsync($"Instance: {steamID.Instance}");
				await content.WriteLineAsync($"Type: {steamID.Type}");
				await content.WriteLineAsync($"AccountID: {steamID.AccountID}");
			}
		}
	}
}