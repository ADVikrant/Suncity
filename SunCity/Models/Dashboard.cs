using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class Dashboard
    {
        public int MembershipCounter { get; set; }
        public int MembersCounter { get; set; }
        public int UserCounter { get; set; }
        public int MembershipPlanCounter { get; set; }
        public int PackageCounter { get; set; }
        public int ActivityCounter { get; set; }
        public decimal AmountTillDate { get; set; }
    }
}