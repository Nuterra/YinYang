using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
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
			_routing.Add(HttpMethod.Post, Post);
			_routing.Add(HttpMethod.Delete, Delete);
		}

		public Task HandleRequestAsync(IOwinContext request)
		{
			var method = HttpMethod.Parse(request.Request.Method);
			return _routing[method](request);
		}

		private async Task Get(IOwinContext request)
		{
			var args = request.Request.Query;
			var id = long.Parse(args["id"]);

			var community = request.GetCommunity();
			var account = await community.Accounts.GetBySteamIDAsync(id);
			if (account != null)
			{
				await request.Response.WriteAsync($"Found the user\n{account.Username}\n{account.SteamID.ToString()}\n{account.Flags.ToString()}\n");
			}
			else
			{
				request.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
		}

		private async Task Post(IOwinContext request)
		{
			request.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
		}

		private async Task Delete(IOwinContext request)
		{
			var args = request.Request.Query;
			var steamID64 = long.Parse(args["id"]);
			var id = new SteamID(steamID64);
			var community = request.GetCommunity();
			var session = request.GetSession();
			if (id == session.SteamID)
			{
				var account = await community.Accounts.GetBySteamIDAsync(steamID64);
				community.Accounts.Remove(account);
				await community.SaveChangesAsync();
			}
		}
	}
}