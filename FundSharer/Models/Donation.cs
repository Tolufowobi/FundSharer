using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class Donation : BaseEntity
    {

        public Donation() : base()
        { }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        public bool IsOpen { get; set; }

        public int DonorId { get; set; }
        [Required]
        public virtual BankAccount Donor { get; set; }


        public String TicketId { get; set; }
        [Required]
        public virtual WaitingTicket Ticket { get; set; }

    }
}