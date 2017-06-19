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
        private ActionResult Dashboard()
        {
                return View("Dashboard");
        }

        public ActionResult FindAMatch()
        {
            var UserId = User.Identity.GetUserId();
            ApplicationUser AppUser;
            using (var db = new ApplicationDbContext())
            {
                AppUser = db.Users.Find(UserId);
                BankAccount donor = (from ba in db.BankAccounts where ba.OwnerId == AppUser.Id select ba).FirstOrDefault();
                WaitingTicket ticket = (from t in db.WaitingList where t.Donations.Count < 2 orderby t.EntryDate select t).FirstOrDefault();
                if (ticket != null)
                {
                    Donation NewDonation = new Donation
                    {
                        Donor = donor,
                        DonorId = donor.Id,
                        IsOpen = false,
                        Ticket = ticket,
                        TicketId = ticket.Id,
                        CreationDate = DateTime.Now
                    };
                    db.Donations.Add(NewDonation);
                    db.SaveChanges();
                    ViewBag.Donation = NewDonation;
                    return PartialView("_FindAMatch");
                }
                else { return HttpNotFound(); }
            }
          
            
        }

        private ActionResult Homepage()
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

            if (UserBankAccount.IsReciever == false)
            {
                if (OutgoingDonations.Where(m => m.IsOpen == false).Count() > 0)
                {
                    PendingOutgoingDonation = OutgoingDonations.Where(m => m.IsOpen == false).First();
                }

                if (OutgoingPayments.Where(m => m.Confirmed = false).Count() > 0)
                {
                    PendingOutgoingPayment = OutgoingPayments.Where(m => m.Confirmed = false).First();
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
            ViewBag.FirstName = FirstName;
            ViewBag.LastName = LastName;
            ViewBag.PhoneNumber = PhoneNumber;
            ViewBag.EmailAddress = Emailaddress;

            //Account Information
            ViewBag.AccountTitle = UserBankAccount.AccountTitle;
            ViewBag.AccountNumber = UserBankAccount.AccountNumber;
            ViewBag.BankName = UserBankAccount.Bank;
            ViewBag.AccountStatus = UserBankAccount.IsReciever;
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
                    PendingIncomingDonations.Add(set);
                }
            }
            ViewBag.PendingIncomingDonations = PendingIncomingDonations;

            //Pending Outgoing Donation View
            DonationDetails DonationDetails = new DonationDetails();
            if (PendingOutgoingDonation != null)
            {
                DonationDetails.DonationId = PendingOutgoingDonation.Id;
                DonationDetails.RecipientFullName = PendingOutgoingDonation.Ticket.TicketHolder.AccountTitle;
                DonationDetails.RecipientAccountNumber = PendingOutgoingDonation.Ticket.TicketHolder.AccountNumber;
                DonationDetails.DonationSetupDate = PendingOutgoingDonation.CreationDate;
            }
            if (PendingOutgoingPayment != null)
            {
                DonationDetails.PaymentId = PendingOutgoingPayment.Id;
                DonationDetails.PaymentStatus = PendingOutgoingPayment.Confirmed;
                DonationDetails.PaymentDate = PendingOutgoingPayment.CreationDate;
            }
            ViewBag.PendingOutgoingDonation = DonationDetails;

            //Pending Outgoing Payment Information
            ViewBag.PendingOutgoingPayment = PendingOutgoingPayment;

            return View("HomePage");
        }
    }

}
