namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedBankAccountModel : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.BankAccounts", name: "OwnerId", newName: "Owner_Id");
            RenameIndex(table: "dbo.BankAccounts", name: "IX_OwnerId", newName: "IX_Owner_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.BankAccounts", name: "IX_Owner_Id", newName: "IX_OwnerId");
            RenameColumn(table: "dbo.BankAccounts", name: "Owner_Id", newName: "OwnerId");
        }
    }
}
