using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
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
			object titleValue;
			object fileValue;
			var form = await MultipartFormParserasdf.ParseMultipartForm(contentType, context.Request.Body);
			if (!form.TryGetValue("title", out titleValue) || !(titleValue is string))
			{
				return "error: missing title";
			}
			if (!form.TryGetValue("file", out fileValue) || !(fileValue is FileUpload))
			{
				return "error: missing file";
			}

			var session = context.GetSession();
			var community = context.GetCommunity();
			Account user = await community.Accounts.GetBySessionAsync(session);

			if (user == null)
			{
				throw new InvalidOperationException("not logged in");
			}

			string title = (string)titleValue;
			FileUpload file = (FileUpload)fileValue;

			if (ConfigurationManager.AppSettings["Imgur.ClientID"] == null)
			{
				throw new InvalidOperationException("Imgur.API not configured");
			}

			var client = new ImgurClient(ConfigurationManager.AppSettings["Imgur.ClientID"], ConfigurationManager.AppSettings["Imgur.ClientSecret"]);
			var endpoint = new ImageEndpoint(client);
			IImage image = await endpoint.UploadImageBinaryAsync(file.Contents);

			Tech newTech = community.Techs.Create();
			newTech.Owner = user;
			newTech.Title = title;
			newTech.ImageUrl = image.Link;
			community.Techs.Add(newTech);
			await community.SaveChangesAsync();

			return newTech.ID;
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
			var techs = community.Techs.OrderByDescending(t => t.CreationTime);
			var transformed = await ApplyResponseTransform(context, techs);
			return await transformed.ToListAsync();
		}

		private async Task<object> GetSpecific(IOwinContext context, long id)
		{
			var community = context.GetCommunity();
			var techs = community.Techs.Where(tech => tech.ID == id || tech.OwnerID == id);
			var transformed = await ApplyResponseTransform(context, techs);
			return await transformed.ToListAsync();
		}

		private async Task<IQueryable> ApplyResponseTransform(IOwinContext context, IQueryable<Tech> query)
		{
			var session = context.GetSession();
			var community = context.GetCommunity();
			if (session.IsValidLogin())
			{
				var accountID = session.SteamID.ToSteamID64();
				return query.Select(t => new
				{
					ID = t.ID,
					Creator = t.Owner.Username,
					CreatorID = t.OwnerID,
					Title = t.Title,
					ImageUrl = t.ImageUrl,
					Featured = t.Featured,
					CreationTime = t.CreationTime,
					Subscriptions = community.Accounts.Count(acc => acc.SubscribedTechs.Contains(t)),
					Subscribed = t.Subscribers.Any(acc => acc.SteamID == accountID),
				});
			}
			else
			{
				return query.Select(t => new
				{
					ID = t.ID,
					Creator = t.Owner.Username,
					CreatorID = t.OwnerID,
					Title = t.Title,
					ImageUrl = t.ImageUrl,
					Featured = t.Featured,
					CreationTime = t.CreationTime,
					Subscriptions = community.Accounts.Count(acc => acc.SubscribedTechs.Contains(t)),
					Subscribed = false,
				});
			}
		}

	}
}