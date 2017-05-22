namespace FundSharer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.BankAccounts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AccountTitle = c.String(nullable: false),
                        Bank = c.String(nullable: false),
                        AccountNumber = c.String(nullable: false),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        IsReciever = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Donations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreationDate = c.DateTime(nullable: false),
                        IsOpen = c.Boolean(nullable: false),
                        DonorId = c.Int(nullable: false),
                        TicketId = c.String(nullable: false, maxLength: 128),
                        Donor_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.Donor_Id, cascadeDelete: true)
                .ForeignKey("dbo.WaitingTickets", t => t.TicketId, cascadeDelete: true)
                .Index(t => t.TicketId)
                .Index(t => t.Donor_Id);
            
            CreateTable(
                "dbo.WaitingTickets",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EntryDate = c.DateTime(nullable: false),
                        TicketHolder_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.TicketHolder_Id)
                .Index(t => t.TicketHolder_Id);
            
            CreateTable(
                "dbo.RevenueTargets",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Income = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationDate = c.DateTime(nullable: false),
                        Confirmed = c.Boolean(nullable: false),
                        DonationPack_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Donations", t => t.DonationPack_Id)
                .Index(t => t.DonationPack_Id);
            
            CreateTable(
                "dbo.POPImages",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Image = c.Binary(),
                        Payment_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Payments", t => t.Payment_Id)
                .Index(t => t.Payment_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.POPImages", "Payment_Id", "dbo.Payments");
            DropForeignKey("dbo.Payments", "DonationPack_Id", "dbo.Donations");
            DropForeignKey("dbo.Donations", "TicketId", "dbo.WaitingTickets");
            DropForeignKey("dbo.WaitingTickets", "TicketHolder_Id", "dbo.BankAccounts");
            DropForeignKey("dbo.Donations", "Donor_Id", "dbo.BankAccounts");
            DropForeignKey("dbo.BankAccounts", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AdminAccounts", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.POPImages", new[] { "Payment_Id" });
            DropIndex("dbo.Payments", new[] { "DonationPack_Id" });
            DropIndex("dbo.WaitingTickets", new[] { "TicketHolder_Id" });
            DropIndex("dbo.Donations", new[] { "Donor_Id" });
            DropIndex("dbo.Donations", new[] { "TicketId" });
            DropIndex("dbo.BankAccounts", new[] { "OwnerId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AdminAccounts", new[] { "OwnerId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.POPImages");
            DropTable("dbo.Payments");
            DropTable("dbo.RevenueTargets");
            DropTable("dbo.WaitingTickets");
            DropTable("dbo.Donations");
            DropTable("dbo.BankAccounts");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AdminAccounts");
        }
    }
}
