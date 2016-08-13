using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunCity.DAL
{
    public partial class TempMembershipPlan
    {
        SuncityDataContext context = new SuncityDataContext();
        public List<TblUserType> templistUsertype { get; set; }

        public List<TblUserType> tempgetListUsertype()
        {

            List<TblUserType> tempobjlistUserType = (from u in context.TblUserTypes
                                                                  select u).ToList();
            return tempobjlistUserType;
        }


    }
}
