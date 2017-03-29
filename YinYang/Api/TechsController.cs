using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YinYang.Community;

namespace YinYang.Api
{
	public sealed class TechController : RequestHandler
	{
		private Dictionary<HttpMethod, Func<IOwinContext, Task<object>>> _routing;

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
				var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
				string json = JsonConvert.SerializeObject(response, settings);
				await context.Response.WriteAsync(json);
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
			return techs;
		}

		private async Task<object> GetSpecific(IOwinContext context, long id)
		{
			var community = context.GetCommunity();
			var techs = await community.Techs.Where(tech => tech.ID == id || tech.OwnerID == id).ToListAsync();
			return techs;
		}

	}
}
