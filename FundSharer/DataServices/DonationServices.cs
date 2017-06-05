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
                return DbAccessHandler.DbContext.Donations.Find(DonationId);
            
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
                        DbAccessHandler.DbContext.Donations.Add(NewDonation);
                        DbAccessHandler.DbContext.SaveChanges();
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
                    DbAccessHandler.DbContext.Donations.Remove(DeleteDonation);
                    DbAccessHandler.DbContext.SaveChanges();
                }
            }
        }

        public static void UpdateDonations(Donation UpdateDonation)
        {
            if (IsNotNull(UpdateDonation))
            {
                if (ExistInRecord(UpdateDonation))
                {
                    DbAccessHandler.DbContext.Entry(UpdateDonation).State = EntityState.Modified;
                    DbAccessHandler.DbContext.SaveChanges();
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
                        OutgoingDonations = (from d in DbAccessHandler.DbContext.Donations where d.DonorId == Account.Id select d).ToList();
                   
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
                return DbAccessHandler.DbContext.Donations.ToList();
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
                var test = DbAccessHandler.DbContext.Donations.Find(donation.Id);
                return IsNotNull(test);
            
        }
        #endregion

    }


}
