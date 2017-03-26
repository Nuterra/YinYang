using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using YinYang.Community;
using YinYang.Session;
using YinYang.Steam;

namespace YinYang
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var hosts = ConfigurationManager.AppSettings.GetValues("YinYang.Listener.Prefix");
			if (hosts == null)
			{
				Console.WriteLine("No hosts specified in the .config file, startup cancelled");
				return;
			}

			var server = SetupServer(hosts);
			RunServer(server);
		}

		private static Server SetupServer(string[] hosts)
		{
			Server server = new Server();
			foreach (string prefix in hosts)
			{
				server.Listener.Prefixes.Add(prefix);
			}

			server.AddRoute(new HttpRoute("/login", HttpMethod.Get, HttpMethod.Post), new SteamLoginHandler());
			server.AddRoute(new HttpRoute("/api/account", HttpMethod.Get, HttpMethod.Post), new Api.AccountCommands());
			server.AddRoute(new HttpRoute("/wait", HttpMethod.Get), new WaitHandler());
			server.AddRoute(new HttpRoute("/", HttpMethod.Get), new StaticFileHandler() { RootDirectory = "app" });

			server.AddMiddleware(new CommunityMiddleware());
			server.AddMiddleware(new SessionMiddleware());

			return server;
		}

		private static void RunServer(Server server)
		{
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