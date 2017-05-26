using FundSharer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FundSharer.DataServices
{
    public class TicketServices
    {

        public static WaitingTicket GetTicketById(string TicketId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.WaitingList.Find(TicketId);
            }
        }

        public static void AddTicket(WaitingTicket NewTicket)
        {
            if (IsNotNull(NewTicket))
            {
                if (!ExistInRecord(NewTicket))// ensure that no ticket in the database exists with the specified ticket id
                {
                    //check the ticket has no pending donations... if not, add the ticket to the database and save the changes
                    if (NewTicket.Donations.Count() == 0)
                    {
                        using (ApplicationDbContext db = new ApplicationDbContext())
                        {
                            db.WaitingList.Add(NewTicket);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public static void RemoveTicket(WaitingTicket DeleteTicket)
        {
            if (IsNotNull(DeleteTicket))
            {
                if (ExistInRecord(DeleteTicket))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.WaitingList.Remove(DeleteTicket);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void UpdateTicket(WaitingTicket UpdateTicket)
        {
            if (IsNotNull(UpdateTicket))
            {
                if (ExistInRecord(UpdateTicket))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Entry(UpdateTicket).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        public static WaitingTicket PopOutTopTicket()
        {
            WaitingTicket ticket = null;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (db.WaitingList.Count() > 0) // if there are tickets in the waiting list
                {
                    // get a list tickest of with less than 2 donations
                    List<WaitingTicket> NeedsDonorList = (from t in db.WaitingList where t.Donations.Count() < 2 select t).ToList();
                    // if there are tickets that fit the specified criteria, select the last ticket on the list..
                    //i.e the oldest ticket on the list
                    if (NeedsDonorList.Count() > 0)
                    {
                        ticket = NeedsDonorList.Last();
                    }
                }
            }
            return ticket;
        }

        public static List<WaitingTicket> GetTicketsForAccount(BankAccount Account)
        {
            List<WaitingTicket> TicketList = null;

            if (BankAccountServices.IsNotNull(Account))
            {
                if (BankAccountServices.ExistInRecord(Account))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        TicketList = (from t in db.WaitingList where t.TicketHolder.Id == Account.Id select t).ToList();
                    }
                }
            }
            return TicketList;
        }

        public static List<WaitingTicket> GetTickets()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.WaitingList.ToList();
            }
        }

        #region Helpers
        public static bool IsNotNull(WaitingTicket ticket)
        {
            if (ticket != null)
            { return true; }
            else
            { return false; }
        }

        public static bool ExistInRecord(WaitingTicket ticket)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var testaccount = db.WaitingList.Find(ticket.Id);
                return IsNotNull(testaccount);
            }
                
        }
        #endregion

    }
}