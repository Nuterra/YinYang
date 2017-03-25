using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YinYang.Community
{
	public sealed class CommunityMiddleware : Middleware
	{
		public async Task HandleRequestAsync(HttpRequest request, RequestHandlerDelegate continuation)
		{
			using (CommunityContext community = new CommunityContext("YinYang.Community"))
			{
				request.SetCommunity(community);
				await continuation(request);
			}
		}
	}
}
