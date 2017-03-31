using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang
{
	public interface Middleware
	{
		Task HandleRequestAsync(IOwinContext context, RequestHandlerDelegate continuation);
	}
}