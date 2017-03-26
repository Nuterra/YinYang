using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
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

		public Task HandleRequestAsync(HttpRequest request)
		{
#warning Throw exception here; it crashes the server
			return _routing[request.Method](request);
		}

		private async Task Get(HttpRequest request)
		{
			var args = HttpUtility.ParseQueryString(request.Request.Url.Query);
			var id = long.Parse(args["id"]);

			var community = request.GetCommunity();
			var account = await community.Accounts.GetBySteamIDAsync(id);
			if (account != null)
			{
				await request.ResponseWriter.WriteLineAsync("Found the user");
				await request.ResponseWriter.WriteLineAsync(account.Username);
				await request.ResponseWriter.WriteLineAsync(account.SteamID.ToString());
				await request.ResponseWriter.WriteLineAsync(account.Flags.ToString());
			}
			else
			{
				request.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
		}

		private async Task Post(HttpRequest request)
		{
			request.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
		}

		private async Task Delete(HttpRequest request)
		{
			var args = HttpUtility.ParseQueryString(request.Request.Url.Query);
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