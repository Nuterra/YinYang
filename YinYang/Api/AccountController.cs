﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using YinYang.Community;
using YinYang.Session;
using YinYang.Steam;

namespace YinYang.Api
{
	public sealed class AccountController : RequestHandler
	{
		private Dictionary<HttpMethod, Func<IOwinContext, Task<object>>> _routing;
		private static readonly object AccountNotFound = new { Error = "Account not found" };

		public AccountController()
		{
			_routing = new Dictionary<HttpMethod, Func<IOwinContext, Task<object>>>();
			_routing.Add(HttpMethod.Get, Get);
			_routing.Add(HttpMethod.Put, Put);
			_routing.Add(HttpMethod.Delete, Delete);
		}

		public async Task HandleRequestAsync(IOwinContext context)
		{
			var method = HttpMethod.Parse(context.Request.Method);
			Func<IOwinContext, Task<object>> handler;
			if (!_routing.TryGetValue(method, out handler))
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				return;
			}
			var response = await handler(context);

			if (response != null)
			{
				var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
				string json = JsonConvert.SerializeObject(response, settings);
				await context.Response.WriteAsync(json);
			}
		}

		private async Task<object> Get(IOwinContext context)
		{
			if (context.Request.Path.HasValue)
			{
				// api/accounts/{id}
				// api/accounts/me
				string id = context.Request.Path.Value.Substring(1);
				object response;
				if (id.Equals("me", StringComparison.OrdinalIgnoreCase))
				{
					response = await GetCurrentAccount(context);
				}
				else
				{
					response = await GetSpecific(context, long.Parse(id));
				}
				if (response == null)
				{
					context.Response.StatusCode = (int)HttpStatusCode.NotFound;
					return AccountNotFound;
				}
				return response;
			}
			else
			{
				// api/accounts?skip=27&take=50
				return await GetAll(context);
			}
		}

		private async Task<object> GetAll(IOwinContext context)
		{
			int skip = 0;
			var skipParam = context.Request.Query.GetValues("skip")?.Single();
			if (skipParam != null)
			{
				skip = int.Parse(skipParam);
			}

			int take = 20;
			var takeParam = context.Request.Query.GetValues("take")?.Single();
			if (takeParam != null)
			{
				take = int.Parse(takeParam);
			}

			var community = context.GetCommunity();
			var accounts = await community.Accounts
				.Where(acc => (acc.Flags & AccountFlags.Activated) > 0)
				.OrderBy(acc => acc.Username)
				.Skip(skip)
				.Take(take)
				.ToListAsync();

			return accounts;
		}

		private async Task<object> GetCurrentAccount(IOwinContext context)
		{
			var session = context.GetSession();
			if (session.SteamID == null)
			{
				return null;
			}
			return await GetSpecific(context, session.SteamID.ToSteamID64());
		}

		private async Task<object> GetSpecific(IOwinContext context, long id)
		{
			var community = context.GetCommunity();
			var account = await community.Accounts.GetBySteamIDAsync(id);
			return account;
		}

		private async Task<object> Put(IOwinContext context)
		{
			// api/account/{id}
			var session = context.GetSession();
			var community = context.GetCommunity();
			Account target = await GetTargetAccount(context.Request.Path.Value, session, community);
			Account authorizer = await community.Accounts.GetBySteamIDAsync(session.SteamID);
			if (!IsChangeAuthorized(target, authorizer))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				return false;
			}

			var form = await context.Request.ReadFormAsync();

			string fieldname = form["name"];
			string value = form["value"];
			switch (fieldname)
			{
				case "username":
					if (string.IsNullOrWhiteSpace(value))
					{
						context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
						return "username is null or empty";
					}
					target.Username = value;
					await community.SaveChangesAsync();
					break;
			}
			return true;
		}

		private async Task<object> Delete(IOwinContext context)
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
				return false;
			}

			target.Flags &= ~(AccountFlags.Activated);
			await community.SaveChangesAsync();
			return true;
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