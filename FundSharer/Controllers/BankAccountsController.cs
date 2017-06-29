using System;
using System.Collections.Generic;
using System.Linq;
using FundSharer.Models;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace FundSharer.Controllers
{
    [Authorize]
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

            var BankAccountList = getBankAccountsDetails();
            return PartialView("_BankAccounts", BankAccountList);
        }
        
        public ActionResult Details(string Id)
        {
            if(Id != null)
            {
                BankAccountDetails bd = null;
                using (var db = new ApplicationDbContext())
                {
                    var ba = (from b in db.BankAccounts where b.Id == Id select b).FirstOrDefault();
                    bd = new BankAccountDetails()
                    {
                        AccountNumber = ba.AccountNumber,
                        AccountTitle = ba.AccountTitle,
                        BankName = ba.Bank,
                        Id = ba.Id
                    };
                    if (User.IsInRole("Administrator"))
                    {
                        bd.IncomingPaymentsCount = (from p in db.Payments where p.DonationPack.Ticket.TicketHolderId == ba.Id && p.Confirmed == true select p).ToList().Count();
                        bd.OutgoingPaymentsCount = (from p2 in db.Payments where p2.DonationPack.DonorId == ba.Id && p2.Confirmed == true select p2).ToList().Count();
                        bd.LastIncomingDonationDate = (from p in db.Payments where p.DonationPack.Ticket.TicketHolderId == ba.Id && p.Confirmed == true orderby p.CreationDate descending select p.CreationDate).FirstOrDefault().ToShortDateString();
                        bd.LastOutgoingDonationDate = (from p2 in db.Payments where p2.DonationPack.DonorId == ba.Id && p2.Confirmed == true orderby p2.CreationDate descending select p2.CreationDate).FirstOrDefault().ToShortDateString();

                    }
                }
                return PartialView("_AccountDetails", bd);
            }
            else
            {
                return HttpNotFound("Record not found");
            }
        }
        public PartialViewResult Edit()
        {
            var uid = User.Identity.GetUserId();
            var acct = new BankAccount();
            using (var db = new ApplicationDbContext())
            {
                acct = (from a in db.BankAccounts where a.OwnerId == uid select a).FirstOrDefault();
            }
                
                RegisterViewModel mdl = new RegisterViewModel() { AccountTitle = acct.AccountTitle, AccountNumber = acct.AccountNumber, BankName = acct.Bank };
            return PartialView("_Edit", mdl);
        }

        public PartialViewResult Edit(RegisterViewModel values)
        {
            if (values != null)
            {
                var uid = User.Identity.GetUserId();
                using (var db = new ApplicationDbContext())
                {
                    var ba = (from a in db.BankAccounts where a.OwnerId == uid select a).FirstOrDefault();
                    if (ba != null)
                    {
                        ba.AccountTitle = values.AccountTitle;
                        ba.AccountNumber = values.AccountNumber;
                        ba.Bank = values.BankName;
                        db.SaveChanges();
                        ViewBag.AccountTitle = ba.AccountTitle;
                        ViewBag.AccountNumber = ba.AccountNumber;
                        ViewBag.BankName = ba.Bank;
                        return PartialView("AccountInfo");
                    }
                    else
                    {
                        return PartialView("_Edit", values);
                    }
                }
            }
            else
            {
                return PartialView("_Edit", values);
            }
        }

        internal List<BankAccountDetails> getBankAccountsDetails()
        {
            List<BankAccountDetails> BankAccountList = new List<BankAccountDetails>();
            List<BankAccount> BankAccounts = new List<BankAccount>();
            using (var db = new ApplicationDbContext())
            {
                BankAccounts = (from b in db.BankAccounts select b).ToList();
                foreach (BankAccount ba in BankAccounts)
                {
                    BankAccountDetails bd = new BankAccountDetails
                    {
                        Id = ba.Id,
                        AccountTitle = ba.AccountTitle,
                        AccountNumber = ba.AccountNumber,
                        BankName = ba.Bank
                    };
                    bd.IncomingPaymentsCount = (from p in db.Payments where p.DonationPack.Ticket.TicketHolderId == ba.Id && p.Confirmed == true select p).ToList().Count();
                    bd.OutgoingPaymentsCount = (from p2 in db.Payments where p2.DonationPack.DonorId == ba.Id && p2.Confirmed == true select p2).ToList().Count();
                    bd.LastIncomingDonationDate = (from p in db.Payments where p.DonationPack.Ticket.TicketHolderId == ba.Id && p.Confirmed == true orderby p.CreationDate descending select p.CreationDate).FirstOrDefault().ToShortDateString();
                    bd.LastOutgoingDonationDate = (from p2 in db.Payments where p2.DonationPack.DonorId == ba.Id && p2.Confirmed == true orderby p2.CreationDate descending select p2.CreationDate).FirstOrDefault().ToShortDateString();
                    BankAccountList.Add(bd);
                }
            }
            return BankAccountList;
        }

    }

}