using System;
using System.Configuration;
using System.Threading;

namespace YinYang
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Server server = new Server();

			var hosts = ConfigurationManager.AppSettings.GetValues("YinYang.Listener.Prefix");

			if (hosts == null)
			{
				Console.WriteLine("No hosts specified in the .config file, startup cancelled");
				return;
			}

			foreach (string prefix in hosts)
			{
				server.Listener.Prefixes.Add(prefix);
			}

			server.AddRoute(new HttpRoute("/login", HttpMethod.Get, HttpMethod.Post), new SteamLoginHandler());
			server.AddRoute(new HttpRoute("/wait", HttpMethod.Get), new WaitHandler());
			server.AddRoute(new HttpRoute("/", HttpMethod.Get), new StaticFileHandler() { RootDirectory = "app" });

			CancellationTokenSource tokenSource = new CancellationTokenSource();
			CancellationToken token = tokenSource.Token;
			server.Listener.Start();

			server.RunListenLoop(token);

			Console.WriteLine("Server Started, press enter to shutdown");
			Console.ReadLine();
			tokenSource.Cancel();
			server.Stop();
		}
	}
}