using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YinYang.Community;
using YinYang.Session;

namespace YinYang.Api
{
	public sealed class TechController : RequestHandler
	{
		private Dictionary<HttpMethod, Func<IOwinContext, Task<object>>> _routing;
		private JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

		public TechController()
		{
			_routing = new Dictionary<HttpMethod, Func<IOwinContext, Task<object>>>();
			_routing.Add(HttpMethod.Get, Get);
			_routing.Add(HttpMethod.Post, Post);
		}


		private async Task<object> Post(IOwinContext context)
		{
			string contentType = context.Request.ContentType;
			if (!contentType.StartsWith("multipart/form-data;"))
			{
				return "error: not multipart form content type";
			}
			FormValue titleValue;
			FormValue fileValue;
			var form = await MultipartFormParser.ParseMultipartForm(contentType, context.Request.Body);
			if (!form.TryGetValue("title", out titleValue))
			{
				return "error: missing title";
			}
			if (!form.TryGetValue("file", out fileValue))
			{
				return "error: missing file";
			}

			string title = titleValue.StringValue;
			FileUpload file = fileValue.FilesValue.Single();
			string finalPath = Path.Combine("images", file.FileName);
			Directory.CreateDirectory("images");
			File.WriteAllBytes(finalPath, file.Contents);

			Tech newTech = context.GetCommunity().Techs.Create();
			newTech.OwnerID = context.GetSession().SteamID.ToSteamID64();
			newTech.Title = title;
			newTech.ImageUrl = "/images/" + finalPath.Replace('\\', '/');
			context.GetCommunity().Techs.Add(newTech);
			await context.GetCommunity().SaveChangesAsync();

			return null;

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
				using (StreamWriter writer = new StreamWriter(context.Response.Body))
				{
					_serializer.Serialize(writer, response);
				}
			}
		}

		private Task<object> Get(IOwinContext context)
		{
			var path = context.Request.Path.Value.Substring(1);

			if (path.Equals("all", StringComparison.OrdinalIgnoreCase))
			{
				// api/techs/all
				return GetAll(context);
			}
			else
			{
				// api/techs/{id}
				return GetSpecific(context, long.Parse(path));
			}
		}

		private async Task<object> GetAll(IOwinContext context)
		{
			var community = context.GetCommunity();
			var techs = community.Techs;

			var session = context.GetSession();
			if (session.SteamID != null)
			{
				var accountID = session.SteamID.ToSteamID64();
				var results = techs
					.OrderByDescending(t => t.CreationTime)
					.Select(t => new
					{
						Creator = t.Owner.Username,
						CreatorID = t.OwnerID,
						Title = t.Title,
						ImageUrl = t.ImageUrl,
						Featured = t.Featured,
						CreationTime = t.CreationTime,
						Subscriptions = community.Accounts.Count(acc => acc.SubscribedTechs.Contains(t)),
						Subscribed = t.Subscribers.Any(acc => acc.SteamID == accountID),
					});
				return await results.ToListAsync();
			}
			else
			{
				var results = techs
					.OrderByDescending(t => t.CreationTime)
					.Select(t => new
					{
						Creator = t.Owner.Username,
						CreatorID = t.OwnerID,
						Title = t.Title,
						ImageUrl = t.ImageUrl,
						Featured = t.Featured,
						CreationTime = t.CreationTime,
						Subscriptions = community.Accounts.Count(acc => acc.SubscribedTechs.Contains(t)),
						Subscribed = false,
					});
				return await results.ToListAsync();
			}
		}

		private async Task<object> GetSpecific(IOwinContext context, long id)
		{
			var community = context.GetCommunity();
			var techs = community.Techs.Where(tech => tech.ID == id || tech.OwnerID == id);
			var results = techs.Select(t => new
			{
				Creator = t.Owner.Username,
				CreatorID = t.OwnerID,
				Title = t.Title,
				ImageUrl = t.ImageUrl,
				Views = community.Accounts.Count(acc => acc.SubscribedTechs.Contains(t)),
				Downloads = 18,
			});
			return await results.ToListAsync();
		}
	}
}