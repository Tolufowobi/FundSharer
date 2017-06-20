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

        [Display(Name = "Status")]
        public bool Status { get; set; }

    }
    
    public class BankAccountDetails
    {
        public string Id { get; set; }

        [Display(Name="Account Title")]
        public string AccountTitle { get; set; }

        [Display (Name="Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name= "Bank Name")]
        public string BankName { get; set; }
        
        private string _lastIncomingD;
        [Display(Name = "Last Incoming Payment")]
        [DataType(DataType.Date)]
        public string LastIncomingDonationDate
        {
            get { return _lastIncomingD ?? "-"; }
            set { _lastIncomingD = value; }
        }
        
        private string _lastOutgoingD;
        [Display(Name = "Last Outgoing Payment")]
        [DataType(DataType.Date)]
        public string LastOutgoingDonationDate
        {
            get { return _lastOutgoingD ?? "-"; }
            set { _lastOutgoingD = value; }
        }

        [Display(Name = "Total Payments Received")]
        public int IncomingPaymentsCount { get; set; }

        [Display(Name="Total Payments Sent")]
        public int OutgoingPaymentsCount { get; set; }

        [Display(Name="Number of Pending Incoming Donations")]
        public int PendingPaymentsCount { get; set; }
        
    }

    public class PaymentDetails
    {
        public string PaymentId { get; set; }

        [Display(Name = "From")]
        public string DonorName { get; set; }

        [Display(Name = "Paid To")]
        public string RecipientName { get; set; }

        [Display(Name = "Paid To")]
        public string RecipientAccountNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Date")]
        public string Date { get; set; }

        [Display(Name= "Payment Status")]
        public bool Status { get; set; }

        [Display(Name="Image")]
        public Byte[] POPimage { get; set; }
    }
}