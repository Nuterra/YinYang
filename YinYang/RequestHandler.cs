using System;
using System.Net;
using System.Threading.Tasks;

namespace YinYang
{
	public interface RequestHandler
	{
		Task HandleRequestAsync(HttpRequest reqiest);
	}
}