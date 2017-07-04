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

            //Populate database
            var Users = (from u in context.Users select u).ToList();

            //Create Bank Accounts
            String[] Banks = { "GTB", "UBA", "Fidelity", "First Bank" };
            int counter = 0;
            foreach (ApplicationUser User in Users)
            {
                if (User.FirstName != "Admin")
                {
                    BankAccount b = new BankAccount
                    {
                        AccountTitle = User.FullName,
                        AccountNumber = "112345",
                        Bank = Banks[counter],
                        BankAccountOwner = User,
                        BankAccountOwnerId = User.Id
                    };
                    context.BankAccounts.Add(b);
                    if (counter == 0) // Create the first ticket 
                    {
                        var firstticket = new WaitingTicket { EntryDate = DateTime.Now, TicketHolder = b, TicketHolderId = b.Id, IsValid = true };
                        context.WaitingList.Add(firstticket);
                        b.IsReceiver = true;
                    }
                    counter++;
                }

            }
        }
    }
}
