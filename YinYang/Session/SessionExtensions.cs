using System;

namespace YinYang.Session
{
	public static class SessionExtensions
	{
		internal const string AttachedContextKey = "YinYang.Session";

		public static HttpSession GetSession(this HttpRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			object result;
			if (request.AttachedContext.TryGetValue(AttachedContextKey, out result))
			{
				return (HttpSession)result;
			}
			return null;
		}

		internal static void SetSession(this HttpRequest request, HttpSession session)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			if (request.AttachedContext.ContainsKey(AttachedContextKey))
			{
				request.AttachedContext[AttachedContextKey] = session;
			}
			else
			{
				request.AttachedContext.Add(AttachedContextKey, session);
			}
		}
	}
}