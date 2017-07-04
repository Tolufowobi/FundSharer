namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BAcctModified : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.BankAccounts", name: "OwnerId", newName: "BankAccountOwnerId");
            RenameIndex(table: "dbo.BankAccounts", name: "IX_OwnerId", newName: "IX_BankAccountOwnerId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.BankAccounts", name: "IX_BankAccountOwnerId", newName: "IX_OwnerId");
            RenameColumn(table: "dbo.BankAccounts", name: "BankAccountOwnerId", newName: "OwnerId");
        }
    }
}
