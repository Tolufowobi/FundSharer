namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedTransactionModels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Donations", "Donor_Id", "dbo.BankAccounts");
            DropForeignKey("dbo.Donations", "TicketId", "dbo.WaitingTickets");
            DropIndex("dbo.Donations", new[] { "TicketId" });
            DropIndex("dbo.Donations", new[] { "Donor_Id" });
            DropColumn("dbo.Donations", "DonorId");
            RenameColumn(table: "dbo.Donations", name: "Donor_Id", newName: "DonorId");
            RenameColumn(table: "dbo.Payments", name: "DonationPack_Id", newName: "DonationPackId");
            RenameColumn(table: "dbo.POPImages", name: "Payment_Id", newName: "PaymentId");
            RenameIndex(table: "dbo.Payments", name: "IX_DonationPack_Id", newName: "IX_DonationPackId");
            RenameIndex(table: "dbo.POPImages", name: "IX_Payment_Id", newName: "IX_PaymentId");
            AlterColumn("dbo.Donations", "DonorId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Donations", "TicketId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Donations", "DonorId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Donations", "DonorId");
            CreateIndex("dbo.Donations", "TicketId");
            AddForeignKey("dbo.Donations", "DonorId", "dbo.BankAccounts", "Id");
            AddForeignKey("dbo.Donations", "TicketId", "dbo.WaitingTickets", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donations", "TicketId", "dbo.WaitingTickets");
            DropForeignKey("dbo.Donations", "DonorId", "dbo.BankAccounts");
            DropIndex("dbo.Donations", new[] { "TicketId" });
            DropIndex("dbo.Donations", new[] { "DonorId" });
            AlterColumn("dbo.Donations", "DonorId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Donations", "TicketId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Donations", "DonorId", c => c.Int(nullable: false));
            RenameIndex(table: "dbo.POPImages", name: "IX_PaymentId", newName: "IX_Payment_Id");
            RenameIndex(table: "dbo.Payments", name: "IX_DonationPackId", newName: "IX_DonationPack_Id");
            RenameColumn(table: "dbo.POPImages", name: "PaymentId", newName: "Payment_Id");
            RenameColumn(table: "dbo.Payments", name: "DonationPackId", newName: "DonationPack_Id");
            RenameColumn(table: "dbo.Donations", name: "DonorId", newName: "Donor_Id");
            AddColumn("dbo.Donations", "DonorId", c => c.Int(nullable: false));
            CreateIndex("dbo.Donations", "Donor_Id");
            CreateIndex("dbo.Donations", "TicketId");
            AddForeignKey("dbo.Donations", "TicketId", "dbo.WaitingTickets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Donations", "Donor_Id", "dbo.BankAccounts", "Id", cascadeDelete: true);
        }
    }
}
