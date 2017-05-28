using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class POPImage : BaseEntity
    {

        public POPImage() : base()
        { }

        public byte[] Image { get; set; }

        public string PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }
    }
}