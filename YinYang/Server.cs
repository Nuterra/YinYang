using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace YinYang
{
	public sealed class Server
	{
		public HttpListener Listener { get; } = new HttpListener();
		private Dictionary<HttpRoute, RequestHandler> _routing = new Dictionary<HttpRoute, RequestHandler>();

		public void AddRoute(HttpRoute route, RequestHandler handler)
		{
			if (route == null) throw new ArgumentNullException(nameof(route));
			if (handler == null) throw new ArgumentNullException(nameof(route));

			_routing.Add(route, handler);
		}

		public async Task RunListenLoop(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				var context = await Listener.GetContextAsync();
				HandleClient(context);
			}
		}

		private async Task HandleClient(HttpListenerContext context)
		{
			try
			{
				Console.WriteLine($"Request: {context.Request.Url}");
				foreach (KeyValuePair<HttpRoute, RequestHandler> route in _routing)
				{
					if (route.Key.CanAccept(context.Request))
					{
						Console.WriteLine($"Handler: {route.Value.GetType()}");
						await route.Value.HandleRequest(context);
						Console.WriteLine($"Handler done: {route.Value.GetType()}");
						break;
					}
				}
			}
			finally
			{
				Console.WriteLine($"Closing: {context.Request.Url}");
				context.Response.OutputStream.Close();
			}
		}

		public void Stop()
		{
			Listener.Stop();
		}
	}
}