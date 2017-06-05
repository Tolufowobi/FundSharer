namespace FundSharer.Migrations
{
    using FundSharer.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FundSharer.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "FundSharer.Models.ApplicationDbContext";
        }

        protected override void  Seed(FundSharer.Models.ApplicationDbContext context)
        {

            // Reset the database
            
            //AdminAccounts
            context.AdminAccounts.RemoveRange(context.AdminAccounts.ToList());

            //BankAccounts
            context.BankAccounts.RemoveRange(context.BankAccounts.ToList());

            //Donations
            context.Donations.RemoveRange(context.Donations.ToList());

            //Payments
            context.Payments.RemoveRange(context.Payments.ToList());

            //PopImages
            context.POPImages.RemoveRange(context.POPImages.ToList());

            //WaitingTickets
            context.WaitingList.RemoveRange(context.WaitingList.ToList());
            
            
            
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
