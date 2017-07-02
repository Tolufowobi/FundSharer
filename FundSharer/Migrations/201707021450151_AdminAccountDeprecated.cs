namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminAccountDeprecated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdminAccounts", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.AdminAccounts", new[] { "OwnerId" });
            DropTable("dbo.AdminAccounts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AdminAccounts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AccountTitle = c.String(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Bank = c.String(nullable: false),
                        AccountNumber = c.String(nullable: false),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        IsReciever = c.Boolean(nullable: false),
                        MatchCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.AdminAccounts", "OwnerId");
            AddForeignKey("dbo.AdminAccounts", "OwnerId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
