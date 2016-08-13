using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class TempMembershipDetail
    {
        public int TempPlanId { get; set; }
        public string TempPlanName { get; set; }
        public int TempPeriod { get; set; }
        public decimal TempAmount { get; set; }
        public string TempMembershipCode { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<BindCounter> ListBindCounter { get; set; }
    }

    public class BindCounter {
        public int UserTypeId { get; set; }
        public string UserType { get; set; }
        public int UserCount { get; set; }
        public int MembershipPlanId { get; set; }
    
    }
}