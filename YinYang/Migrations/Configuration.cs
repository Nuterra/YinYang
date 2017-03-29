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

			context.Accounts.AddOrUpdate(acc => acc.SteamID, maritaria, testAccount);

			TechUpload tech1 = new TechUpload
			{
				ID = 1,
				OwnerID = maritaria.SteamID,
				Title = "MyFirstTech"
			};
			TechUpload tech2 = new TechUpload
			{
				ID = 2,
				OwnerID = maritaria.SteamID,
				Title = "My other tech"
			};
			TechUpload tech3 = new TechUpload
			{
				ID = 3,
				OwnerID = testAccount.SteamID,
				Title = "I made a thing"
			};
			TechUpload tech4 = new TechUpload
			{
				ID = 4,
				OwnerID = testAccount.SteamID,
				Title = "UNBELIEVABLE SUPER MEGA TECH OMG"
			};

			context.Techs.AddOrUpdate(t => t.Title, tech1, tech2, tech3, tech4);


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
