using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using YinYang.Community;
using YinYang.Session;
using YinYang.Steam;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin;
using System.Threading.Tasks;

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

			string host = hosts.First();
			using (WebApp.Start(host, SetupServer))
			{
				Console.WriteLine($"Host: {host}");
				Console.WriteLine("Server Started, press enter to shutdown");
				Console.ReadLine();
			}
		}

		private static void SetupServer(IAppBuilder app)
		{
			var server = new Server();

			server.AddRoute(new HttpRoute("/login", HttpMethod.Get, HttpMethod.Post), new SteamLoginHandler());
			server.AddRoute(new HttpRoute("/api/account", HttpMethod.Get, HttpMethod.Post), new Api.AccountCommands());
			server.AddRoute(new HttpRoute("/wait", HttpMethod.Get), new WaitHandler());
			server.AddRoute(new HttpRoute("/", HttpMethod.Get), new StaticFileHandler() { RootDirectory = "app" });

			server.AddMiddleware(new CommunityMiddleware());
			server.AddMiddleware(new SessionMiddleware());

			app.Run(context => server.RequestHandler(context));
		}
	}
}