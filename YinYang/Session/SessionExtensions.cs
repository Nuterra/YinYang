using System;
using Microsoft.Owin;
using Owin;

namespace YinYang.Session
{
	public static class SessionExtensions
	{
		internal const string AttachedContextKey = "YinYang.Session";

		public static IAppBuilder UseSession(this IAppBuilder app)
		{
			if (app == null) throw new ArgumentNullException(nameof(app));
			return app.Use(typeof(SessionMiddleware));
		}

		public static HttpSession GetSession(this IOwinContext request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			return request.Get<HttpSession>(AttachedContextKey);
		}

		internal static void SetSession(this IOwinContext request, HttpSession session)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			request.Set(AttachedContextKey, session);
		}
	}
}