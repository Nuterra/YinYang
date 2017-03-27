using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang
{
	public delegate Task RequestHandlerDelegate(IOwinContext request);

	public delegate Task MiddlewareDelegate(IOwinContext request, RequestHandlerDelegate handler);

	public sealed class Server
	{
		public HttpListener Listener { get; } = new HttpListener();
		private Dictionary<HttpRoute, RequestHandler> _routing = new Dictionary<HttpRoute, RequestHandler>();

		public Server()
		{
		}

		public void AddRoute(HttpRoute route, RequestHandler handler)
		{
			if (route == null) throw new ArgumentNullException(nameof(route));
			if (handler == null) throw new ArgumentNullException(nameof(route));

			_routing.Add(route, handler);
		}

		public async Task HandleClient(IOwinContext request)
		{
			try
			{
				foreach (KeyValuePair<HttpRoute, RequestHandler> route in _routing)
				{
					if (route.Key.CanAccept(request.Request))
					{
						await route.Value.HandleRequestAsync(request);
						return;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine(ex.Message);
			}
		}

		public void Stop()
		{
			Listener.Stop();
		}
	}
}