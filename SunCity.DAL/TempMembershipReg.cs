using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunCity.DAL
{
    public partial class TempMembershipReg
    {
        SuncityDataContext context = new SuncityDataContext();
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

    }
}
