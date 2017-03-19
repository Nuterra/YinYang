using System;
using System.Linq;
using System.Net;

namespace YinYang
{
	public class HttpRoute
	{
		public string Prefix { get; }
		public HttpMethod[] Methods { get; }

		public HttpRoute(string pattern, params HttpMethod[] methods)
		{
			if (pattern == null) throw new ArgumentNullException(nameof(pattern));
			Prefix = pattern;
			Methods = methods ?? new HttpMethod[0];
		}

		public bool CanAccept(HttpListenerRequest request)
		{
			return Methods.Contains(request.HttpMethod) && request.Url.AbsolutePath.StartsWith(Prefix);
		}
	}
}