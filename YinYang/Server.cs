using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace YinYang
{
	public sealed class Server
	{
		private HttpListener _listener = new HttpListener();
		private Dictionary<HttpRoute, RequestHandler> _routing = new Dictionary<HttpRoute, RequestHandler>();
		public int MaxConcurrentRequests { get; set; } = 10;
		public string BaseUrl { get; }

		public Server(string baseUrl)
		{
			if (!HttpListener.IsSupported)
			{
				throw new NotSupportedException("HttpListener is not supported");
			}
			BaseUrl = baseUrl;
		}

		public void AddRoute(HttpRoute route, RequestHandler handler)
		{
			if (route == null) throw new ArgumentNullException(nameof(route));
			if (handler == null) throw new ArgumentNullException(nameof(route));

			_routing.Add(route, handler);
		}

		public void Start()
		{
			if (_listener.IsListening) throw new InvalidOperationException("Server already started");
			_listener.Start();
			_listener.Prefixes.Add(BaseUrl);

			var c = TaskScheduler.Current;

			var l = Listen();
		}

		private async Task Listen()
		{
			var taskPool = new HashSet<Task>();
			for (int i = 0; i < MaxConcurrentRequests; i++)
				taskPool.Add(_listener.GetContextAsync());

			while (_listener.IsListening)
			{
				Console.WriteLine($"Waiting... {Thread.CurrentThread.ManagedThreadId}");
				Task task = await Task.WhenAny(taskPool);
				taskPool.Remove(task);

				Console.WriteLine($"Task completed {Thread.CurrentThread.ManagedThreadId}");

				if (task is Task<HttpListenerContext>)
				{
					var context = (task as Task<HttpListenerContext>).Result;
					taskPool.Add(_listener.GetContextAsync());
					taskPool.Add(Task.Factory.StartNew(HandleClient, context));
					//taskPool.Add(HandleClient(context));
				}
			}
		}

		private async Task HandleClient(object c)
		{
			HttpListenerContext context = (HttpListenerContext)c;
			try
			{
				Console.WriteLine($"Begin handle {context.GetHashCode()} {Thread.CurrentThread.ManagedThreadId}");
				Console.WriteLine($"Request: {context.Request.RawUrl}");

				foreach (KeyValuePair<HttpRoute, RequestHandler> route in _routing)
				{
					if (route.Key.CanAccept(context.Request))
					{
						await route.Value.HandleRequest(context);
						return;
					}
				}
			}
			finally
			{
				Console.WriteLine($"End handle {context.GetHashCode()} {Thread.CurrentThread.ManagedThreadId}");
				context.Response.OutputStream.Close();
			}
		}

		public void Stop()
		{
			_listener.Stop();
		}
	}
}