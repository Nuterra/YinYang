using System;

namespace YinYang
{
	public static class ServerExtensions
	{
		public static void AddMiddleware(this Server server, MiddlewareDelegate middleware)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (middleware == null) throw new ArgumentNullException(nameof(middleware));

			var oldHandler = server.RequestHandler;
			server.RequestHandler = new RequestHandlerDelegate(request => middleware(request, oldHandler));
		}

		public static void AddMiddleware(this Server server, Middleware middleware)
		{
			server.AddMiddleware(middleware.HandleRequestAsync);
		}
	}
}