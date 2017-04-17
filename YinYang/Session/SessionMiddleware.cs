using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang.Session
{
	internal sealed class SessionMiddleware : OwinMiddleware
	{
		private const string SessionCookieName = "YinYang.Session";

		private Dictionary<string, HttpSession> _sessions = new Dictionary<string, HttpSession>();

		public SessionMiddleware(OwinMiddleware next) : base(next)
		{
		}

		public override async Task Invoke(IOwinContext context)
		{
			var sessionGuid = context.Request.Cookies[SessionCookieName];

			if (sessionGuid == null)
			{
				await CreateNewSession(context);
			}
			else
			{
				HttpSession existingSession;
				if (_sessions.TryGetValue(sessionGuid, out existingSession) && existingSession.IsValid())
				{
					existingSession.MarkUsed();
					context.SetSession(existingSession);
				}
				else
				{
					await CreateNewSession(context);
				}
			}

			await Next.Invoke(context);
		}

		private Task CreateNewSession(IOwinContext context)
		{
			HttpSession newSession = new HttpSession(TimeSpan.FromHours(1));
			string sessionGuid = Guid.NewGuid().ToString();
			context.Response.Cookies.Append(SessionCookieName, sessionGuid, new CookieOptions
			{
				HttpOnly = true,
				Expires = newSession.Expires
			});
			_sessions[sessionGuid] = newSession;
			context.SetSession(newSession);
			return Task.FromResult<object>(null);
		}
	}
}