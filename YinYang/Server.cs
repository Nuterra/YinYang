using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace YinYang
{
	public delegate Task RequestHandlerDelegate(HttpRequest request);

	public delegate Task MiddlewareDelegate(HttpRequest request, RequestHandlerDelegate handler);

	public sealed class Server
	{
		public HttpListener Listener { get; } = new HttpListener();
		private Dictionary<HttpRoute, RequestHandler> _routing = new Dictionary<HttpRoute, RequestHandler>();

		public RequestHandlerDelegate RequestHandler { get; set; }

		public Server()
		{
			RequestHandler = HandleClient;
		}

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
				Console.WriteLine($"Request: {context.Request.Url.AbsolutePath}");
				var request = new HttpRequest(context);
				RequestHandler(request);
			}
		}

		private async Task HandleClient(HttpRequest request)
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

				Environment.Exit(-1);
			}
			finally
			{
				await request.Dispose();
			}
		}

		public void Stop()
		{
			Listener.Stop();
		}
	}
}