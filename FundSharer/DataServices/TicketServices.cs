using FundSharer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FundSharer.DataServices
{
    public class TicketServices
    {

        public static WaitingTicket GetTicketById(string TicketId)
        {
                return DbAccessHandler.DbContext.WaitingList.Find(TicketId);
            
        }

        public static void AddTicket(WaitingTicket NewTicket)
        {
            if (IsNotNull(NewTicket))
            {
                if (!ExistInRecord(NewTicket))// ensure that no ticket in the database exists with the specified ticket id
                {
                    //check the ticket has no pending donations... if not, add the ticket to the database and save the changes
                    if (NewTicket.Donations == null)
                    {
                        if ( BankAccountServices.IsNotNull(NewTicket.TicketHolder))
                        {
                            DbAccessHandler.DbContext.Entry(NewTicket.TicketHolder).State = EntityState.Unchanged;
                            DbAccessHandler.DbContext.WaitingList.Add(NewTicket);
                            DbAccessHandler.DbContext.SaveChanges();
                            
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
                    DbAccessHandler.DbContext.WaitingList.Remove(DeleteTicket);
                    DbAccessHandler.DbContext.SaveChanges();
                }
            }
        }

        public static void UpdateTicket(WaitingTicket UpdateTicket)
        {
            if (IsNotNull(UpdateTicket))
            {
                if (ExistInRecord(UpdateTicket))
                {
                    DbAccessHandler.DbContext.Entry(UpdateTicket).State = EntityState.Modified;
                    DbAccessHandler.DbContext.SaveChanges();
                    
                }
            }
        }

        public static WaitingTicket PopOutTopTicket()
        {
            WaitingTicket ticket = null;
            
                if (DbAccessHandler.DbContext.WaitingList.Count() > 0) // if there are tickets in the waiting list
                {
                    // get a list tickest of with less than 2 donations
                    List<WaitingTicket> NeedsDonorList = (from t in DbAccessHandler.DbContext.WaitingList where t.Donations.Count() < 2 select t).ToList();
                    // if there are tickets that fit the specified criteria, select the last ticket on the list..
                    //i.e the oldest ticket on the list
                    if (NeedsDonorList.Count() > 0)
                    {
                        ticket = NeedsDonorList.Last();
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
                        TicketList = (from t in DbAccessHandler.DbContext.WaitingList where t.TicketHolder.Id == Account.Id select t).ToList();
                    
                }
            }
            return TicketList;
        }

        public static List<WaitingTicket> GetTickets()
        {
                return DbAccessHandler.DbContext.WaitingList.ToList();
            
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
                var testaccount = DbAccessHandler.DbContext.WaitingList.Find(ticket.Id);
                return IsNotNull(testaccount);
            
                
        }
        #endregion

    }
}