using System;
using System.Data.Entity;
using System.Threading.Tasks;
using YinYang.Steam;

namespace YinYang.Community
{
	public static class CommunityExtensions
	{
		internal const string AttachedContextKey = "YinYang.Community";

		public static CommunityContext GetCommunity(this HttpRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			object result;
			if (request.AttachedContext.TryGetValue(AttachedContextKey, out result))
			{
				return (CommunityContext)result;
			}
			return null;
		}

		internal static void SetCommunity(this HttpRequest request, CommunityContext community)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			if (request.AttachedContext.ContainsKey(AttachedContextKey))
			{
				request.AttachedContext[AttachedContextKey] = community;
			}
			else
			{
				request.AttachedContext.Add(AttachedContextKey, community);
			}
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