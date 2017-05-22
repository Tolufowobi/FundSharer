using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class PaymentDetails
    {
        public string DonationId { get; set; }

        [Display(Name = "From")]
        public string DonorFullName { get; set; }

        public string DonorAccountNumber { get; set; }

        [Display(Name = "Bank")]
        public string DonorBankName { get; set; }

        [Display(Name = "To")]
        public string RecipientFullName { get; set; }

        public string RecipientAccountNumber { get; set; }

        [Display(Name = "Bank")]
        public string RecipientBankName { get; set; }

        [Display(Name = "Amount Paid")]
        public decimal Amount { get; set; }

    }
}