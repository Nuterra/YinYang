using System;
using Microsoft.Owin;

namespace YinYang.Session
{
	public static class SessionExtensions
	{
		internal const string AttachedContextKey = "YinYang.Session";

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