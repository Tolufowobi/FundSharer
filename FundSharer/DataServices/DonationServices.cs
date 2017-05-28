using FundSharer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FundSharer.DataServices
{
    public class DonationServices
    {

        public static Donation GetDonationById(string DonationId)
        {
            if (DonationId == "")
            {
                return null;
            }
                using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Donations.Find(DonationId);
            }
        }

        //Get a donation package for a new donor
        static public void AddDonation(Donation NewDonation)
        {

            if (IsNotNull(NewDonation))// Ensure the specified donation object is not null
            {
                //Ensure that the donation does not exist in the database
                if (!ExistInRecord(NewDonation))
                {
                    if (TicketServices.IsNotNull(NewDonation.Ticket) && BankAccountServices.IsNotNull(NewDonation.Donor))
                    { 
                        using (ApplicationDbContext db = new ApplicationDbContext())
                        {
                            db.Entry(NewDonation.Ticket).State = EntityState.Unchanged;
                            db.Entry(NewDonation.Donor).State = EntityState.Unchanged;
                            db.Donations.Add(NewDonation);
                            db.SaveChanges();
                        }
                    }
                }
            }

        }

        public static void RemoveDonation(Donation DeleteDonation)
        {
            if (IsNotNull(DeleteDonation))
            {
                if (ExistInRecord(DeleteDonation))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Donations.Remove(DeleteDonation);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void UpdateDonations(Donation UpdateDonation)
        {
            if (IsNotNull(UpdateDonation))
            {
                if (ExistInRecord(UpdateDonation))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Entry(UpdateDonation).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        static public List<Donation> GetOutgoingAccountDonations(BankAccount Account)
        {
            List<Donation> OutgoingDonations = null;
            if (BankAccountServices.IsNotNull(Account))
            {
                if (BankAccountServices.ExistInRecord(Account))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        OutgoingDonations = (from d in db.Donations where d.DonorId == Account.Id select d).ToList();
                    }
                }
            }
            return OutgoingDonations;
        }

        static public List<Donation> GetIncomingAccountDonations(BankAccount Account)
        {
            List<Donation> IncomingDonations = new List<Donation>();
            if (BankAccountServices.IsNotNull(Account))
            {
                if (BankAccountServices.ExistInRecord(Account))
                {
                    List<WaitingTicket> AccountTickets = TicketServices.GetTicketsForAccount(Account);
                    foreach (WaitingTicket t in AccountTickets)// loop through the list of ticket options
                    {
                        if (t.Donations.Count() != 0)
                        {
                            foreach (Donation d in t.Donations)// loop through the donations list of each ticket
                            {
                                if (DonationServices.IsNotNull(d))// add non null donations to the list of donations.
                                {
                                    IncomingDonations.Add(d);
                                }
                            }
                        }
                    }
                }
            }
            return IncomingDonations;
        }

        public static List<Donation> GetDonations()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Donations.ToList();
            }
        }

        #region Helpers
        public static bool IsNotNull(Donation donation)
        {
            if (donation != null)
            { return true; }
            else
            { return false; }
        }

        public static bool ExistInRecord(Donation donation)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var test = db.Donations.Find(donation.Id);
                return IsNotNull(test);
            }
        }
        #endregion

    }


}
