using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YinYang
{
	public interface Middleware
	{
		Task HandleRequestAsync(HttpRequest request, RequestHandlerDelegate continuation);
	}
}
