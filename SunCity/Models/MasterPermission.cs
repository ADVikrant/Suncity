using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SunCity.DAL;
namespace SunCity.Models
{
    public class MasterPermission
    {
        public List<TblRole> AllRole { get; set; }
        public List<PermissionList> Permission { get; set; }
    }
    public class PermissionList
    {
        public PermissionRecord AllPermission { get; set; }
        public List<PermissionRecord_Role_Mapping> PermissionRoleMap { get; set; }
    }
}