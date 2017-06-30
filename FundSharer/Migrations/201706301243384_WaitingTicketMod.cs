namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WaitingTicketMod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WaitingTickets", "IsValid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WaitingTickets", "IsValid");
        }
    }
}
