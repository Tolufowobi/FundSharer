 using FundSharer.DataServices;
using FundSharer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FundSharer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("Welcome")]
        public ActionResult Home()
        {
            if (User.IsInRole("Administrator"))
            {
                return Dashboard();
            }
            else
            {
                return Homepage();
            }
                

        }
        
        [Authorize(Roles = "Administrator")]
        internal ActionResult Dashboard()
        {
            using (var bd = new ApplicationDbContext())
            {
                
            }
                return View("Dashboard");
        }

        internal ActionResult Homepage()
        {
            ApplicationUser AppUser;
            // Get data
            var UserId = User.Identity.GetUserId();
            AppUser = DbAccessHandler.DbContext.Users.Find(UserId);
            var UserBankAccount = BankAccountServices.GetUserBankAccount(AppUser);
            var IncomingDonations = DonationServices.GetIncomingAccountDonations(UserBankAccount);
            List<Payment> IncomingPayments = new List<Payment>();
            foreach (Donation d in IncomingDonations)
            {
                Payment p = PaymentServices.GetPaymentForDonation(d);
                if (PaymentServices.IsNotNull(p))
                {
                    IncomingPayments.Add(p);
                }
            }

            var OutgoingDonations = DonationServices.GetOutgoingAccountDonations(UserBankAccount);
            List<Payment> OutgoingPayments = new List<Payment>();
            foreach (Donation d in OutgoingDonations)
            {
                Payment p1 = PaymentServices.GetPaymentForDonation(d);
                if (PaymentServices.IsNotNull(p1))
                {
                    OutgoingPayments.Add(p1);
                }
            }


            List<Donation> _pendingIncomingDonations = new List<Donation>(2);
            List<Payment> _pendingIncomingpayments = new List<Payment>(2);

            Donation PendingOutgoingDonation = null;
            Payment PendingOutgoingPayment = null;

            if (UserBankAccount.IsReceiver == false)
            {
                if (OutgoingDonations.Where(m => m.IsOpen == false).Count() > 0)
                {
                    PendingOutgoingDonation = OutgoingDonations.Where(m => m.IsOpen == false).First();
                }

                if (OutgoingPayments.Where(m => m.Confirmed == false).Count() > 0)
                {
                    PendingOutgoingPayment = OutgoingPayments.Where(m => m.Confirmed == false).First();
                }

            }
            else
            {
                var IDnts = IncomingDonations.Where(m => m.IsOpen == false).ToList();
                if (IDnts != null)
                {
                    _pendingIncomingDonations = IDnts;
                }
                var IPymts = IncomingPayments.Where(m => m.Confirmed == false).ToList();
                if (IPymts != null)
                {
                    _pendingIncomingpayments = IPymts;
                }
            }

            //Get Users personal Information
            var FirstName = AppUser.FirstName;
            var LastName = AppUser.LastName;
            var PhoneNumber = AppUser.PhoneNumber;
            if (AppUser.PhoneNumberConfirmed == false)
            {
                PhoneNumber = PhoneNumber + " (Unconfirmed)";
            }
            var Emailaddress = AppUser.Email;

            //Add objects to the data dictionary

            //Personal Information
            UserDetails Ud = new UserDetails
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                UserName = Emailaddress
            };
            ViewBag.UserDetails = Ud;

            //Account Information
            BankAccountDetails Bd = new BankAccountDetails
            {
                AccountTitle = UserBankAccount.AccountTitle,
                AccountNumber = UserBankAccount.AccountNumber,
                BankName = UserBankAccount.Bank,
                IsReceiver = UserBankAccount.IsReceiver
            };
            ViewBag.BankAccountDetails = Bd;

            //Outgoing Payments Information
            ViewBag.OutgoingPayments = OutgoingPayments.Where(m => m.Confirmed == true).Select(m => m.CreationDate).ToList();

            //Incoming Payments Information
            ViewBag.IncomingPayments = IncomingPayments.Where(m => m.Confirmed == true).Select(m => m.CreationDate).ToList();

            //Pending Donors Information
            var PendingIncomingDonations = new List<Dictionary<string, object>>();
            if (_pendingIncomingDonations.Count > 0)
            {
                foreach (Donation d in _pendingIncomingDonations)
                {
                    var DonationId = d.Id;
                    var Name = d.Donor.AccountTitle;
                    var AccountNumber = d.Donor.AccountNumber;
                    var BankName = d.Donor.Bank;
                    var Payment = _pendingIncomingpayments.Where(p => p.DonationPack.Id == d.Id).FirstOrDefault();

                    Dictionary<string, object> set = new Dictionary<string, object>();
                    set.Add("DonationId", DonationId);
                    set.Add("Name", Name);
                    set.Add("AccountNumber", AccountNumber);
                    set.Add("BankName", BankName);
                    if (Payment != null)
                    {
                        set.Add("PaymentId", Payment.Id);
                        set.Add("Confirmed", Payment.Confirmed);
                    }
                    else
                    {
                        set.Add("PaymentId", null);
                        set.Add("Confirmed", null);
                    }
                    PendingIncomingDonations.Add(set);
                }
            }
            ViewBag.PendingIncomingDonations = PendingIncomingDonations;

            //Pending Outgoing Donation View
            DonationDetails DonationDetails = null;
            if (PendingOutgoingDonation != null)
            {
                DonationDetails = new DonationDetails();
                DonationDetails.DonationId = PendingOutgoingDonation.Id;
                DonationDetails.RecipientFullName = PendingOutgoingDonation.Ticket.TicketHolder.AccountTitle;
                DonationDetails.RecipientAccountNumber = PendingOutgoingDonation.Ticket.TicketHolder.AccountNumber;
                DonationDetails.RecipientBankName = PendingOutgoingDonation.Ticket.TicketHolder.Bank;
                DonationDetails.DonationSetupDate = PendingOutgoingDonation.CreationDate;
            }
            ViewBag.PendingOutgoingDonation = DonationDetails;
            PaymentDetails PayDetails = null;
            if (PendingOutgoingPayment != null)
            {
                PayDetails = new PaymentDetails();
                PayDetails.PaymentId = PendingOutgoingPayment.Id;
                PayDetails.Status = PendingOutgoingPayment.Confirmed;
                PayDetails.Date = PendingOutgoingPayment.CreationDate.ToShortDateString();
            }
            ViewBag.PendingPayDetails = PayDetails;

            //Pending Outgoing Payment Information
            ViewBag.PendingOutgoingPayment = PendingOutgoingPayment;

            return View("HomePage");
        }

        public ActionResult FindAMatch()
        {
            var UserId = User.Identity.GetUserId();
            ApplicationUser AppUser;
            using (var db = new ApplicationDbContext())
            {
                AppUser = db.Users.Find(UserId);
                BankAccount donor = (from ba in db.BankAccounts where ba.OwnerId == AppUser.Id select ba).FirstOrDefault();
                string AdminUid = (from r in db.Roles where r.Name == "Administrator" select r).FirstOrDefault().Users.FirstOrDefault().UserId;
                WaitingTicket Ticket = (from t in db.WaitingList where t.Donations.Count < 2 && t.IsValid == true && t.TicketHolderId == AdminUid orderby t.EntryDate select t).FirstOrDefault();
                if(Ticket == null)
                {
                    Ticket = (from t in db.WaitingList where t.Donations.Count < 2 & t.IsValid == true orderby t.EntryDate select t).FirstOrDefault();
                }
             
                if (Ticket != null)
                {
                    Donation NewDonation = new Donation
                    {
                        Donor = donor,
                        DonorId = donor.Id,
                        IsOpen = false,
                        Ticket = Ticket,
                        TicketId = Ticket.Id,
                        CreationDate = DateTime.Now
                    };
                    db.Donations.Add(NewDonation);
                    db.SaveChanges();
                    DonationDetails DonDetails = new DonationDetails { DonationId = NewDonation.Id,
                        RecipientFullName = NewDonation.Ticket.TicketHolder.AccountTitle,
                        RecipientAccountNumber = NewDonation.Ticket.TicketHolder.AccountNumber,
                        RecipientBankName = NewDonation.Ticket.TicketHolder.Bank,
                    };
                    return PartialView("_FindAMatch", DonDetails );
                }
                else { return HttpNotFound(); }
            }
          
            
        }
      
    }

}
