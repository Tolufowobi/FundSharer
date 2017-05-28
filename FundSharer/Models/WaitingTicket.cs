using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class WaitingTicket : BaseEntity
    {

        public WaitingTicket() : base()
        {
        }
        
        public string TicketHolderId { get; set; }
        [ForeignKey("TicketHolderId")]
        public virtual BankAccount TicketHolder { get; set; }

        public DateTime EntryDate { get; set; }

        public virtual ICollection<Donation> Donations
        { get; set; }

    }
}