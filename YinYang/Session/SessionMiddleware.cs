using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace YinYang.Session
{
	internal sealed class SessionMiddleware : Middleware
	{
		private const string SessionCookieName = "YinYang.Session";

		private Dictionary<string, HttpSession> _sessions = new Dictionary<string, HttpSession>();

		public async Task HandleRequestAsync(HttpRequest request, RequestHandlerDelegate continuation)
		{
			var sessionGuid = request.Request.Cookies[SessionCookieName]?.Value;

			if (sessionGuid == null)
			{
				await CreateNewSession(request);
			}
			else
			{
				HttpSession existingSession;
				if (_sessions.TryGetValue(sessionGuid, out existingSession))
				{
					existingSession.MarkUsed();
					request.SetSession(existingSession);
				}
				else
				{
					await CreateNewSession(request);
				}
			}

			await continuation(request);
		}

		private async Task CreateNewSession(HttpRequest request)
		{
			string sessionGuid = Guid.NewGuid().ToString();
			Cookie sessionCookie = new Cookie(SessionCookieName, sessionGuid)
			{
				HttpOnly = true,
			};
			request.Response.SetCookie(sessionCookie);
			HttpSession newSession = new HttpSession(TimeSpan.FromHours(1));
			_sessions[sessionGuid] = newSession;
			request.SetSession(newSession);
		}
	}
}