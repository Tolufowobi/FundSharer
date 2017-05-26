namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketModelUpdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WaitingTickets", new[] { "TicketHolder_Id" });
            DropForeignKey(dependentTable: "dbo.WaitingTickets", dependentColumn: "TicketHolder_Id", principalTable: "dbo.BankAccounts");
            
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.WaitingTickets", name: "IX_TicketHolderId", newName: "IX_TicketHolder_Id");
            RenameColumn(table: "dbo.WaitingTickets", name: "TicketHolderId", newName: "TicketHolder_Id");
        }
    }
}
