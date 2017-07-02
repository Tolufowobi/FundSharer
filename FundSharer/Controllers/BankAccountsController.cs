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

        [Authorize(Roles ="Administrator")]
        public ActionResult Delete(string Id)
        {
            using (var db = new ApplicationDbContext())
            {
                BankAccount ac = (from a in db.BankAccounts where a.Id == Id select a).FirstOrDefault();
                if(ac != null)
                {
                    //retrieve any waiting ticket on and pending donation matches for the account and delete them
                    WaitingTicket tkt = (from t in db.WaitingList where t.TicketHolderId == ac.Id && t.IsValid == true select t).FirstOrDefault();
                    if(tkt.Donations.Count() > 0)
                    {
                        foreach(Donation d in tkt.Donations)
                        {
                            // delete the unserviced donations for the account's ticket
                            db.Donations.Remove(d);
                        }
                        // delete the account's ticket
                        db.WaitingList.Remove(tkt);
                    }
                    //delete the account
                    db.BankAccounts.Remove(ac);
                    db.SaveChanges();
                }
                return Json("done");
            }
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
                return PartialView("_BankAccountDetails", bd);
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
                
                BankAccountDetails mdl = new BankAccountDetails() { AccountTitle = acct.AccountTitle, AccountNumber = acct.AccountNumber, BankName = acct.Bank, Id=acct.Id };
            return PartialView("_Edit", mdl);
        }

        [HttpPost]
        public PartialViewResult Edit(BankAccountDetails values)
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
                        BankAccountDetails bd = new BankAccountDetails() { AccountTitle = ba.AccountTitle, AccountNumber = ba.AccountNumber, BankName = ba.Bank };
                        return PartialView("_BankAccountDetails", bd);
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

        [Authorize(Roles="Administrator")]
        public ActionResult AdminAccounts()
        {
            var uid = User.Identity.GetUserId();
            List<BankAccountDetails> AdminAccounts = new List<BankAccountDetails>();
            using (var db = new ApplicationDbContext())
            {
                var AdmAcc = (from b in db.BankAccounts where b.OwnerId == uid select b).ToList();
                if (AdmAcc.Count > 0)
                {
                    foreach (BankAccount b in AdmAcc)
                    {
                        BankAccountDetails bd = CreateBankAccountDetails(b);
                        bd.PendingPaymentsCount = (from p in db.Payments where b.Id == p.DonationPack.Ticket.TicketHolderId && p.Confirmed == false select p).Count();
                        AdminAccounts.Add(bd);
                    }
                } 
            }
            return PartialView("_AdminAccounts", AdminAccounts);
        }

        [Authorize(Roles="Administrator")]
        public ActionResult AdminDonations(string Id)
        {
            List<Dictionary<string, object>> Pds = new List<Dictionary<string, object>>();
            using (var db = new ApplicationDbContext())
            {
                var acct = db.BankAccounts.Find(Id);
                if (acct != null)
                {
                    var Ps = DataServices.PaymentServices.GetIncomingPaymentsForAccount(acct);
                    foreach(Payment p in Ps)
                    {
                        if (p.Confirmed==false)
                        {
                            Dictionary<string, object> pd = new Dictionary<string, object>();
                            pd.Add("Name", p.DonationPack.Donor.AccountTitle);
                            pd.Add("AccountNumber", p.DonationPack.Donor.AccountNumber);
                            pd.Add("BankName", p.DonationPack.Donor.Bank);
                            pd.Add("PaymentId", p.Id);
                            pd.Add("Confirmed", p.Confirmed);
                            Pds.Add(pd);
                        }
                        
                    }
                }
                
            }
            return PartialView("_AccountDonations", Pds);
        }

        [Authorize(Roles="Administrator")]
        public ActionResult AddNewAccount()
        {
            return PartialView("_NewAccount");
        }

        [HttpPost]
        [Authorize(Roles="Administrator")]
        public ActionResult AddNewAccount(BankAccountDetails NewAccount)
        {
            using (var db = new ApplicationDbContext())
            {
                var testaccount = (from a in db.BankAccounts where a.AccountTitle == NewAccount.AccountTitle && a.AccountNumber == NewAccount.AccountNumber && a.Bank == NewAccount.BankName select a).Count();
                if(testaccount == 0)
                {
                    var uid = User.Identity.GetUserId();
                    BankAccount B = new BankAccount() { AccountNumber = NewAccount.AccountNumber, AccountTitle = NewAccount.AccountTitle, Bank = NewAccount.BankName, OwnerId = uid };
                    db.BankAccounts.Add(B);
                    db.SaveChanges();
                }
            } 
            return Json("_NewAccount");
        }

        [Authorize(Roles="Administrator")]
        public JsonResult Activate(String Id)
        {
            BankAccountDetails bd = null;
            using (var db = new ApplicationDbContext())
            {
                var acct = (from a in db.BankAccounts where a.Id == Id select a).FirstOrDefault();
                if (acct!= null && acct.IsReceiver == false)
                {
                    acct.IsReceiver = true;
                    //create an open ticket for the account
                    WaitingTicket adminTicket = new WaitingTicket { EntryDate = DateTime.Now, IsValid = true, TicketHolderId = acct.Id };
                    db.WaitingList.Add(adminTicket);
                    db.SaveChanges();
                     bd = CreateBankAccountDetails(acct);
                }
            }
            return Json("Done");
        }


        [Authorize(Roles="Administrator")]
        public JsonResult Deactivate(string Id)
        {
            BankAccountDetails bd = null;
            using (var db = new ApplicationDbContext())
            {
                var acct = (from a in db.BankAccounts where a.Id == Id select a).FirstOrDefault();
                if (acct != null && acct.IsReceiver == true)
                {
                    acct.IsReceiver = false;
                    //remove any existing waiting tickets from the waiting ticket repository
                    WaitingTicket tckt = (from t in db.WaitingList where t.TicketHolderId == acct.Id && t.IsValid == true select t).FirstOrDefault();
                    if(tckt != null)
                    {
                        if(tckt.Donations != null)
                        {
                            foreach (Donation d in tckt.Donations)
                            {
                                db.Donations.Remove(d);
                            }
                            db.WaitingList.Remove(tckt);
                        }
                    }
                    db.SaveChanges();
                    bd = CreateBankAccountDetails(acct);
                }
            }
            return Json("Done");
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

        private BankAccountDetails CreateBankAccountDetails(BankAccount bankAccount)
        {
            BankAccountDetails bd = new BankAccountDetails { Id = bankAccount.Id, AccountNumber = bankAccount.AccountNumber, AccountTitle = bankAccount.AccountTitle, BankName = bankAccount.Bank, IsReceiver = bankAccount.IsReceiver };
            return bd;
        }
    }

}