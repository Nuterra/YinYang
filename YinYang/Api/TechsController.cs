using System;
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
			var techs = await community.Techs.ToListAsync();

			return techs.Select(TechToJson);
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
			});
			return await results.ToListAsync();
		}

		private object TechToJson(TechUpload tech)
		{
			var json = JObject.FromObject(tech, _serializer);
			json.Add("creator", tech.Owner.Username);
			return json;
		}
	}
}