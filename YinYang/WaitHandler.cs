using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace YinYang
{
	internal class WaitHandler : RequestHandler
	{
		public async Task HandleRequestAsync(HttpRequest request)
		{
			int threadCount = Process.GetCurrentProcess().Threads.Count;
			Console.WriteLine($"Threads: {threadCount}");
			await Task.Delay(TimeSpan.FromSeconds(10));
		}
	}
}