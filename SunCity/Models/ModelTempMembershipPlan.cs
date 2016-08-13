using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class ModelTempMembershipPlan
    {
        public int TempPlanId { get; set; }
        public string TempPlanName { get; set; }
        public int TempPeriod { get; set; }
        public decimal TempAmount { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<MapPlanUserType> Mapplanusertypelist { get; set; }
        public List<MapFieldUser> Mapfielduser { get; set; }

    }

    public class MapPlanUserType {

        public int MapPlanUserTypeId { get; set; }
        public int TempPlanId { get; set; }
        public int UserTypeId { get; set; }
        public int Usercount { get; set; }
    }
    public class MapFieldUser {
        public int fieldid { get; set; }
        public string FieldName { get; set; }
        public string FieldText { get; set; }
        public string FieldType { get; set; }
        public int UserTypeId { get; set; }
    }
}