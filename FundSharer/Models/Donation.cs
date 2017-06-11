using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        public bool IsOpen { get; set; }
        
        public String DonorId { get; set; }
        [ForeignKey("DonorId")]
        public virtual BankAccount Donor { get; set; }

        public String TicketId { get; set; }
        [ForeignKey("TicketId")]
        public virtual WaitingTicket Ticket { get; set; }

    }
}