using System;
using System.ComponentModel.DataAnnotations;

namespace FundSharer.Models
{
    public class ConfirmPaymentViewModel
    {

        public string PaymentId { get; set; }

        [Display(Name = "From")]
        public string DonorName { get; set; }

        [Display(Name = "Paid To")]
        public string RecipientAccountNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Date")]
        public string Date { get; set; }

        public Byte[] POPimage { get; set; }
    }
}