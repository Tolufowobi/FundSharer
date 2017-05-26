using System;
using System.Collections.Generic;
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
        public virtual Payment Payment { get; set; }
    }
}