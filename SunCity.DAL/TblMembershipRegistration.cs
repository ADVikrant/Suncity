using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunCity.DAL
{
    public partial class TblMembershipRegistration
    {
        SuncityDataContext context = new SuncityDataContext();
        public List<TblMembershipPlan> listMembershipPlan { get; set; }

        public List<TblMembershipPlan> getListMembershipPlan()
        {

            List<TblMembershipPlan> objlistMembershipPlan = (from u in context.TblMembershipPlans where u.IsDeleted== false
                                     select u).ToList();
            return objlistMembershipPlan;
        }

        

        public List<TblPackage> listPackages { get; set; }

        public List<TblPackage> getListPackage()
        {

            List<TblPackage> objlistPackage = (from u in context.TblPackages where u.IsDeleted == false
                                                             select u).ToList();
            return objlistPackage;
        }

        public List<TblMember> listmembers { get; set; }
        public List<TblDocument> listdocuments { get; set; }
    }
}
