using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunCity.Core
{
    public class Enum
    {
      
        public enum Rollname
        {
             SuperAdmin = 1, Admin = 2,  Manager = 3, Employee=4
        }

        
        public enum MembershipPlan
        { 
            Family_MemberShip_11_Years=1, Family_MemberShip_3_Years=2,Corporate_Plan=3,Single_Membership=4
        
        }

        public enum TempMembershipPlan
        { 
        Corporate_Membership_Package=4,Single_Male_Membership=6,Single_Female_Membership=7,Senior_Citizen_Membership=8,Family_Membership_Package=9,ThreeYear_Membership_Package=10
         //   Corporate_Membership_Package = 4, Single_Male_Membership = 5, Single_Female_Membership = 6, Senior_Citizen_Membership = 7, Family_Membership_Package = 8, ThreeYear_Membership_Package = 9

        }

        public enum Permission
        {
            //For Local
            //NewEnrollment = 1, ViewMembershipRegistration = 2, ViewMembership = 6, ViewMember = 7, BlockedMembers = 8, EditMember = 9, AddMembershipPlan = 10, ViewMembershipPlan = 11, EditMembershipPlan = 12, AddPackage = 13, ViewPackage = 14, EditPackage = 15, MapPackageActivity = 16, AddActivity = 17, ViewActivity = 18, AddUser = 19, ViewUser = 20, EditUser = 21, EditActivity = 22, AddRole = 26, ViewRoles = 27, EditRoles = 28, ManagePermission = 29, DeleteUser = 30, DeletePackage = 31, DeleteMembershipPlan = 32, DeleteMembership = 33, DeleteMember = 34, DeleteRole = 35, DeleteActivity = 36, NewFestival = 37, EditFestival = 38, ViewFestival = 39, DeleteFestival=40

            //For Live
            NewEnrollment = 1, ViewMembershipRegistration = 2, ViewMembership = 6, ViewMember = 7, BlockedMembers = 8, EditMember = 9, AddMembershipPlan = 10, ViewMembershipPlan = 11, EditMembershipPlan = 12, AddPackage = 13, ViewPackage = 14, EditPackage = 15, MapPackageActivity = 16, AddActivity = 17, ViewActivity = 18, AddUser = 19, ViewUser = 20, EditUser = 21, EditActivity = 22, AddRole = 26, ViewRoles = 27, EditRoles = 28, ManagePermission = 29, DeleteUser = 30, DeletePackage = 31, DeleteMembershipPlan = 32, DeleteMembership = 33, DeleteMember = 34, DeleteRole = 35, DeleteActivity = 36, NewFestival = 38, EditFestival = 1037, ViewFestival = 1038, DeleteFestival=1039
        }

        public enum UserType
        { 
            Main_Member=1, Spouse=2, Child=3, Senior=4
        }
    }

    public static class Keys
    {
      public static  string passwordkey = "App_Password";
    }

    
}
