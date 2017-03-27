using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang
{
	public interface RequestHandler
	{
		Task HandleRequestAsync(IOwinContext context);
	}
}