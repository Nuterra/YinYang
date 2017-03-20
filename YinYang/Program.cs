using System;
using System.Threading;

namespace YinYang
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Server s = new Server();
			s.Listener.Prefixes.Add("http://localhost/");
			s.Listener.Prefixes.Add("http://127.0.0.1/");

			s.AddRoute(new HttpRoute("/api/mydata", HttpMethod.Get, HttpMethod.Post), new MyDataHandler());
			s.AddRoute(new HttpRoute("/wait", HttpMethod.Get), new WaitHandler());
			s.AddRoute(new HttpRoute("/", HttpMethod.Get), new StaticFileHandler());

			CancellationTokenSource tokenSource = new CancellationTokenSource();
			CancellationToken token = tokenSource.Token;
			s.Listener.Start();
			s.RunListenLoop(token);
			Console.WriteLine("Server Started, press enter to shutdown");
			Console.ReadLine();
			tokenSource.Cancel();
			s.Stop();
		}
	}
}