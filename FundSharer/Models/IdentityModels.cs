using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations;
using System;

namespace FundSharer.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            return userIdentity;
        }

        [Required(ErrorMessage = "Oops... we need your First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Oops... we need your last name")]
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public bool ? IsLocked { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Login Date")]
        public DateTime LastLoginDate {get; set;}
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        { 
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<POPImage> POPImages { get; set; }
        public DbSet<WaitingTicket> WaitingList { get; set; }
        public DbSet<AdminAccount> AdminAccounts { get; set; }
        public DbSet<RevenueTarget> IncomeTarget { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

                throw ex;
            }

        }
        
    }
}