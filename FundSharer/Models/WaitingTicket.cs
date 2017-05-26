using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class WaitingTicket : BaseEntity
    {

        public WaitingTicket() : base()
        {
            Donations = new List<Donation>();
        }

        [Required]
        public string TicketHolderId { get; set; }
      
        public BankAccount TicketHolder { get; set; }

        public DateTime EntryDate { get; set; }

        public virtual List<Donation> Donations { get; set; }
    }
}