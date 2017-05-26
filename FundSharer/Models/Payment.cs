using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class Payment : BaseEntity
    {
        public Payment() : base()
        {
            Amount = 50000;
        }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "")]
        public DateTime CreationDate { get; set; }

        public bool Confirmed { get; set; }

        public String DonationPackId { get; set; }
        public virtual Donation DonationPack { get; set; }
    }
}