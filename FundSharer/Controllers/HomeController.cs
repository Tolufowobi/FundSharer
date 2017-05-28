using FundSharer.DataServices;
using FundSharer.Models;
using Microsoft.AspNet.Identity;
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

        public ActionResult HomePage()
        {
            using (var Db = new ApplicationDbContext())
            {
                // Get data
                var UserId = User.Identity.GetUserId();

                var AppUser = Db.Users.Find(UserId);
                var UserBankAccount = BankAccountServices.GetUserBankAccount(AppUser);
                var IncomingPayments = PaymentServices.GetIncomingPaymentsForAccount(UserBankAccount);
                var OutgoingPayments = PaymentServices.GetOutgoingPaymentsForAccount(UserBankAccount);
                var IncomingDonations = DonationServices.GetIncomingAccountDonations(UserBankAccount);
                var OutgoingDonations = DonationServices.GetOutgoingAccountDonations(UserBankAccount);

                List<Donation> _pendingIncomingDonations = new List<Donation>(2);
                // IncomingDonations.Where(m => m.IsOpen == false).ToList();

                List<Payment> _pendingpayments = new List<Payment>(2);

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
                    _pendingIncomingDonations = IncomingDonations.Where(m => m.IsOpen == false).ToList();
                    _pendingpayments = IncomingPayments.Where(m => m.Confirmed == false).ToList().Where(p => _pendingIncomingDonations.Exists(d => d.Id == p.DonationPack.Id)).ToList();
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
                ViewBag.AccountDetails = UserBankAccount;

                //Outgoing Payments Information
                ViewBag.OutgoingPayments = OutgoingPayments.Where(m => m.Confirmed == true).ToList();

                //Incoming Payments Information
                ViewBag.IncomingPayments = IncomingPayments.Where(m => m.Confirmed == true).ToList();

                //Pending Donors Information
                ViewBag.PendingIncomingDonations = _pendingIncomingDonations;

                //Pending Payments Information
                ViewBag.PendingPayments = _pendingpayments;

                //Pending Outgoing Donation View
                ViewBag.PendingOutgoingDonation = PendingOutgoingDonation;

                //Pending Outgoing Payment Information
                ViewBag.PendingOutgoingPayment = PendingOutgoingPayment;
            }
            return View();
        }

        public ActionResult FindAMatch()
        {
            var usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = User.Identity.GetUserId();
            ApplicationUser Appuser = usermanager.Users.First(m => m.Id == userId);
            BankAccount donor = BankAccountServices.GetUserBankAccount(Appuser);
            WaitingTicket ticket = TicketServices.PopOutTopTicket();
            Donation NewDonation = null;

            if (TicketServices.IsNotNull(ticket))
            {
                NewDonation = new Donation
                {
                    Donor = donor,
                    IsOpen = false,
                    TicketId = ticket.Id,
                    CreationDate = DateTime.Now
                };
            }
            ViewBag.Donation = NewDonation;
            return PartialView("_FindAMatch");
        }


    }

}
