using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace YinYang
{
	public sealed class HttpRequest
	{
		private HttpListenerContext _listenerContext;

		public HttpListenerRequest Request => _listenerContext.Request;
		public HttpListenerResponse Response => _listenerContext.Response;
		public Dictionary<string, object> AttachedContext { get; } = new Dictionary<string, object>();
		public StreamWriter ResponseWriter { get; }

		public HttpMethod Method { get; }

		public HttpRequest(HttpListenerContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			_listenerContext = context;

			ResponseWriter = new StreamWriter(Response.OutputStream);
			Method = new HttpMethod(Request.HttpMethod);
		}

		public async Task Dispose()
		{
			await ResponseWriter.FlushAsync();
			await Response.OutputStream.FlushAsync();
			Response.OutputStream.Close();
		}
	}
}