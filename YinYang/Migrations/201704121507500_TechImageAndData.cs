namespace YinYang.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class TechImageAndData : DbMigration
	{
		public override void Up()
		{
			AddColumn("TechUploads", "ImageUrl", c => c.String(unicode: false));
			AddColumn("TechUploads", "TechData", c => c.String(unicode: false));
		}

		public override void Down()
		{
			DropColumn("TechUploads", "TechData");
			DropColumn("TechUploads", "ImageUrl");
		}
	}
}
