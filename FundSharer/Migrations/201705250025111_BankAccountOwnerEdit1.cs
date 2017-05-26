namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BankAccountOwnerEdit1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BankAccounts", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.BankAccounts", new[] { "Owner_Id" });
            RenameColumn(table: "dbo.BankAccounts", name: "Owner_Id", newName: "OwnerId");
            AlterColumn("dbo.BankAccounts", "OwnerId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.BankAccounts", "OwnerId");
            AddForeignKey("dbo.BankAccounts", "OwnerId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BankAccounts", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.BankAccounts", new[] { "OwnerId" });
            AlterColumn("dbo.BankAccounts", "OwnerId", c => c.String(maxLength: 128));
            RenameColumn(table: "dbo.BankAccounts", name: "OwnerId", newName: "Owner_Id");
            CreateIndex("dbo.BankAccounts", "Owner_Id");
            AddForeignKey("dbo.BankAccounts", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
