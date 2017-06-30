namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BankAccountModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccounts", "IsReceiver", c => c.Boolean(nullable: false));
            DropColumn("dbo.BankAccounts", "IsReciever");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BankAccounts", "IsReciever", c => c.Boolean(nullable: false));
            DropColumn("dbo.BankAccounts", "IsReceiver");
        }
    }
}
