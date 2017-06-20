using System;
using System.Collections.Generic;
using System.Linq;
using FundSharer.Models;
using System.Web;
using System.Web.Mvc;

namespace FundSharer.Controllers
{
    public class BankAccountsController : Controller
    {
        // GET: BankAccounts
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles="Administrator")]
        public PartialViewResult BankAccounts()
        {
            List<BankAccountDetails> BankAccountDetails = new List<BankAccountDetails>();
            List<BankAccount> BankAccounts = new List<BankAccount>();
            using (var db = new ApplicationDbContext())
            {
                BankAccounts = (from b in db.BankAccounts select b).ToList();
                foreach( BankAccount ba in BankAccounts)
                {
                    BankAccountDetails bd = new BankAccountDetails
                    {   Id= ba.Id,
                        AccountTitle = ba.AccountTitle,
                        AccountNumber = ba.AccountNumber,
                        BankName = ba.Bank
                    };
                    bd.IncomingPaymentsCount = (from p in db.Payments where p.DonationPack.Ticket.TicketHolderId == ba.Id && p.Confirmed == true select p).ToList().Count();
                    bd.OutgoingPaymentsCount = (from p2 in db.Payments where p2.DonationPack.DonorId == ba.Id && p2.Confirmed == true select p2).ToList().Count();
                    bd.LastIncomingDonationDate = (from p in db.Payments where p.DonationPack.Ticket.TicketHolderId == ba.Id && p.Confirmed == true orderby p.CreationDate descending select p.CreationDate).FirstOrDefault().ToShortDateString();
                    bd.LastOutgoingDonationDate = (from p2 in db.Payments where p2.DonationPack.DonorId == ba.Id && p2.Confirmed == true orderby p2.CreationDate descending select p2.CreationDate).FirstOrDefault().ToShortDateString();
                }
            }

            return PartialView("_BankAccounts", BankAccountDetails);
        }
        
    }
}