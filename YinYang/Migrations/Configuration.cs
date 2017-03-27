namespace YinYang.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	using Community;
	using MySql.Data.Entity;

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