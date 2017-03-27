using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YinYang.Community;
using YinYang.Session;
using YinYang.Steam;

namespace YinYang.Api
{
	public sealed class AccountCommands : RequestHandler
	{
		private Dictionary<HttpMethod, RequestHandlerDelegate> _routing;

		public AccountCommands()
		{
			_routing = new Dictionary<HttpMethod, RequestHandlerDelegate>();
			_routing.Add(HttpMethod.Get, Get);
			_routing.Add(HttpMethod.Put, Put);
			_routing.Add(HttpMethod.Delete, Delete);
		}

		public async Task HandleRequestAsync(IOwinContext context)
		{
			var method = HttpMethod.Parse(context.Request.Method);
			RequestHandlerDelegate handler;
			if (!_routing.TryGetValue(method, out handler))
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				return;
			}
			await handler(context);
		}

		private Task Get(IOwinContext context)
		{
			var path = context.Request.Path.Value.Substring(1);

			if (path.Equals("all", StringComparison.OrdinalIgnoreCase))
			{
				// api/account/all
				return GetAll(context);
			}
			else
			{
				// api/account/{id}
				return GetSpecific(context, long.Parse(path));
			}
		}

		private async Task GetAll(IOwinContext context)
		{
			var community = context.GetCommunity();
			var ids = await community.Accounts.Where(acc => (acc.Flags & AccountFlags.Activated) > 0).Select(acc => acc.SteamID.ToString()).ToListAsync();
			string json = JsonConvert.SerializeObject(ids);
			await context.Response.WriteAsync(json);

		}

		private async Task GetSpecific(IOwinContext context, long id)
		{
			var community = context.GetCommunity();
			var account = await community.Accounts.GetBySteamIDAsync(id);
			if (account != null)
			{
				string accountJson = JsonConvert.SerializeObject(account);
				await context.Response.WriteAsync(accountJson);
			}
			else
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
		}

		private async Task Put(IOwinContext context)
		{
			// api/account/{id}
			var session = context.GetSession();
			var community = context.GetCommunity();
			Account target = await GetTargetAccount(context.Request.Path.Value, session, community);
			Account authorizer = await community.Accounts.GetBySteamIDAsync(session.SteamID);
			if (!IsChangeAuthorized(target, authorizer))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				return;
			}

			using (StreamReader reader = new StreamReader(context.Request.Body))
			{
				var jsonBody = await reader.ReadToEndAsync();
				JObject obj = JObject.Parse(jsonBody);
				string newUsername = obj.GetValue("username").ToString();
				target.Username = newUsername;
				await community.SaveChangesAsync();
			}
		}

		private async Task Delete(IOwinContext context)
		{
			// api/account/{id}
			var args = context.Request.Query;
			var session = context.GetSession();
			var community = context.GetCommunity();

			Account target = await GetTargetAccount(context.Request.Path.Value, session, community);
			Account authorizer = await community.Accounts.GetBySteamIDAsync(session.SteamID);

			if (!IsChangeAuthorized(target, authorizer))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				return;
			}

			target.Flags &= ~(AccountFlags.Activated);
			await community.SaveChangesAsync();
		}

		private static async Task<Account> GetTargetAccount(string apiPath, HttpSession session, CommunityContext community)
		{
			var path = apiPath.Substring(1);
			var id = long.Parse(path);
			SteamID targetID;
			string targetIDString = path;
			if (targetIDString != null)
			{
				targetID = new SteamID(long.Parse(targetIDString));
			}
			else
			{
				targetID = session.SteamID;
			}
			Account target = await community.Accounts.GetBySteamIDAsync(targetID);
			return target;
		}

		private bool IsChangeAuthorized(Account target, Account authoritive)
		{
			return (target.SteamID == authoritive.SteamID || authoritive.Flags.HasFlag(AccountFlags.Admin));
		}
	}
}