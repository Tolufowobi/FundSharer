using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class WaitingTicket : BaseEntity
    {

        public WaitingTicket() : base()
        { }

        public virtual BankAccount TicketHolder { get; set; }

        public DateTime EntryDate { get; set; }

        public virtual List<Donation> Donations { get; set; }
    }
}