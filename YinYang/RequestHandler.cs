using System;
using System.Net;
using System.Threading.Tasks;

namespace YinYang
{
	public interface RequestHandler
	{
		Task HandleRequest(HttpListenerContext context);
	}
}