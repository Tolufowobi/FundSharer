
using System;
using System.ComponentModel.DataAnnotations;

namespace FundSharer.Models
{
    public class BankAccount : BaseEntity
    {
        public BankAccount() : base()
        { }

        [Required]
        [Display(Name = "Account Title")]
        public string AccountTitle { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public string Bank { get; set; }

        [Required]
        [Display(Name = ("Account Number"))]
        public string AccountNumber { get; set; }

        [Required]
        public string OwnerId { get; set; }       
        public ApplicationUser Owner { get; set; }

        [Display(Name = "Can Recieve Donations")]
        public bool IsReciever { get; set; }
    }
}