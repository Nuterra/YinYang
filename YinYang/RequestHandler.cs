using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang
{
	public interface RequestHandler
	{
		Task HandleRequestAsync(IOwinContext context);
	}
}