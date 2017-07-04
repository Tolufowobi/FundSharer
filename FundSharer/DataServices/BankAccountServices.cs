using FundSharer.Models;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Data.Entity;

namespace FundSharer.DataServices
{

    public class BankAccountServices
    {
        public static BankAccount GetBankAccountById(string AccountId)
        {
           
                return DbAccessHandler.DbContext.BankAccounts.Find(AccountId);
            
        }

        static public BankAccount GetUserBankAccount(ApplicationUser AUser)
        {
            BankAccount Acc = (from ba in DbAccessHandler.DbContext.BankAccounts where ba.BankAccountOwnerId == AUser.Id select ba).FirstOrDefault();
            return Acc;
        }

        static public void AddBankAccount(BankAccount NewAccount)
        {
            //Ensure that the bank account isn't null
            if (IsNotNull(NewAccount))
            {
                    //check that the specified account doesn't exist...
                    //bank accounts should have a unique id, and bank name and account number combination
                    int testCount = (from a in DbAccessHandler.DbContext.BankAccounts where a.Id == NewAccount.Id || (a.AccountNumber == NewAccount.AccountNumber && a.AccountTitle == NewAccount.AccountTitle) || a.BankAccountOwner.Id == NewAccount.BankAccountOwnerId select a.Id).Count();
                    if (testCount == 0)// if the account doesn't exist, create it
                    {
                        //check that its referenced entity is not null and ensure that its state is defined as unchanged to 
                        //avoid record duplication
                        if (NewAccount.BankAccountOwner != null)
                        {
                        //DbAccessHandler.DbContext.Entry(NewAccount.Owner).State = EntityState.Unchanged;
                        DbAccessHandler.DbContext.BankAccounts.Add(NewAccount);
                        DbAccessHandler.DbContext.SaveChanges();
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
                    using (var db = DbAccessHandler.DbContext)
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
                    DbAccessHandler.DbContext.Entry(UpdatedAccount).State = System.Data.Entity.EntityState.Modified;
                    DbAccessHandler.DbContext.SaveChanges();
                    
                }
            }
        }
        
        public static List<BankAccount> GetBankAccounts()
        {
            DbAccessHandler.DbContext.BankAccounts.Load();
            return DbAccessHandler.DbContext.BankAccounts.ToList(); 
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
                var test = DbAccessHandler.DbContext.BankAccounts.Find(Account.Id);
                return IsNotNull(test);
        }
        #endregion

    }
}