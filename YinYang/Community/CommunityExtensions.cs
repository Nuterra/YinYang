using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YinYang.Community
{
	public static class CommunityExtensions
	{
		internal const string AttachedContextKey = "YinYang.Community";

		public static CommunityContext GetCommunity(this HttpRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			object result;
			if (request.AttachedContext.TryGetValue(AttachedContextKey, out result))
			{
				return (CommunityContext)result;
			}
			return null;
		}

		internal static void SetCommunity(this HttpRequest request, CommunityContext session)
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
