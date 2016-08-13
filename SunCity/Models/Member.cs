using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public string MemberMiddleName { get; set; }
        public DateTime MemberDOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string EmailId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlocked { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int MembershipRegistrationId { get; set; }
        public string MembershipPlan { get; set; }

        public List<Gender> listgender { get; set; }
        public string ProfilePictreName { get; set; }
        public int profilePictureId { get; set; }
        public int MembershipPlanId { get; set; }
        
    }

     

    public class Gender
    {
        public string gendername { get; set; }
        public string gendervalue { get; set; }
    
    }


}