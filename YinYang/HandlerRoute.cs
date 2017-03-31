using System;
using System.Linq;
using Microsoft.Owin;

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

		public bool CanAccept(IOwinRequest request)
		{
			var method = HttpMethod.Parse(request.Method);
			return Methods.Contains(method) && request.Uri.AbsolutePath.StartsWith(Prefix);
		}
	}
}