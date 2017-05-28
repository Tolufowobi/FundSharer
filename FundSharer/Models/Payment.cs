using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Display(Name = "Entry Date")]
        public DateTime CreationDate { get; set; }

        public bool Confirmed { get; set; }

        public String DonationPackId { get; set; }
        [ForeignKey("DonationPackId")]
        public virtual Donation DonationPack { get; set; }
    }
}