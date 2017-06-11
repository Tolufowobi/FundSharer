using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class DonationDetails
    {
        public string DonationId { get; set; }

        [Display(Name = "From")]
        public string DonorFullName { get; set; }

        [Display(Name = "Donor Account Number")]
        public string DonorAccountNumber { get; set; }

        [Display(Name = "Donor Bank")]
        public string DonorBankName { get; set; }

        [Display(Name = "To")]
        public string RecipientFullName { get; set; }

        [Display(Name = "Recipient Account Number")]
        public string RecipientAccountNumber { get; set; }

        [Display(Name = "Recipient Bank")]
        public string RecipientBankName { get; set; }

        [Display(Name= "Donation Creation Date")]
        public DateTime DonationSetupDate { get; set; }

        [Display(Name = "Donation Deadline")]
        public DateTime DonationDeadline { get; set; }

        public string PaymentId { get; set; }

        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Confirmed")]
        public bool PaymentStatus { get; set; }

        [Display(Name = "Amount Paid")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

    }
    
}