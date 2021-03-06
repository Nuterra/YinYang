﻿using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
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

			app.Use(RedirectAppRequests);
			app.Map("/assets", ConfigureStaticFiles);

			app.UseSession();
			app.UseCommunity();

			app.Map("/login", ConfigureLogin);

			server.AddRoute(new HttpRoute("/images", HttpMethod.Get), new StaticFileHandler() { RootDirectory = "./images/" });
			server.AddRoute(new HttpRoute("/", HttpMethod.Get), new StaticFileHandler() { RootDirectory = ConfigurationManager.AppSettings["YinYang.AssetDirectory"] });

			app.Map("/api/accounts", ConfigureAccountApi);
			app.Map("/api/techs", ConfigureTechApi);


			app.Run(context => server.HandleClient(context));
		}

		private static void ConfigureStaticFiles(IAppBuilder app)
		{
			app.Run(new StaticFileHandler() { RootDirectory = ConfigurationManager.AppSettings["YinYang.AssetDirectory"] }.HandleRequestAsync);
		}

		private static Task RedirectAppRequests(IOwinContext context, Func<Task> continuation)
		{
			if (context.Request.Path.StartsWithSegments(new PathString("/app")))
			{
				context.Request.Path = new PathString("/assets/index.html");
			}
			return continuation.Invoke();
		}

		private static void ConfigureLogin(IAppBuilder app)
		{
			app.Run(new SteamLoginHandler().HandleRequestAsync);
		}

		private static void ConfigureAccountApi(IAppBuilder app)
		{
			app.Run(new AccountController().HandleRequestAsync);
		}

		private static void ConfigureTechApi(IAppBuilder app)
		{
			app.Run(new TechController().HandleRequestAsync);
		}
	}
}