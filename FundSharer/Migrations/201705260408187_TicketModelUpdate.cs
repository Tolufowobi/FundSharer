namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketModelUpdate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.WaitingTickets", name: "TicketHolder_Id", newName: "TicketHolderId");
            RenameIndex(table: "dbo.WaitingTickets", name: "IX_TicketHolder_Id", newName: "IX_TicketHolderId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.WaitingTickets", name: "IX_TicketHolderId", newName: "IX_TicketHolder_Id");
            RenameColumn(table: "dbo.WaitingTickets", name: "TicketHolderId", newName: "TicketHolder_Id");
        }
    }
}
