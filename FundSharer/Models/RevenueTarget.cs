using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FundSharer.Models
{
    public class RevenueTarget : BaseEntity
    {
        public RevenueTarget() : base()
        { }

        public Decimal Target { get; set; }

        public Decimal Income { get; set; }

    }
}