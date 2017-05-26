using FundSharer.Models;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace FundSharer.DataServices
{

    public class BankAccountServices
    {
        public static BankAccount GetBankAccountById(string AccountId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.BankAccounts.Find(AccountId);
            }
        }

        static public BankAccount GetUserBankAccount(ApplicationUser AUser)
        {
            BankAccount Acc = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Acc = (from ba in db.BankAccounts where ba.OwnerId == AUser.Id select ba).First();
            }
            return Acc;
        }

        static public void AddBankAccount(BankAccount NewAccount)
        {
            //Ensure that the bank account isn't null
            if (IsNotNull(NewAccount))
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //check that the specified account doesn't exist...
                    //bank accounts should have a unique id, and bank name and account number combination
                    int testCount = (from a in db.BankAccounts where a.Id == NewAccount.Id || (a.AccountNumber == NewAccount.AccountNumber && a.AccountTitle == NewAccount.AccountTitle) || a.Owner.Id == NewAccount.OwnerId select a.Id).Count();
                    if (testCount == 0)// if the account doesn't exist, create it
                    {
                        db.BankAccounts.Add(NewAccount);
                        db.SaveChanges();
                    }
                }
            }
        }

        static public void RemoveBankAccount(BankAccount DeletedAccount)
        {
            if (IsNotNull(DeletedAccount))
            {
                if (ExistInRecord(DeletedAccount))// if the record exists in the database, then deleted it
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.BankAccounts.Remove(DeletedAccount);
                        db.SaveChanges();
                    }
                }
            }
        }

        static public void UpdateBankAccount(BankAccount UpdatedAccount)
        {
            if (IsNotNull(UpdatedAccount))
            {
                if (ExistInRecord(UpdatedAccount))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Entry(UpdatedAccount).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        static public AdminAccount GetAdminBankAccount(string AdminAccountId)
        {
            AdminAccount AdmAc = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<AdminAccount> List = (from adm in db.AdminAccounts where adm.Id == AdminAccountId select adm).ToList();
                if (List.Count() > 0)
                {
                    AdmAc = List.First();
                }
            }
            return AdmAc;
        }
        public static List<BankAccount> GetBankAccounts()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.BankAccounts.ToList();
            }
        }

        #region Helpers
        public static bool IsNotNull(BankAccount Account)
        {
            if (Account != null)
            { return true; }
            else
            { return false; }
        }

        public static bool ExistInRecord(BankAccount Account)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var test = db.BankAccounts.Find(Account.Id);
                return IsNotNull(test);
            }
        }
        #endregion

    }
}