using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace YinYang
{
	internal class WaitHandler : RequestHandler
	{
		public async Task HandleRequestAsync(IOwinContext context)
		{
			int threadCount = Process.GetCurrentProcess().Threads.Count;
			Console.WriteLine($"Threads: {threadCount}");
			await Task.Delay(TimeSpan.FromSeconds(10));
		}
	}
}