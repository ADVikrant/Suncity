using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunCity.DAL;
namespace SunCity.DAL
{
    
    public partial class TblUser
    {
        SuncityDataContext context = new SuncityDataContext();
        public List<TblRole> listRole { get; set; }

        public List<TblRole> getListRole()
        {

            List<TblRole> objlist = (from u in context.TblRoles
                                      select u).ToList();
            return objlist;
        }

        //public List<Department> getListDepartment()
        //{
        //    ProprintDBDataContext context = new ProprintDBDataContext();
        //    List<Department> objlist = (from u in context.Departments
        //                                where u.IsDeleted == false
        //                                select u).ToList();
        //    return objlist;
        //}
    }
}
