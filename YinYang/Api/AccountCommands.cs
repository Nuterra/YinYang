using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YinYang.Api
{
	public sealed class AccountCommands : RequestHandler
	{
		private Dictionary<HttpMethod, RequestHandlerDelegate> _routing = new Dictionary<HttpMethod, RequestHandlerDelegate>();

		public Task HandleRequestAsync(HttpRequest request)
		{
			return _routing[request.Method](request);
		}

		private async Task Get(HttpRequest request)
		{
		}

		private async Task Post(HttpRequest request)
		{
		}
	}
}