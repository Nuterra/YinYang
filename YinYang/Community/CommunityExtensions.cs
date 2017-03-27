﻿using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using YinYang.Steam;

namespace YinYang.Community
{
	public static class CommunityExtensions
	{
		internal const string AttachedContextKey = "YinYang.Community";

		public static IAppBuilder UseCommunity(this IAppBuilder app)
		{
			if (app == null) throw new ArgumentNullException(nameof(app));
			return app.Use(typeof(CommunityMiddleware));
		}

		public static CommunityContext GetCommunity(this IOwinContext request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			return request.Get<CommunityContext>(AttachedContextKey);
		}

		internal static void SetCommunity(this IOwinContext request, CommunityContext community)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			request.Set(AttachedContextKey, community);
		}

		public static Task<Account> GetBySteamIDAsync(this DbSet<Account> accounts, SteamID steamID)
		{
			if (steamID == null) throw new ArgumentNullException(nameof(steamID));
			return accounts.GetBySteamIDAsync(steamID.ToSteamID64());
		}

		public static Task<Account> GetBySteamIDAsync(this DbSet<Account> accounts, long steamID64)
		{
			if (accounts == null) throw new ArgumentNullException(nameof(accounts));
			return accounts.SingleOrDefaultAsync(acc => acc.SteamID == steamID64);
		}
	}
}