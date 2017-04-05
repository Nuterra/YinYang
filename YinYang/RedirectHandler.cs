using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang
{
	public sealed class RedirectHandler : RequestHandler
	{
		public string RedirectTarget { get; set; }

		public Task HandleRequestAsync(IOwinContext context)
		{
			context.Response.Redirect(RedirectTarget);
			return Task.FromResult<object>(null);
		}
	}
}