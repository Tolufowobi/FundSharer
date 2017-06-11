using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FundSharer.Models;
using System.Data.Entity;

namespace FundSharer.DataServices
{
    public class PaymentServices
    {
        public static Payment GetPaymentById(string PaymentId)
        {
                return DbAccessHandler.DbContext.Payments.Find(PaymentId);
            
        }

        public static void AddPayment(Payment NewPayment)
        {
            if (IsNotNull(NewPayment))
            {
                if (!ExistInRecord(NewPayment))
                {
                    //Ensure the donation property associated with the payment is not null or open
                    if ( DonationServices.IsNotNull(NewPayment.DonationPack))
                    {
                        if (NewPayment.DonationPack.IsOpen == false)
                        {
                            //if (!IsNotNull(GetPaymentForDonation(NewPayment.DonationPack))) // Ensure it doesn't not have any other payment associated with it
                            //{
                                DbAccessHandler.DbContext.Payments.Add(NewPayment);
                                DbAccessHandler.DbContext.SaveChanges();
                            //}
                        }
                    }
                }
            }

        }

        public static void RemovePayment(Payment DeletedPayment)
        {
            if (IsNotNull(DeletedPayment))
            {
                if (ExistInRecord(DeletedPayment))
                {
                    DbAccessHandler.DbContext.Payments.Remove(DeletedPayment);
                    DbAccessHandler.DbContext.SaveChanges();
                }
            }
        }

        public static void UpdatePayment(Payment UpdatePay)
        {
            if (IsNotNull(UpdatePay))
            {
                if (ExistInRecord(UpdatePay))
                {
                    using (var db = DbAccessHandler.DbContext)
                    {
                        db.Entry(UpdatePay).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        public static Payment GetPaymentForDonation(Donation donation)
        {
            Payment donationPay = null;

            if (DonationServices.IsNotNull(donation))
            {
                    var pay = (from p in DbAccessHandler.DbContext.Payments where p.DonationPackId == donation.Id select p).FirstOrDefault();
                    if (pay != null)
                    {
                        donationPay = pay;
                    }
                
            }
            return donationPay;
        }

        public static List<Payment> GetOutgoingPaymentsForAccount(BankAccount account)
        {
            List<Payment> OutgoingPaymentsList = new List<Payment>();
            if (BankAccountServices.IsNotNull(account))
            {
                List<Donation> OutgoingAccountDonations = DonationServices.GetOutgoingAccountDonations(account);
                if (OutgoingAccountDonations.Count() > 0)
                {
                    foreach (Donation d in OutgoingAccountDonations)
                    {
                        Payment p = GetPaymentForDonation(d);
                        if (IsNotNull(p))
                        {
                            OutgoingPaymentsList.Add(p);
                        }
                    }
                }
            }
            return OutgoingPaymentsList;
        }

        public static List<Payment> GetIncomingPaymentsForAccount(BankAccount account)
        {
            List<Payment> IncomingPaymentsList = new List<Payment>();
            if (BankAccountServices.IsNotNull(account))
            {
                List<WaitingTicket> AccountTickets = TicketServices.GetTicketsForAccount(account);
                if (AccountTickets.Count() > 0)
                {
                    foreach (WaitingTicket t in AccountTickets)
                    {
                        if (t.Donations != null)
                        {
                            foreach (Donation d in t.Donations)
                            {
                                Payment p = GetPaymentForDonation(d);
                                if (IsNotNull(p))
                                {
                                    IncomingPaymentsList.Add(p);
                                }
                            }
                        }
                        
                    }
                }
            }
            return IncomingPaymentsList;
        }

        public static List<Payment> GetPayments()
        {
                return DbAccessHandler.DbContext.Payments.ToList();
            
        }

        #region Helpers
        public static bool IsNotNull(Payment payment)
        {
            if (payment != null)
            { return true; }
            else
            { return false; }
        }

        public static bool ExistInRecord(Payment payment)
        {
                var test = DbAccessHandler.DbContext.Payments.Find(payment.Id);
                return IsNotNull(test);
        }
        #endregion

    }
}