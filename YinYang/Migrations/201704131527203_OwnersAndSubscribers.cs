namespace YinYang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OwnersAndSubscribers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "AccountTechUploads",
                c => new
                    {
                        Account_SteamID = c.Long(nullable: false),
                        TechUpload_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Account_SteamID, t.TechUpload_ID })                
                .ForeignKey("Accounts", t => t.Account_SteamID, cascadeDelete: true)
                .ForeignKey("TechUploads", t => t.TechUpload_ID, cascadeDelete: true)
                .Index(t => t.Account_SteamID)
                .Index(t => t.TechUpload_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("AccountTechUploads", "TechUpload_ID", "TechUploads");
            DropForeignKey("AccountTechUploads", "Account_SteamID", "Accounts");
            DropIndex("AccountTechUploads", new[] { "TechUpload_ID" });
            DropIndex("AccountTechUploads", new[] { "Account_SteamID" });
            DropTable("AccountTechUploads");
        }
    }
}
