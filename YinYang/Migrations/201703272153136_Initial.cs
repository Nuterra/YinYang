namespace YinYang.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class Initial : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.Accounts",
				c => new
				{
					SteamID = c.Long(nullable: false),
					Username = c.String(unicode: false),
					Flags = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.SteamID);

			CreateTable(
				"dbo.TechUploads",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					OwnerID = c.Long(nullable: false),
					Title = c.String(unicode: false),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Accounts", t => t.OwnerID, cascadeDelete: true)
				.Index(t => t.OwnerID);
		}

		public override void Down()
		{
			DropForeignKey("dbo.TechUploads", "OwnerID", "dbo.Accounts");
			DropIndex("dbo.TechUploads", new[] { "OwnerID" });
			DropTable("dbo.TechUploads");
			DropTable("dbo.Accounts");
		}
	}
}