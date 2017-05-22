using System;
using System.ComponentModel.DataAnnotations;

namespace FundSharer.Models
{
    public class AdminAccount : BaseEntity
    {
        public AdminAccount() : base()
        { }

        [Required]
        [Display(Name = "Account Title")]
        public string AccountTitle { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public Decimal Balance { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public string Bank { get; set; }

        [Required]
        [Display(Name = ("Account Number"))]
        public string AccountNumber { get; set; }

        [Required]
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        [Display(Name = "Waiting for donors")]
        public bool IsReciever { get; set; }

        public int MatchCount { get; set; }
    }
}