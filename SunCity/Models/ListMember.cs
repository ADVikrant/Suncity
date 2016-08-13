using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class ListMember
    {
        public int TempMemberId { get; set; }
        public int TempMembershipRegistrationId { get; set; }
        public int UserTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlocked { get; set; }
        public string fname { get; set; }
        public string mname { get; set; }
        public string sname { get; set; }
        public string dob { get; set; }
        public string phone { get; set; }
    }
}