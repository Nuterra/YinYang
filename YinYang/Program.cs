using System;
using System.Configuration;
using System.Linq;
using Microsoft.Owin.Hosting;
using Owin;
using YinYang.Api;
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

			app.Map("/app", ConfigureStaticFiles);

			app.UseSession();
			app.UseCommunity();

			app.Map("/login", ConfigureLogin);

			server.AddRoute(new HttpRoute("/", HttpMethod.Get), new StaticFileHandler() { RootDirectory = @"..\..\app" });

			app.Map("/api/account", ConfigureAccountApi);
			app.Map("/api/techs", ConfigureTechApi);

			app.Run(context => server.HandleClient(context));
		}

		private static void ConfigureStaticFiles(IAppBuilder app)
		{
			app.Run(new StaticFileHandler() { RootDirectory = @"..\..\app" }.HandleRequestAsync);
		}

		private static void ConfigureLogin(IAppBuilder app)
		{
			app.Run(new SteamLoginHandler().HandleRequestAsync);
		}

		private static void ConfigureAccountApi(IAppBuilder app)
		{
			app.Run(new AccountCommands().HandleRequestAsync);
		}

		private static void ConfigureTechApi(IAppBuilder app)
		{
			app.Run(new TechController().HandleRequestAsync);
		}
	}
}