using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YinYang
{
	public sealed class Server
	{
		private HttpListener listener = new HttpListener();
		private Dictionary<HttpRoute, RequestHandler> routing = new Dictionary<HttpRoute, RequestHandler>();

		public Server()
		{
		}

		public void Start()
		{
			listener.Start();
			listener.GetContextAsync().ContinueWith(DispatchRequest);
			listener.Realm = "http://localhost/realm";
			listener.Prefixes.Add("http://localhost/test/");
		}

		public void Run()
		{
			while(listener.IsListening)
			{
				var contextTask = listener.GetContextAsync();
				contextTask.ContinueWith(DispatchRequest);
			}
		}

		public void Stop()
		{
			listener.Stop();
		}

		public void AddRoute(HttpRoute route, RequestHandler handler)
		{
			if (route == null) throw new ArgumentNullException(nameof(route));
			if (handler == null) throw new ArgumentNullException(nameof(route));

			routing.Add(route, handler);
		}

		private void DispatchRequest(Task<HttpListenerContext> requestTask)
		{
			var context = requestTask.Result;
			foreach (KeyValuePair<HttpRoute, RequestHandler> route in routing)
			{
				if (route.Key.CanAccept(context.Request))
				{
					route.Value.HandleRequest(context);
					return;
				}
			}
			Console.WriteLine($"Request: {context.Request.RawUrl}");
			new StaticFileHandler().HandleRequest(context);
		}
	}

}