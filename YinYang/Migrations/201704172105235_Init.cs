namespace YinYang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Accounts",
                c => new
                    {
                        SteamID = c.Long(nullable: false),
                        Username = c.String(unicode: false),
                        Flags = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SteamID)                ;
            
            CreateTable(
                "Teches",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OwnerID = c.Long(nullable: false),
                        Title = c.String(unicode: false),
                        ImageUrl = c.String(unicode: false),
                        TechData = c.String(unicode: false),
                        Featured = c.Boolean(nullable: false),
                        CreationTime = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.ID)                
                .ForeignKey("Accounts", t => t.OwnerID, cascadeDelete: true)
                .Index(t => t.OwnerID);
            
            CreateTable(
                "AccountTeches",
                c => new
                    {
                        Account_SteamID = c.Long(nullable: false),
                        Tech_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Account_SteamID, t.Tech_ID })                
                .ForeignKey("Accounts", t => t.Account_SteamID, cascadeDelete: true)
                .ForeignKey("Teches", t => t.Tech_ID, cascadeDelete: true)
                .Index(t => t.Account_SteamID)
                .Index(t => t.Tech_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Teches", "OwnerID", "Accounts");
            DropForeignKey("AccountTeches", "Tech_ID", "Teches");
            DropForeignKey("AccountTeches", "Account_SteamID", "Accounts");
            DropIndex("AccountTeches", new[] { "Tech_ID" });
            DropIndex("AccountTeches", new[] { "Account_SteamID" });
            DropIndex("Teches", new[] { "OwnerID" });
            DropTable("AccountTeches");
            DropTable("Teches");
            DropTable("Accounts");
        }
    }
}
