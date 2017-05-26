namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BankAccountOwnerEdit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BankAccounts", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.BankAccounts", new[] { "Owner_Id" });
            AlterColumn("dbo.BankAccounts", "Owner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.BankAccounts", "Owner_Id");
            AddForeignKey("dbo.BankAccounts", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BankAccounts", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.BankAccounts", new[] { "Owner_Id" });
            AlterColumn("dbo.BankAccounts", "Owner_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.BankAccounts", "Owner_Id");
            AddForeignKey("dbo.BankAccounts", "Owner_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
