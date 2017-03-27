using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang.Session
{
	internal sealed class SessionMiddleware : Middleware
	{
		private const string SessionCookieName = "YinYang.Session";

		private Dictionary<string, HttpSession> _sessions = new Dictionary<string, HttpSession>();

		public async Task HandleRequestAsync(IOwinContext context, RequestHandlerDelegate continuation)
		{
			var sessionGuid = context.Request.Cookies[SessionCookieName];

			if (sessionGuid == null)
			{
				await CreateNewSession(context);
			}
			else
			{
				HttpSession existingSession;
				if (_sessions.TryGetValue(sessionGuid, out existingSession))
				{
					existingSession.MarkUsed();
					context.SetSession(existingSession);
				}
				else
				{
					await CreateNewSession(context);
				}
			}

			await continuation(context);
		}

		private async Task CreateNewSession(IOwinContext context)
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
		}
	}
}