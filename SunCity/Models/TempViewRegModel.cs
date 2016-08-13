using SunCity.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class TempViewRegModel
    {
        SuncityDataContext context = new SuncityDataContext();
        public List<TblMembershipPlan> listMembershipPlan { get; set; }

        public List<TblMembershipPlan> getListMembershipPlan()
        {

            List<TblMembershipPlan> objlistMembershipPlan = (from u in context.TblMembershipPlans
                                                             where u.IsDeleted == false
                                                             select u).ToList();
            return objlistMembershipPlan;
        }

        public List<TempMembershipPlan> templistMembershipPlan { get; set; }

        public List<TempMembershipPlan> tempgetListMembershipPlan()
        {

            List<TempMembershipPlan> tempobjlistMembershipPlan = (from u in context.TempMembershipPlans
                                                                  where u.IsDeleted == false
                                                                  select u).ToList();
            return tempobjlistMembershipPlan;
        }

        public List<TblPackage> listPackages { get; set; }

        public List<TblPackage> getListPackage()
        {

            List<TblPackage> objlistPackage = (from u in context.TblPackages
                                               where u.IsDeleted == false
                                               select u).ToList();
            return objlistPackage;
        }

        public List<TblMember> listmembers { get; set; }
        public List<TblDocument> listdocuments { get; set; }
        
        public List<TempMember> listtempmember { get; set; }
        public TempMembershipReg objtempreg { get; set; }
        public List<DeserializeMemberObj> lstdeserialize { get; set; }
        
        
        public class DeserializeMemberObj
        {
            public int MemberId { get; set; }
            public int UserTypeId { get; set; }
            public int TempregistrationId { get; set; }
            public List<KeyValue> debobj { get; set; }
            public bool Isdeleted { get; set; }
            public string ProfilePictreName { get; set; }
            public int profilePictureId { get; set; }
            public string BarcodeImage { get; set; }
            public string PinNo { get; set; }
            public List<Gender> listgender { get; set; }
            public List<MrgOption> listmarriage { get; set; }
        }
        public class MrgOption
        {
            public string mrgname { get; set; }
            public string mrgvalue { get; set; }
        }
        public class KeyValue
        {

            public string key { get; set; }
            public string value { get; set; }
        }

    }
}