using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang
{
	public interface Middleware
	{
		Task HandleRequestAsync(IOwinContext context, RequestHandlerDelegate continuation);
	}
}
