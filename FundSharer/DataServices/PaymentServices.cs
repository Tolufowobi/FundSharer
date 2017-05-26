using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FundSharer.Models;

namespace FundSharer.DataServices
{
    public class PaymentServices
    {
        public static Payment GetPaymentById(string PaymentId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Payments.Find(PaymentId);
            }
        }

        public static void AddPayment(Payment NewPayment)
        {
            if (IsNotNull(NewPayment))
            {
                if (!ExistInRecord(NewPayment))
                {
                    //Ensure the donation property associated with the payment is not null or open
                    if (NewPayment.DonationPack != null)
                    {
                        if (NewPayment.DonationPack.IsOpen == false)
                        {
                            if (!IsNotNull(GetPaymentForDonation(NewPayment.DonationPack))) // Ensure it doesn't not have any other payment associated with it
                            {
                                using (ApplicationDbContext db = new ApplicationDbContext())
                                {
                                    db.Payments.Add(NewPayment);
                                    db.SaveChanges();
                                }
                            }
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
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Payments.Remove(DeletedPayment);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void UpdatePayment(Payment UpdatePay)
        {
            if (IsNotNull(UpdatePay))
            {
                if (ExistInRecord(UpdatePay))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Entry(UpdatePay).State = System.Data.Entity.EntityState.Modified;
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
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var paylist = (from p in db.Payments where p.DonationPack.Id == donation.Id select p).ToList();
                    if (paylist.Count() > 0)
                    {
                        donationPay = paylist.First();
                    }
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
                        if (t.Donations.Count() > 0)
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
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Payments.ToList();
            }
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
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var test = db.Payments.Find(payment.Id);
                return IsNotNull(test);
            }
        }
        #endregion

    }
}