using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class MembershipRegistration
    {
        public int MembershipRegistrationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime MembershipStartDate { get; set; }
        public DateTime MembershipEndDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public string MembershipNo { get; set; }
    }
}