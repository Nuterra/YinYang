namespace YinYang.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	using Community;
	using MySql.Data.Entity;
	using Steam;

	internal sealed class Configuration : DbMigrationsConfiguration<CommunityContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
			SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
			//SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));// BROKEN
			CodeGenerator = new MySqlMigrationCodeGenerator();
		}

		protected override void Seed(CommunityContext context)
		{
			//  This method will be called after migrating to the latest version.

			Account maritaria = new Account
			{
				SteamID = 76561198023393043,
				Username = "maritaria",
				Flags = AccountFlags.Activated | AccountFlags.Admin
			};
			Account testAccount = new Account
			{
				SteamID = new SteamID(SteamUniverse.Public, SteamAccountType.Individual, SteamInstance.Desktop, 1).ToSteamID64(),
				Username = "test_Account",
				Flags = AccountFlags.Activated,
			};


			Tech tech1 = new Tech
			{
				ID = 1,
				OwnerID = maritaria.SteamID,
				Title = "MyFirstTech",
				ImageUrl = "http://i.imgur.com/M6aJxUX.png",
				TechData = "this is a 1 tech",
				Featured = true,
			};
			Tech tech2 = new Tech
			{
				ID = 2,
				OwnerID = maritaria.SteamID,
				Title = "My other tech",
				ImageUrl = "http://i.imgur.com/jGONnH2.png",
				TechData = "this is a 2 tech",
			};
			Tech tech3 = new Tech
			{
				ID = 3,
				OwnerID = testAccount.SteamID,
				Title = "I made a thing",
				ImageUrl = "http://i.imgur.com/xgCE4Uj.png",
				TechData = "this is a 3 tech",
				CreationTime = DateTime.Now.Add(TimeSpan.FromMinutes(5)),
			};
			Tech tech4 = new Tech
			{
				ID = 4,
				OwnerID = testAccount.SteamID,
				Title = "UNBELIEVABLE SUPER MEGA TECH OMGeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
				ImageUrl = "http://i.imgur.com/qBcJZbH.png",
				TechData = "this is a 4 techeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
				CreationTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(5)),
			};
			maritaria.SubscribedTechs.Add(tech1);
			maritaria.SubscribedTechs.Add(tech2);

			testAccount.SubscribedTechs.Add(tech2);
			testAccount.SubscribedTechs.Add(tech3);

			context.Techs.AddOrUpdate(t => t.ID, tech1, tech2, tech3, tech4);
			context.Accounts.AddOrUpdate(acc => acc.SteamID, maritaria, testAccount);

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
		}
	}
}