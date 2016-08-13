using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SunCity.DAL;
using System.Configuration;
using SunCity.Core;
using SunCity.Models;
using System.IO;
namespace SunCity.Controllers
{
    public class FunctionController : Controller
    {
        //
        // GET: /Function/
        SuncityDataContext context = new SuncityDataContext();
        public ActionResult Index()
        {
            return View();
        }

        //----------------------------Start of Role Block-------------------------------------//
        /// <summary>
        /// Below action is used to set view for the AddRole
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public ActionResult AddRole(int RoleId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddRole)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblRole objrole = new TblRole();
            if (RoleId == 0)
            {
                objrole.IsDeleted = false;
                return View(objrole);
            }
            else
            {

                objrole = (from r in context.TblRoles where r.RoleId == RoleId select r).FirstOrDefault();
                if (objrole.RoleName != null && objrole.RoleName != "")
                {
                    objrole.RoleName = objrole.RoleName;
                    objrole.IsDeleted = objrole.IsDeleted;
                }
                return View(objrole);
            }

        }

        public ActionResult _GridRoles()
        {
            var getroles = (from r in context.TblRoles orderby r.RoleName select r).ToList();
            if (getroles != null)
            {
                return View(getroles);
            }
            else
            {
                return View();
            }
        }

        public ActionResult ViewRoles(int RoleId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewRoles)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblRole objrole = new TblRole();
            if (RoleId == 0)
            {
                objrole.IsDeleted = false;
                return View(objrole);
            }
            else
            {
                objrole = (from r in context.TblRoles where r.RoleId == RoleId select r).FirstOrDefault();
                if (objrole.RoleName != null && objrole.RoleName != "")
                {
                    objrole.RoleName = objrole.RoleName;
                    objrole.IsDeleted = objrole.IsDeleted;
                }
                return View(objrole);
            }

        }
        public ActionResult EditRoles(int RoleId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditRoles)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblRole objrole = new TblRole();
            if (RoleId == 0)
            {
                objrole.IsDeleted = false;
                return View(objrole);
            }
            else
            {

                objrole = (from r in context.TblRoles where r.RoleId == RoleId select r).FirstOrDefault();
                if (objrole.RoleName != null && objrole.RoleName != "")
                {
                    objrole.RoleName = objrole.RoleName;
                    objrole.IsDeleted = objrole.IsDeleted;
                }
                return View(objrole);
            }

        }
        [HttpPost]
        public ActionResult EditRoles(TblRole Objuserrole, int RoleId = 0)
        {
            try
            {
                //Cancel button clicked
                if (Request["hiddencancelflag"] != "0")
                {
                    return RedirectToAction("ViewRoles", "Function");
                }
                //Delete button handling
                if (Request["hiddendeleteflag"] != "0")
                {
                    TblRole objroldel = new TblRole();
                    if (RoleId != 0)
                    {
                        objroldel = (from k in context.TblRoles where k.RoleId == RoleId select k).FirstOrDefault();
                    }
                    if (objroldel != null)
                    {
                        objroldel.IsDeleted = true;
                    }
                    context.SubmitChanges();
                    TempData["deletestatus"] = "Success";
                    return RedirectToAction("ViewRoles", "Function");
                }

                TblRole objrole = new TblRole();
                if (RoleId != 0)
                {
                    objrole = (from k in context.TblRoles where k.RoleId == RoleId select k).FirstOrDefault();
                }

                objrole.RoleName = Objuserrole.RoleName;


                if (Request["IsDeleted"] == null)
                {
                    objrole.IsDeleted = false;
                }
                else
                {
                    objrole.IsDeleted = true;
                }

                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Role:" + objrole.RoleName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("ViewRoles", "Function", new { RoleId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }


        /// <summary>
        /// Function to Add Roles
        /// </summary>
        /// <param name="Objuserrole">Objuserrole</param>
        /// <param name="Id"></param>
        /// <returns>It redirects to the page and displays the list of role</returns>
        [HttpPost]
        public ActionResult AddRole(TblRole Objuserrole, int RoleId = 0)
        {
            try
            {
                TblRole objrole = new TblRole();
                if (RoleId != 0)
                {
                    objrole = (from k in context.TblRoles where k.RoleId == RoleId select k).FirstOrDefault();
                }

                objrole.RoleName = Objuserrole.RoleName;
                objrole.IsDeleted = false;

                if (RoleId == 0)
                {
                    objrole.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objrole.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TblRoles.InsertOnSubmit(objrole);
                }
                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Role:" + objrole.RoleName + " has been added by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("AddRole", "Function", new { RoleId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }
        /// <summary>
        /// Function to deactivate the role
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public ActionResult DeleteRole(int? RoleId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteRole)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblRole objrole = new TblRole();
            if (RoleId != 0)
            {
                objrole = (from k in context.TblRoles where k.RoleId == RoleId select k).FirstOrDefault();
            }
            objrole.IsDeleted = true;
            context.SubmitChanges();
            TempData["deletestatus"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Role:" + objrole.RoleName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("ViewRoles", "Function", new { RoleId = 0 });

        }

        //----------------------------End of Role Block-------------------------------------//

        //----------------------------XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX-------------------------------------//

        //----------------------------Start of Activity Block-------------------------------------//
        /// <summary>
        /// Function used to list all activities
        /// </summary>
        /// <returns></returns>
        public ActionResult _GridActivities()
        {
            var getroles = (from r in context.TblActivities orderby r.ActivityName select r).ToList();
            if (getroles != null)
            {
                return View(getroles);
            }
            else
            {
                return View();
            }
        }
        /// <summary>
        /// Function to view AddActivity page by passing on the required values
        /// </summary>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public ActionResult AddActivity(int ActivityId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddActivity)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblActivity objactivity = new TblActivity();
            if (ActivityId == 0)
            {
                objactivity.IsDeleted = false;
                return View(objactivity);
            }
            else
            {
                objactivity = (from r in context.TblActivities where r.ActivityId == ActivityId select r).FirstOrDefault();
                if (objactivity.ActivityName != null && objactivity.ActivityName != "")
                {
                    objactivity.ActivityName = objactivity.ActivityName;
                }
                return View(objactivity);
            }
        }
        /// <summary>
        /// Function to add activities in the suncity project.
        /// </summary>
        /// <param name="Frmactivity"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddActivity(TblActivity Frmactivity, int ActivityId = 0)
        {
            try
            {
                TblActivity objactivity = new TblActivity();
                if (ActivityId != 0)
                {
                    objactivity = (from k in context.TblActivities where k.ActivityId == ActivityId select k).FirstOrDefault();
                }

                objactivity.ActivityName = Frmactivity.ActivityName;


                if (Request["IsDeleted"] == null)
                {
                    objactivity.IsDeleted = false;
                }
                else
                {
                    objactivity.IsDeleted = true;
                }

                if (ActivityId == 0)
                {
                    objactivity.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objactivity.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TblActivities.InsertOnSubmit(objactivity);
                }
                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Activity:" + objactivity.ActivityName + " has been added by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("AddActivity", "Function", new { ActivityId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        public ActionResult ViewActivity(int ActivityId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewActivity)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblActivity objactivity = new TblActivity();
            if (ActivityId == 0)
            {
                objactivity.IsDeleted = false;
                return View(objactivity);
            }
            else
            {
                objactivity = (from r in context.TblActivities where r.ActivityId == ActivityId select r).FirstOrDefault();
                if (objactivity.ActivityName != null && objactivity.ActivityName != "")
                {
                    objactivity.ActivityName = objactivity.ActivityName;
                }
                return View(objactivity);
            }
        }

        /// <summary>
        /// Below function is used to get the details of activity which we want to edit. we need to pass the activity id for it.
        /// </summary>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public ActionResult EditActivity(int ActivityId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditActivity)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblActivity objactivity = new TblActivity();
            if (ActivityId == 0)
            {
                objactivity.IsDeleted = false;
                return View(objactivity);
            }
            else
            {
                objactivity = (from r in context.TblActivities where r.ActivityId == ActivityId select r).FirstOrDefault();
                if (objactivity.ActivityName != null && objactivity.ActivityName != "")
                {
                    objactivity.ActivityName = objactivity.ActivityName;
                }
                return View(objactivity);
            }
        }

        /// <summary>
        /// Function to edit activities in the system.
        /// </summary>
        /// <param name="Frmactivity"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditActivity(TblActivity Frmactivity, int ActivityId = 0)
        {
            try
            {
                //Cancel button clicked
                if (Request["hiddencancelflag"] != "0")
                {
                    return RedirectToAction("ViewActivity", "Function");
                }
                //Delete button handling
                if (Request["hiddendeleteflag"] != "0")
                {
                    TblActivity objactivitydel = new TblActivity();
                    if (ActivityId != 0)
                    {
                        objactivitydel = (from k in context.TblActivities where k.ActivityId == ActivityId select k).FirstOrDefault();
                    }
                    if (objactivitydel != null)
                    {
                        objactivitydel.IsDeleted = true;
                    }
                    context.SubmitChanges();
                    TempData["deletestatus"] = "Success";
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Activity:" + objactivitydel.ActivityName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    return RedirectToAction("ViewActivity", "Function");
                }

                TblActivity objactivity = new TblActivity();
                if (ActivityId != 0)
                {
                    objactivity = (from k in context.TblActivities where k.ActivityId == ActivityId select k).FirstOrDefault();
                }

                objactivity.ActivityName = Frmactivity.ActivityName;


                if (Request["IsDeleted"] == null)
                {
                    objactivity.IsDeleted = false;
                }
                else
                {
                    objactivity.IsDeleted = true;
                }

                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Activity:" + objactivity.ActivityName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("ViewActivity", "Function", new { ActivityId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        /// <summary>
        /// Below action is used to delete an activity in the system. We need to pass on the activity id.
        /// </summary>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public ActionResult DeleteActivity(int? ActivityId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteActivity)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblActivity objactivity = new TblActivity();
            if (ActivityId != 0)
            {
                objactivity = (from k in context.TblActivities where k.ActivityId == ActivityId select k).FirstOrDefault();
            }
            objactivity.IsDeleted = true;
            context.SubmitChanges();
            TempData["deletestatus"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Activity:" + objactivity.ActivityName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("ViewActivity", "Function", new { ActivityId = 0 });
        }

        //----------------------------End of Activity Block-------------------------------------//

        //----------------------------XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX-------------------------------------//

        //----------------------------Start of User block-------------------------------------//
        /// <summary>
        /// Below action is used to set the view for adding user.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult AddUser(int UserId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddUser)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblUser objgetuser = new TblUser();
            string passphrase = ConfigurationManager.AppSettings["PassPhrase"];
            if (UserId == 0)
            {
                objgetuser.listRole = objgetuser.getListRole();
                objgetuser.IsDeleted = false;
                return View(objgetuser);
            }
            else
            {
                objgetuser = (from i in context.TblUsers where i.UserId == UserId select i).FirstOrDefault();
                objgetuser.listRole = objgetuser.getListRole();
                if (objgetuser.Userpwd != null && objgetuser.Userpwd != "")
                {
                    objgetuser.Userpwd = UtilityFunction.DecryptString(objgetuser.Userpwd, passphrase);
                }
                return View(objgetuser);
            }

        }

        public ActionResult ViewUser(int UserId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewUser)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblUser objgetuser = new TblUser();
            string passphrase = ConfigurationManager.AppSettings["PassPhrase"];
            if (UserId == 0)
            {
                objgetuser.listRole = objgetuser.getListRole();
                objgetuser.IsDeleted = false;
                return View(objgetuser);
            }
            else
            {
                objgetuser = (from i in context.TblUsers where i.UserId == UserId select i).FirstOrDefault();
                objgetuser.listRole = objgetuser.getListRole();
                if (objgetuser.Userpwd != null && objgetuser.Userpwd != "")
                {
                    objgetuser.Userpwd = UtilityFunction.DecryptString(objgetuser.Userpwd, passphrase);
                }
                return View(objgetuser);
            }

        }

        /// <summary>
        /// Below action is used to get details of a user. we need to pass the Userid to get all the details
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult EditUser(int UserId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditUser)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblUser objgetuser = new TblUser();
            string passphrase = ConfigurationManager.AppSettings["PassPhrase"];
            if (UserId == 0)
            {
                objgetuser.listRole = objgetuser.getListRole();
                objgetuser.IsDeleted = false;
                return View(objgetuser);
            }
            else
            {
                objgetuser = (from i in context.TblUsers where i.UserId == UserId select i).FirstOrDefault();
                objgetuser.listRole = objgetuser.getListRole();
                if (objgetuser.Userpwd != null && objgetuser.Userpwd != "")
                {
                    objgetuser.Userpwd = UtilityFunction.DecryptString(objgetuser.Userpwd, passphrase);
                }
                return View(objgetuser);
            }

        }

        /// <summary>
        /// Below action will list all the users  which are there in the system.
        /// </summary>
        /// <returns></returns>
        public ActionResult _GridUsers()
        {
            var getuser = (from u in context.TblUsers orderby u.UserId descending select u).ToList();
            if (getuser != null)
            {
                return View(getuser);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Function to insert users to the table TblUser
        /// </summary>
        /// <param name="FrmUser"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUser(TblUser FrmUser, int UserId = 0)
        {
            try
            {
                TblUser objuser = new TblUser();
                string passphrase = ConfigurationManager.AppSettings["PassPhrase"];
                if (UserId != 0)
                {
                    objuser = (from k in context.TblUsers where k.UserId == UserId select k).FirstOrDefault();
                }

                objuser.FirstName = FrmUser.FirstName;
                objuser.MiddleName = FrmUser.MiddleName;
                objuser.LastName = FrmUser.LastName;
                objuser.RoleId = FrmUser.RoleId;
                objuser.Username = FrmUser.Username;
                objuser.Userpwd = UtilityFunction.EncryptData(FrmUser.Userpwd, passphrase);
                objuser.Phone = FrmUser.Phone;
                objuser.Email = FrmUser.Email;
                objuser.IsDeleted = false;
                if (UserId == 0)
                {
                    objuser.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objuser.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TblUsers.InsertOnSubmit(objuser);
                }
                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " User:" + objuser.FirstName + " " + objuser.LastName + " " + " has been added by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("AddUser", "Function", new { Id = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        /// <summary>
        /// Below action will edit the user. We need to pass the object and the userid.
        /// </summary>
        /// <param name="FrmUser"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditUser(TblUser FrmUser, int UserId = 0)
        {
            try
            {
                //Cancel button clicked
                if (Request["hiddencancelflag"] != "0")
                {
                    return RedirectToAction("ViewUser", "Function");
                }
                //Delete button handling
                if (Request["hiddendeleteflag"] != "0")
                {
                    TblUser objuserdel = new TblUser();

                    if (UserId != 0)
                    {
                        objuserdel = (from k in context.TblUsers where k.UserId == UserId select k).FirstOrDefault();
                    }
                    if (objuserdel != null)
                    {
                        objuserdel.IsDeleted = true;
                    }
                    context.SubmitChanges();
                    TempData["deletestatus"] = "Success";
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " User:" + objuserdel.FirstName + " " + objuserdel.LastName + " " + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    return RedirectToAction("ViewUser", "Function");
                }

                TblUser objuser = new TblUser();
                string passphrase = ConfigurationManager.AppSettings["PassPhrase"];
                if (UserId != 0)
                {
                    objuser = (from k in context.TblUsers where k.UserId == UserId select k).FirstOrDefault();
                }

                objuser.FirstName = FrmUser.FirstName;
                objuser.MiddleName = FrmUser.MiddleName;
                objuser.LastName = FrmUser.LastName;
                objuser.RoleId = FrmUser.RoleId;
                objuser.Username = FrmUser.Username;
                objuser.Userpwd = UtilityFunction.EncryptData(FrmUser.Userpwd, passphrase);
                objuser.Phone = FrmUser.Phone;
                objuser.Email = FrmUser.Email;
                if (Request["IsDeleted"] == null)
                {
                    objuser.IsDeleted = false;
                }
                else
                {
                    objuser.IsDeleted = true;
                }

                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " User:" + objuser.FirstName + " " + objuser.LastName + " " + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("ViewUser", "Function", new { Id = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        /// <summary>
        /// below action is used to delete the user. We need to pass the userid to be deleted.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult DeleteUser(int? UserId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteUser)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblUser objuser = new TblUser();
            if (UserId != 0)
            {
                objuser = (from k in context.TblUsers where k.UserId == UserId select k).FirstOrDefault();
            }
            objuser.IsDeleted = true;
            context.SubmitChanges();
            TempData["deletestatus"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " User:" + objuser.FirstName + " " + objuser.LastName + " " + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("ViewUser", "Function", new { UserId = 0 });
        }

        //----------------------------End of User Block-------------------------------------//

        //----------------------------XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX-------------------------------------//

        //----------------------------Add Package Block-------------------------------------//
        /// <summary>
        /// View to show add package.
        /// </summary>
        /// <param name="PackageId"></param>
        /// <returns></returns>
        public ActionResult AddPackage(int PackageId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddPackage)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblPackage objpackage = new TblPackage();
            if (PackageId == 0)
            {
                objpackage.IsDeleted = false;
                return View(objpackage);
            }
            else
            {
                objpackage = (from r in context.TblPackages where r.PackageId == PackageId select r).FirstOrDefault();
                if (objpackage.PackageName != null && objpackage.PackageName != "")
                {
                    objpackage.PackageName = objpackage.PackageName;
                }
                return View(objpackage);
            }

        }
        public ActionResult ViewPackage(int PackageId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewPackage)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblPackage objpackage = new TblPackage();
            if (PackageId == 0)
            {
                objpackage.IsDeleted = false;
                return View(objpackage);
            }
            else
            {
                objpackage = (from r in context.TblPackages where r.PackageId == PackageId select r).FirstOrDefault();
                if (objpackage.PackageName != null && objpackage.PackageName != "")
                {
                    objpackage.PackageName = objpackage.PackageName;
                }
                return View(objpackage);
            }

        }

        /// <summary>
        /// View to show the edit package. We need to pass the packageid
        /// </summary>
        /// <param name="PackageId"></param>
        /// <returns></returns>
        public ActionResult EditPackage(int PackageId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditPackage)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblPackage objpackage = new TblPackage();
            if (PackageId == 0)
            {
                objpackage.IsDeleted = false;
                return View(objpackage);
            }
            else
            {
                objpackage = (from r in context.TblPackages where r.PackageId == PackageId select r).FirstOrDefault();
                if (objpackage.PackageName != null && objpackage.PackageName != "")
                {
                    objpackage.PackageName = objpackage.PackageName;
                }
                return View(objpackage);
            }

        }

        /// <summary>
        /// Below action List down all the packages in the system
        /// </summary>
        /// <returns></returns>
        public ActionResult _GridPackages()
        {
            var getpackagelist = (from u in context.TblPackages orderby u.PackageId descending select u).ToList();
            if (getpackagelist != null)
            {
                return View(getpackagelist);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Below action is used to insert package to the table TblPackage
        /// </summary>
        /// <param name="FrmUser"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPackage(TblPackage FrmPackage, int PackageId = 0)
        {
            try
            {
                TblPackage objpackage = new TblPackage();
                if (PackageId != 0)
                {
                    objpackage = (from k in context.TblPackages where k.PackageId == PackageId select k).FirstOrDefault();
                }

                objpackage.PackageName = FrmPackage.PackageName;
                objpackage.IsDeleted = false;

                if (PackageId == 0)
                {
                    objpackage.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objpackage.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TblPackages.InsertOnSubmit(objpackage);
                }
                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = "Package:" + objpackage.PackageName + " has been added by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("AddPackage", "Function", new { PackageId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        /// <summary>
        /// below action is used to edit the package to the system. We need to pass the object FRmpackage which will store the updated values in the table. 
        /// </summary>
        /// <param name="FrmPackage">Object variable of TBLPAckage table</param>
        /// <param name="PackageId">Id of the package to be updated</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditPackage(TblPackage FrmPackage, int PackageId = 0)
        {
            try
            {
                //Cancel button clicked
                if (Request["hiddencancelflag"] != "0")
                {
                    return RedirectToAction("ViewPackage", "Function");
                }
                //Delete button handling
                if (Request["hiddendeleteflag"] != "0")
                {
                    TblPackage objpackagedel = new TblPackage();
                    if (PackageId != 0)
                    {
                        objpackagedel = (from k in context.TblPackages where k.PackageId == PackageId select k).FirstOrDefault();
                    }
                    if (objpackagedel != null)
                    {
                        objpackagedel.IsDeleted = true;
                    }
                    context.SubmitChanges();
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Package:" + objpackagedel.PackageName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    TempData["deletestatus"] = "Success";
                    return RedirectToAction("ViewPackage", "Function");
                }

                //Update button handling
                TblPackage objpackage = new TblPackage();
                if (PackageId != 0)
                {
                    objpackage = (from k in context.TblPackages where k.PackageId == PackageId select k).FirstOrDefault();
                }

                objpackage.PackageName = FrmPackage.PackageName;


                if (Request["IsDeleted"] == null)
                {
                    objpackage.IsDeleted = false;
                }
                else
                {
                    objpackage.IsDeleted = true;
                }

                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                return RedirectToAction("ViewPackage", "Function", new { PackageId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        /// <summary>
        /// Below action is used to delete a package in the system.
        /// </summary>
        /// <param name="PackageId">Need to pass the packageid</param>
        /// <returns></returns>
        public ActionResult DeletePackage(int? PackageId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeletePackage)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblPackage objpackage = new TblPackage();
            if (PackageId != 0)
            {
                objpackage = (from k in context.TblPackages where k.PackageId == PackageId select k).FirstOrDefault();
            }
            objpackage.IsDeleted = true;
            context.SubmitChanges();
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Package:" + objpackage.PackageName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            TempData["deletestatus"] = "Success";
            return RedirectToAction("ViewPackage", "Function", new { PackageId = 0 });
        }

        //----------------------------End of Package Block-------------------------------------//

        //----------------------------XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX-------------------------------------//

        //----------------------------Start of Membership Plan Block-------------------------------------//

        public ActionResult AddMembershipPlan(int MembershipPlanId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblMembershipPlan objmembershipplan = new TblMembershipPlan();
            if (MembershipPlanId == 0)
            {
                objmembershipplan.IsDeleted = false;
                return View(objmembershipplan);
            }
            else
            {
                objmembershipplan = (from r in context.TblMembershipPlans where r.MembershipPlanId == MembershipPlanId select r).FirstOrDefault();
                if (objmembershipplan.MembershipPlanName != null && objmembershipplan.MembershipPlanName != "")
                {
                    objmembershipplan.MembershipPlanName = objmembershipplan.MembershipPlanName;
                    objmembershipplan.MembershipPeriod = objmembershipplan.MembershipPeriod;
                }
                return View(objmembershipplan);
            }

        }
        public ActionResult ViewMembershipPlan(int MembershipPlanId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblMembershipPlan objmembershipplan = new TblMembershipPlan();
            if (MembershipPlanId == 0)
            {
                objmembershipplan.IsDeleted = false;
                return View(objmembershipplan);
            }
            else
            {
                objmembershipplan = (from r in context.TblMembershipPlans where r.MembershipPlanId == MembershipPlanId select r).FirstOrDefault();
                if (objmembershipplan.MembershipPlanName != null && objmembershipplan.MembershipPlanName != "")
                {
                    objmembershipplan.MembershipPlanName = objmembershipplan.MembershipPlanName;
                    objmembershipplan.MembershipPeriod = objmembershipplan.MembershipPeriod;
                }
                return View(objmembershipplan);
            }

        }
        public ActionResult EditMembershipPlan(int MembershipPlanId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblMembershipPlan objmembershipplan = new TblMembershipPlan();
            if (MembershipPlanId == 0)
            {
                objmembershipplan.IsDeleted = false;
                return View(objmembershipplan);
            }
            else
            {
                objmembershipplan = (from r in context.TblMembershipPlans where r.MembershipPlanId == MembershipPlanId select r).FirstOrDefault();
                if (objmembershipplan.MembershipPlanName != null && objmembershipplan.MembershipPlanName != "")
                {
                    objmembershipplan.MembershipPlanName = objmembershipplan.MembershipPlanName;
                    objmembershipplan.MembershipPeriod = objmembershipplan.MembershipPeriod;
                }
                return View(objmembershipplan);
            }

        }

        public ActionResult _GridMembershipPlans()
        {
            var getmembershipplanlist = (from u in context.TblMembershipPlans orderby u.MembershipPlanId descending select u).ToList();
            if (getmembershipplanlist != null)
            {
                return View(getmembershipplanlist);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Function to insert membrship plan to the table TblMembershipPlans
        /// </summary>
        /// <param name="FrmUser"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddMembershipPlan(TblMembershipPlan Frmmembershipplan, int MembershipPlanId = 0)
        {
            try
            {
                TblMembershipPlan objmembershipplan = new TblMembershipPlan();
                if (MembershipPlanId != 0)
                {
                    objmembershipplan = (from k in context.TblMembershipPlans where k.MembershipPlanId == MembershipPlanId select k).FirstOrDefault();
                }

                objmembershipplan.MembershipPlanName = Frmmembershipplan.MembershipPlanName;
                objmembershipplan.MembershipPeriod = Frmmembershipplan.MembershipPeriod;
                objmembershipplan.IsDeleted = false;
                objmembershipplan.Amount = Frmmembershipplan.Amount;
                objmembershipplan.NOFChilds = Frmmembershipplan.NOFChilds;
                objmembershipplan.NOFAdults = Frmmembershipplan.NOFAdults;
                objmembershipplan.NOFCouple = Frmmembershipplan.NOFCouple;

                if (MembershipPlanId == 0)
                {
                    objmembershipplan.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objmembershipplan.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TblMembershipPlans.InsertOnSubmit(objmembershipplan);
                }
                context.SubmitChanges();
                TempData["alertstatus"] = "Success";

                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Membership Plan:" + objmembershipplan.MembershipPlanName + " has been added by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("AddMembershipPlan", "Function", new { MembershipPlanId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditMembershipPlan(TblMembershipPlan Frmmembershipplan, int MembershipPlanId = 0)
        {
            try
            {
                //Cancel button clicked
                if (Request["hiddencancelflag"] != "0")
                {
                    return RedirectToAction("ViewMembershipPlan", "Function");
                }
                //Delete button handling
                if (Request["hiddendeleteflag"] != "0")
                {
                    TblMembershipPlan objmembershipplandel = new TblMembershipPlan();
                    if (MembershipPlanId != 0)
                    {
                        objmembershipplandel = (from k in context.TblMembershipPlans where k.MembershipPlanId == MembershipPlanId select k).FirstOrDefault();
                    }
                    if (objmembershipplandel != null)
                    {
                        objmembershipplandel.IsDeleted = true;
                    }
                    context.SubmitChanges();
                    TempData["deletestatus"] = "Success";
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Membership Plan:" + objmembershipplandel.MembershipPlanName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    return RedirectToAction("ViewMembershipPlan", "Function");
                }


                TblMembershipPlan objmembershipplan = new TblMembershipPlan();
                if (MembershipPlanId != 0)
                {
                    objmembershipplan = (from k in context.TblMembershipPlans where k.MembershipPlanId == MembershipPlanId select k).FirstOrDefault();
                }

                objmembershipplan.MembershipPlanName = Frmmembershipplan.MembershipPlanName;
                objmembershipplan.MembershipPeriod = Frmmembershipplan.MembershipPeriod;


                if (Request["IsDeleted"] == null)
                {
                    objmembershipplan.IsDeleted = false;
                }
                else
                {
                    objmembershipplan.IsDeleted = true;
                }


                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Membership Plan:" + objmembershipplan.MembershipPlanName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("ViewMembershipPlan", "Function", new { MembershipPlanId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        public ActionResult DeleteMembershipPlan(int? MembershipPlanId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblMembershipPlan objmembershipplan = new TblMembershipPlan();
            if (MembershipPlanId != 0)
            {
                objmembershipplan = (from k in context.TblMembershipPlans where k.MembershipPlanId == MembershipPlanId select k).FirstOrDefault();
            }
            objmembershipplan.IsDeleted = true;
            context.SubmitChanges();
            TempData["deletestatus"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Membership Plan:" + objmembershipplan.MembershipPlanName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("ViewMembershipPlan", "Function", new { MembershipPlanId = 0 });
        }


        //----------------------------End of Membership Plan Block-------------------------------------//

        //----------------------------XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX-------------------------------------//

        //----------------------------Start of Package-Activity Mapping Block-------------------------------------//
        /// <summary>
        /// Below action is used to get view of Package-Activity mapping. It also displays a grid showing activities currently binded to a package.
        /// </summary>
        /// <returns></returns>
        public ActionResult MapPackageActivity()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.MapPackageActivity)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            var objpkg = (from p in context.TblPackages where p.IsDeleted == false select p).ToList();
            if (objpkg != null)
            {
                return View(objpkg);
            }
            else { return View(); }
        }

        /// <summary>
        /// Below action is used to get all the activities which are binded to the current package.
        /// </summary>
        /// <param name="PackageId"></param>
        /// <returns></returns>
        public ActionResult getMappingDetails(int? PackageId)
        {
            List<PackageActivityList> listpackageactivity = new List<PackageActivityList>();
            PackageActivityList objpackageactivity = new PackageActivityList();
            var getactivitylist = (from i in context.TblActivities
                                   where i.IsDeleted == false
                                   select i).ToList();
            var getactivitycount = getactivitylist.Count();
            var getmappedactivity = (from o in context.TblMapPackageActivities where o.PackageId == PackageId select o).ToList();
            string details = "";
            foreach (var itemactivity in getactivitylist)
            {
                objpackageactivity = new PackageActivityList();
                objpackageactivity.ActivityName = itemactivity.ActivityName;
                objpackageactivity.ActivityId = itemactivity.ActivityId;
                objpackageactivity.PackageId = Convert.ToInt32(PackageId);
                //Check for activity and package in mappackagactivity table
                var checkmappkgact = (from ck in context.TblMapPackageActivities where ck.ActivityId == itemactivity.ActivityId && ck.PackageId == PackageId select ck).FirstOrDefault();
                if (checkmappkgact != null)
                {
                    objpackageactivity.MappedStatus = true;
                    objpackageactivity.CheckedStatus = "checked='checked'";
                }
                else
                {
                    objpackageactivity.MappedStatus = false;
                    objpackageactivity.CheckedStatus = "";
                }

                details += "<div class='checkbox block'><label><input type='checkbox' name='" + objpackageactivity.ActivityName + "' value='" + objpackageactivity.ActivityId + "' " + objpackageactivity.CheckedStatus + ">" + objpackageactivity.ActivityName + "</label></div>";
                listpackageactivity.Add(objpackageactivity);
            }


            return Content(details);
        }

        /// <summary>
        /// below action is used to update the listed activities to the particular package.
        /// </summary>
        /// <param name="objpak"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MapPackageActivity(TblMapPackageActivity objpak)
        {

            int PackageId = Convert.ToInt32(Request["PackageId"]);
            var getactivitylist = (from i in context.TblActivities select i).ToList();
            foreach (var item in getactivitylist)
            {
                TblMapPackageActivity objmap = new TblMapPackageActivity();
                if (Request[item.ActivityName] != null)
                {
                    //Check for the current activity and package value in the table
                    objmap = (from i in context.TblMapPackageActivities where i.PackageId == PackageId && i.ActivityId == Convert.ToInt32(Request[item.ActivityName]) select i).SingleOrDefault();
                    if (objmap == null)
                    {
                        objmap = new TblMapPackageActivity();
                        objmap.ActivityId = Convert.ToInt32(Request[item.ActivityName]);
                        objmap.PackageId = PackageId;
                        context.TblMapPackageActivities.InsertOnSubmit(objmap);
                    }
                }
                else
                {
                    objmap = (from i in context.TblMapPackageActivities where i.PackageId == PackageId && i.ActivityId == Convert.ToInt32(item.ActivityId) select i).SingleOrDefault();

                    if (objmap != null)
                    {
                        context.TblMapPackageActivities.DeleteOnSubmit(objmap);
                    }
                }

                context.SubmitChanges();


            }
            TempData["alertstatus"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Package-Activity mapping has been done by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("MapPackageActivity", "Function");
        }

        /// <summary>
        /// Below action displays the activities binded to the current package.
        /// </summary>
        /// <returns></returns>
        public ActionResult _GridPackageActivity()
        {
            List<TblPackage> lstpkg = new List<TblPackage>();
            TblPackage objpackact = new TblPackage();
            var getpackage = (from u in context.TblPackages where u.IsDeleted == false orderby u.PackageId descending select u).ToList();
            if (getpackage != null)
            {
                foreach (var item in getpackage)
                {
                    objpackact = new TblPackage();
                    var str = "";
                    objpackact.MapPackageId = item.PackageId;
                    objpackact.MapPackageName = item.PackageName;
                    var getactivitname = (from a in context.TblMapPackageActivities
                                          join b in context.TblActivities on a.ActivityId equals b.ActivityId
                                          where a.PackageId == item.PackageId
                                          select b).ToList();

                    for (int j = 0; j <= getactivitname.Count() - 1; j++)
                    {
                        if (j == getactivitname.Count() - 1)
                        {
                            str += getactivitname[j].ActivityName;
                        }
                        else
                        {
                            str += getactivitname[j].ActivityName + ", ";
                        }
                    }
                    if (getactivitname.Count() == 0)
                    {
                        objpackact.Activities = "--";
                    }
                    else
                    {
                        objpackact.Activities = str;
                    }

                    lstpkg.Add(objpackact);
                }
                return View(lstpkg);
            }
            else
            {
                return View();
            }
        }

        //----------------------------End of Package-Activity Mapping Block-------------------------------------//


        //----------------------------XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX-------------------------------------//


        //----------------------------Start of Membership Registration Block-------------------------------------//
        public ActionResult AddMembershipRegistration(int MembershipRegestrationId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.NewEnrollment)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblMembershipRegistration objgetregdetails = new TblMembershipRegistration();
            objgetregdetails.listMembershipPlan = objgetregdetails.getListMembershipPlan();
            objgetregdetails.listPackages = objgetregdetails.getListPackage();
            objgetregdetails.IsDeleted = false;
            return View(objgetregdetails);

        }

        public ActionResult Testm(int MembershipRegestrationId = 0)
        {
            TblMembershipRegistration objgetregdetails = new TblMembershipRegistration();
            objgetregdetails.listMembershipPlan = objgetregdetails.getListMembershipPlan();
            objgetregdetails.listPackages = objgetregdetails.getListPackage();
            objgetregdetails.IsDeleted = false;
            return View(objgetregdetails);

        }

        public ActionResult testupload(int MembershipRegestrationId = 0)
        {
            TblMembershipRegistration objgetregdetails = new TblMembershipRegistration();
            objgetregdetails.listMembershipPlan = objgetregdetails.getListMembershipPlan();
            objgetregdetails.listPackages = objgetregdetails.getListPackage();
            objgetregdetails.IsDeleted = false;
            return View(objgetregdetails);
        }

        [HttpPost]
        public ActionResult testupload(TblMembershipRegistration FrmMembershipRegistration, int MembershipRegestrationId = 0)
        {
            try
            {
                TblMembershipRegistration objmembershipregister = new TblMembershipRegistration();
                if (MembershipRegestrationId != 0)
                {
                    objmembershipregister = (from k in context.TblMembershipRegistrations where k.MembershipRegistrationId == Convert.ToInt32(MembershipRegestrationId) select k).FirstOrDefault();
                }

                objmembershipregister.MembershipPlanId = FrmMembershipRegistration.MembershipPlanId;
                objmembershipregister.PackageId = FrmMembershipRegistration.PackageId;
                objmembershipregister.Amount = FrmMembershipRegistration.Amount;
                DateTime stdt = Convert.ToDateTime(FrmMembershipRegistration.MembershipStartDate);
                objmembershipregister.MembershipStartDate = FrmMembershipRegistration.MembershipStartDate;

                var getmembershiplan = (from m in context.TblMembershipPlans where m.MembershipPlanId == FrmMembershipRegistration.MembershipPlanId select m).FirstOrDefault();
                objmembershipregister.MembershipEndDate = stdt.AddYears(Convert.ToInt32(getmembershiplan.MembershipPeriod));
                objmembershipregister.IsDeleted = false;
                if (MembershipRegestrationId == 0)
                {
                    objmembershipregister.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objmembershipregister.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TblMembershipRegistrations.InsertOnSubmit(objmembershipregister);
                }
                context.SubmitChanges();


                TblMember objmember = new TblMember();
                //Start process for entering members

                //Check for corporate Plan
                if (objmembershipregister.MembershipPlanId == Convert.ToInt32(SunCity.Core.Enum.MembershipPlan.Corporate_Plan))
                {
                    //Add Corporate Members here
                    if (Convert.ToString(Request["corpmem1firstname"]) != "" && Convert.ToString(Request["corpmem1middlename"]) != "" && Convert.ToString(Request["corpmem1lastname"]) != "")
                    {
                        //Enter Family-1 Member-1
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem1dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem1email"]);
                        objmember.Address = Convert.ToString(Request["corpmem1address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem1phone"]);
                        objmember.City = Convert.ToString(Request["corpmem1city"]);
                        objmember.State = Convert.ToString(Request["corpmem1state"]);
                        objmember.Country = Convert.ToString(Request["corpmem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem2firstname"]) != "" && Convert.ToString(Request["corpmem2middlename"]) != "" && Convert.ToString(Request["corpmem2lastname"]) != "")
                    {
                        //Enter Family-1 Member-2
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem2firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem2middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem2lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem2dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem2gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem2email"]);
                        objmember.Address = Convert.ToString(Request["corpmem2address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem2phone"]);
                        objmember.City = Convert.ToString(Request["corpmem2city"]);
                        objmember.State = Convert.ToString(Request["corpmem2state"]);
                        objmember.Country = Convert.ToString(Request["corpmem2country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem3firstname"]) != "" && Convert.ToString(Request["corpmem3middlename"]) != "" && Convert.ToString(Request["corpmem3lastname"]) != "")
                    {
                        //Enter Family-1 Member-3
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem3firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem3middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem3lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem3dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem3gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem3email"]);
                        objmember.Address = Convert.ToString(Request["corpmem3address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem3phone"]);
                        objmember.City = Convert.ToString(Request["corpmem3city"]);
                        objmember.State = Convert.ToString(Request["corpmem3state"]);
                        objmember.Country = Convert.ToString(Request["corpmem3country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem4firstname"]) != "" && Convert.ToString(Request["corpmem4middlename"]) != "" && Convert.ToString(Request["corpmem4lastname"]) != "")
                    {
                        //Enter Family-1 Member-4
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem4firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem4middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem4lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem4dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem4gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem4email"]);
                        objmember.Address = Convert.ToString(Request["corpmem4address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem4phone"]);
                        objmember.City = Convert.ToString(Request["corpmem4city"]);
                        objmember.State = Convert.ToString(Request["corpmem4state"]);
                        objmember.Country = Convert.ToString(Request["corpmem4country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem5firstname"]) != "" && Convert.ToString(Request["corpmem5middlename"]) != "" && Convert.ToString(Request["corpmem5lastname"]) != "")
                    {
                        //Enter Family-2 Member-1
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem5firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem5middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem5lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem5dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem5gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem5email"]);
                        objmember.Address = Convert.ToString(Request["corpmem5address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem5phone"]);
                        objmember.City = Convert.ToString(Request["corpmem5city"]);
                        objmember.State = Convert.ToString(Request["corpmem5state"]);
                        objmember.Country = Convert.ToString(Request["corpmem5country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem6firstname"]) != "" && Convert.ToString(Request["corpmem6middlename"]) != "" && Convert.ToString(Request["corpmem6lastname"]) != "")
                    {
                        //Enter Family-2 Member-2
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem6firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem6middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem6lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem6dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem6gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem6email"]);
                        objmember.Address = Convert.ToString(Request["corpmem6address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem6phone"]);
                        objmember.City = Convert.ToString(Request["corpmem6city"]);
                        objmember.State = Convert.ToString(Request["corpmem6state"]);
                        objmember.Country = Convert.ToString(Request["corpmem6country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                    }


                    if (Convert.ToString(Request["corpmem7firstname"]) != "" && Convert.ToString(Request["corpmem7middlename"]) != "" && Convert.ToString(Request["corpmem7lastname"]) != "")
                    {
                        //Enter Family-2 Member-3
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem7firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem7middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem7lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem7dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem7gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem7email"]);
                        objmember.Address = Convert.ToString(Request["corpmem7address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem7phone"]);
                        objmember.City = Convert.ToString(Request["corpmem7city"]);
                        objmember.State = Convert.ToString(Request["corpmem7state"]);
                        objmember.Country = Convert.ToString(Request["corpmem7country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem8firstname"]) != "" && Convert.ToString(Request["corpmem8middlename"]) != "" && Convert.ToString(Request["corpmem8lastname"]) != "")
                    {
                        //Enter Family-2 Member-4
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem8firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem8middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem8lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem8dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem8gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem8email"]);
                        objmember.Address = Convert.ToString(Request["corpmem8address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem8phone"]);
                        objmember.City = Convert.ToString(Request["corpmem8city"]);
                        objmember.State = Convert.ToString(Request["corpmem8state"]);
                        objmember.Country = Convert.ToString(Request["corpmem8country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                    }

                }
                else
                {
                    if (Convert.ToInt32(Request["Noppl"]) == 1)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddenmem1photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();

                    }
                    else if (Convert.ToInt32(Request["Noppl"]) == 2)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;
                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter Member#2 entry

                        TblMember objmember2 = new TblMember();
                        objmember2.MemberFirstName = Convert.ToString(Request["mem2firstname"]);
                        objmember2.MemberMiddleName = Convert.ToString(Request["mem2middlename"]);
                        objmember2.MemberLastName = Convert.ToString(Request["mem2lastname"]);
                        objmember2.MemberDOB = Convert.ToDateTime(Request["mem2dob"]);
                        objmember2.Gender = Convert.ToString(Request["mem2gen"]);
                        objmember2.EmailId = Convert.ToString(Request["mem2email"]);
                        objmember2.Address = Convert.ToString(Request["mem2address"]);
                        objmember2.Phone = Convert.ToString(Request["mem2phone"]);
                        objmember2.City = Convert.ToString(Request["mem2city"]);
                        objmember2.State = Convert.ToString(Request["mem2state"]);
                        objmember2.Country = Convert.ToString(Request["mem2country"]);
                        objmember2.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember2.IsDeleted = false;
                        objmember2.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember2);
                        context.SubmitChanges();
                    }
                    else if (Convert.ToInt32(Request["Noppl"]) == 3)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;
                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter Member#2 entry

                        TblMember objmember2 = new TblMember();
                        objmember2.MemberFirstName = Convert.ToString(Request["mem2firstname"]);
                        objmember2.MemberMiddleName = Convert.ToString(Request["mem2middlename"]);
                        objmember2.MemberLastName = Convert.ToString(Request["mem2lastname"]);
                        objmember2.MemberDOB = Convert.ToDateTime(Request["mem2dob"]);
                        objmember2.Gender = Convert.ToString(Request["mem2gen"]);
                        objmember2.EmailId = Convert.ToString(Request["mem2email"]);
                        objmember2.Address = Convert.ToString(Request["mem2address"]);
                        objmember2.Phone = Convert.ToString(Request["mem2phone"]);
                        objmember2.City = Convert.ToString(Request["mem2city"]);
                        objmember2.State = Convert.ToString(Request["mem2state"]);
                        objmember2.Country = Convert.ToString(Request["mem2country"]);
                        objmember2.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember2.IsDeleted = false;
                        objmember2.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember2);
                        context.SubmitChanges();


                        //Enter Member#3 entry

                        TblMember objmember3 = new TblMember();
                        objmember3.MemberFirstName = Convert.ToString(Request["mem3firstname"]);
                        objmember3.MemberMiddleName = Convert.ToString(Request["mem3middlename"]);
                        objmember3.MemberLastName = Convert.ToString(Request["mem3lastname"]);
                        objmember3.MemberDOB = Convert.ToDateTime(Request["mem3dob"]);
                        objmember3.Gender = Convert.ToString(Request["mem3gen"]);
                        objmember3.EmailId = Convert.ToString(Request["mem3email"]);
                        objmember3.Address = Convert.ToString(Request["mem3address"]);
                        objmember3.Phone = Convert.ToString(Request["mem3phone"]);
                        objmember3.City = Convert.ToString(Request["mem3city"]);
                        objmember3.State = Convert.ToString(Request["mem3state"]);
                        objmember3.Country = Convert.ToString(Request["mem3country"]);
                        objmember3.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember3.IsDeleted = false;
                        objmember3.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember3);
                        context.SubmitChanges();
                    }
                    else if (Convert.ToInt32(Request["Noppl"]) == 4)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;
                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter Member#2 entry

                        TblMember objmember2 = new TblMember();
                        objmember2.MemberFirstName = Convert.ToString(Request["mem2firstname"]);
                        objmember2.MemberMiddleName = Convert.ToString(Request["mem2middlename"]);
                        objmember2.MemberLastName = Convert.ToString(Request["mem2lastname"]);
                        objmember2.MemberDOB = Convert.ToDateTime(Request["mem2dob"]);
                        objmember2.Gender = Convert.ToString(Request["mem2gen"]);
                        objmember2.EmailId = Convert.ToString(Request["mem2email"]);
                        objmember2.Address = Convert.ToString(Request["mem2address"]);
                        objmember2.Phone = Convert.ToString(Request["mem2phone"]);
                        objmember2.City = Convert.ToString(Request["mem2city"]);
                        objmember2.State = Convert.ToString(Request["mem2state"]);
                        objmember2.Country = Convert.ToString(Request["mem2country"]);
                        objmember2.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember2.IsDeleted = false;
                        objmember2.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember2);
                        context.SubmitChanges();


                        //Enter Member#3 entry

                        TblMember objmember3 = new TblMember();
                        objmember3.MemberFirstName = Convert.ToString(Request["mem3firstname"]);
                        objmember3.MemberMiddleName = Convert.ToString(Request["mem3middlename"]);
                        objmember3.MemberLastName = Convert.ToString(Request["mem3lastname"]);
                        objmember3.MemberDOB = Convert.ToDateTime(Request["mem3dob"]);
                        objmember3.Gender = Convert.ToString(Request["mem3gen"]);
                        objmember3.EmailId = Convert.ToString(Request["mem3email"]);
                        objmember3.Address = Convert.ToString(Request["mem3address"]);
                        objmember3.Phone = Convert.ToString(Request["mem3phone"]);
                        objmember3.City = Convert.ToString(Request["mem3city"]);
                        objmember3.State = Convert.ToString(Request["mem3state"]);
                        objmember3.Country = Convert.ToString(Request["mem3country"]);
                        objmember3.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember3.IsDeleted = false;
                        objmember3.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember3);
                        context.SubmitChanges();

                        //Enter Member#4 entry

                        TblMember objmember4 = new TblMember();
                        objmember4.MemberFirstName = Convert.ToString(Request["mem4firstname"]);
                        objmember4.MemberMiddleName = Convert.ToString(Request["mem4middlename"]);
                        objmember4.MemberLastName = Convert.ToString(Request["mem4lastname"]);
                        objmember4.MemberDOB = Convert.ToDateTime(Request["mem4dob"]);
                        objmember4.Gender = Convert.ToString(Request["mem4gen"]);
                        objmember4.EmailId = Convert.ToString(Request["mem4email"]);
                        objmember4.Address = Convert.ToString(Request["mem4address"]);
                        objmember4.Phone = Convert.ToString(Request["mem4phone"]);
                        objmember4.City = Convert.ToString(Request["mem4city"]);
                        objmember4.State = Convert.ToString(Request["mem4state"]);
                        objmember4.Country = Convert.ToString(Request["mem4country"]);
                        objmember4.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember4.IsDeleted = false;
                        objmember4.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember4);
                        context.SubmitChanges();
                    }
                    else if (Convert.ToInt32(Request["Noppl"]) == 5)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;
                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter Member#2 entry

                        TblMember objmember2 = new TblMember();
                        objmember2.MemberFirstName = Convert.ToString(Request["mem2firstname"]);
                        objmember2.MemberMiddleName = Convert.ToString(Request["mem2middlename"]);
                        objmember2.MemberLastName = Convert.ToString(Request["mem2lastname"]);
                        objmember2.MemberDOB = Convert.ToDateTime(Request["mem2dob"]);
                        objmember2.Gender = Convert.ToString(Request["mem2gen"]);
                        objmember2.EmailId = Convert.ToString(Request["mem2email"]);
                        objmember2.Address = Convert.ToString(Request["mem2address"]);
                        objmember2.Phone = Convert.ToString(Request["mem2phone"]);
                        objmember2.City = Convert.ToString(Request["mem2city"]);
                        objmember2.State = Convert.ToString(Request["mem2state"]);
                        objmember2.Country = Convert.ToString(Request["mem2country"]);
                        objmember2.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember2.IsDeleted = false;
                        objmember2.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember2);
                        context.SubmitChanges();


                        //Enter Member#3 entry

                        TblMember objmember3 = new TblMember();
                        objmember3.MemberFirstName = Convert.ToString(Request["mem3firstname"]);
                        objmember3.MemberMiddleName = Convert.ToString(Request["mem3middlename"]);
                        objmember3.MemberLastName = Convert.ToString(Request["mem3lastname"]);
                        objmember3.MemberDOB = Convert.ToDateTime(Request["mem3dob"]);
                        objmember3.Gender = Convert.ToString(Request["mem3gen"]);
                        objmember3.EmailId = Convert.ToString(Request["mem3email"]);
                        objmember3.Address = Convert.ToString(Request["mem3address"]);
                        objmember3.Phone = Convert.ToString(Request["mem3phone"]);
                        objmember3.City = Convert.ToString(Request["mem3city"]);
                        objmember3.State = Convert.ToString(Request["mem3state"]);
                        objmember3.Country = Convert.ToString(Request["mem3country"]);
                        objmember3.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember3.IsDeleted = false;
                        objmember3.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember3);
                        context.SubmitChanges();

                        //Enter Member#4 entry

                        TblMember objmember4 = new TblMember();
                        objmember4.MemberFirstName = Convert.ToString(Request["mem4firstname"]);
                        objmember4.MemberMiddleName = Convert.ToString(Request["mem4middlename"]);
                        objmember4.MemberLastName = Convert.ToString(Request["mem4lastname"]);
                        objmember4.MemberDOB = Convert.ToDateTime(Request["mem4dob"]);
                        objmember4.Gender = Convert.ToString(Request["mem4gen"]);
                        objmember4.EmailId = Convert.ToString(Request["mem4email"]);
                        objmember4.Address = Convert.ToString(Request["mem4address"]);
                        objmember4.Phone = Convert.ToString(Request["mem4phone"]);
                        objmember4.City = Convert.ToString(Request["mem4city"]);
                        objmember4.State = Convert.ToString(Request["mem4state"]);
                        objmember4.Country = Convert.ToString(Request["mem4country"]);
                        objmember4.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember4.IsDeleted = false;
                        objmember4.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember4);
                        context.SubmitChanges();


                        //Enter Member#5 entry

                        TblMember objmember5 = new TblMember();
                        objmember5.MemberFirstName = Convert.ToString(Request["mem5firstname"]);
                        objmember5.MemberMiddleName = Convert.ToString(Request["mem5middlename"]);
                        objmember5.MemberLastName = Convert.ToString(Request["mem5lastname"]);
                        objmember5.MemberDOB = Convert.ToDateTime(Request["mem5dob"]);
                        objmember5.Gender = Convert.ToString(Request["mem5gen"]);
                        objmember5.EmailId = Convert.ToString(Request["mem5email"]);
                        objmember5.Address = Convert.ToString(Request["mem5address"]);
                        objmember5.Phone = Convert.ToString(Request["mem5phone"]);
                        objmember5.City = Convert.ToString(Request["mem5city"]);
                        objmember5.State = Convert.ToString(Request["mem5state"]);
                        objmember5.Country = Convert.ToString(Request["mem5country"]);
                        objmember5.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember5.IsDeleted = false;
                        objmember5.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember5);
                        context.SubmitChanges();

                        //Enter Member#6 entry

                        TblMember objmember6 = new TblMember();
                        objmember6.MemberFirstName = Convert.ToString(Request["mem6firstname"]);
                        objmember6.MemberMiddleName = Convert.ToString(Request["mem6middlename"]);
                        objmember6.MemberLastName = Convert.ToString(Request["mem6lastname"]);
                        objmember6.MemberDOB = Convert.ToDateTime(Request["mem6dob"]);
                        objmember6.Gender = Convert.ToString(Request["mem6gen"]);
                        objmember6.EmailId = Convert.ToString(Request["mem6email"]);
                        objmember6.Address = Convert.ToString(Request["mem6address"]);
                        objmember6.Phone = Convert.ToString(Request["mem6phone"]);
                        objmember6.City = Convert.ToString(Request["mem6city"]);
                        objmember6.State = Convert.ToString(Request["mem6state"]);
                        objmember6.Country = Convert.ToString(Request["mem6country"]);
                        objmember6.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember6.IsDeleted = false;
                        objmember6.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember6);
                        context.SubmitChanges();
                    }

                }

                TempData["alertstatus"] = "Success";
                return RedirectToAction("AddMembershipRegistration", "Function", new { MembershipRegestrationId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }

        }

        public string UploadImage(HttpPostedFileBase fileData)
        {
            if (fileData != null && fileData.ContentLength > 0)
            {
                string imageName = string.Empty;
                imageName = DateTime.UtcNow.AddHours(5.50).ToString() + "_" + fileData.FileName;
                imageName = imageName.Replace('/', '_');
                imageName = imageName.Replace('-', '_');
                imageName = imageName.Replace(':', '_');
                imageName = imageName.Replace(' ', '_');
                imageName = imageName.Replace(",", "");
                var currpath = Path.Combine(Server.MapPath("~/UserProfile/"), imageName);
                fileData.SaveAs(currpath);
                return imageName;
            }
            else
            {
                return string.Empty;
            }
        }

        [HttpPost]
        public ActionResult AddMembershipRegistration(TblMembershipRegistration FrmMembershipRegistration, int MembershipRegestrationId = 0)
        {
            try
            {
                TblMembershipRegistration objmembershipregister = new TblMembershipRegistration();
                if (MembershipRegestrationId != 0)
                {
                    objmembershipregister = (from k in context.TblMembershipRegistrations where k.MembershipRegistrationId == Convert.ToInt32(MembershipRegestrationId) select k).FirstOrDefault();
                }

                objmembershipregister.MembershipPlanId = FrmMembershipRegistration.MembershipPlanId;
                objmembershipregister.PackageId = FrmMembershipRegistration.PackageId;
                objmembershipregister.Amount = FrmMembershipRegistration.Amount;
                DateTime stdt = Convert.ToDateTime(FrmMembershipRegistration.MembershipStartDate);
                objmembershipregister.MembershipStartDate = FrmMembershipRegistration.MembershipStartDate;

                var getmembershiplan = (from m in context.TblMembershipPlans where m.MembershipPlanId == FrmMembershipRegistration.MembershipPlanId select m).FirstOrDefault();
                objmembershipregister.MembershipEndDate = stdt.AddYears(Convert.ToInt32(getmembershiplan.MembershipPeriod));
                objmembershipregister.IsDeleted = false;
                if (MembershipRegestrationId == 0)
                {
                    objmembershipregister.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objmembershipregister.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TblMembershipRegistrations.InsertOnSubmit(objmembershipregister);
                }
                context.SubmitChanges();

                //Add documents entry
                TblDocument objdocs = new TblDocument();
                string getdocs = Convert.ToString(Request["hiddendocs"]);
                string[] words = getdocs.Split(',');
                foreach (string word in words)
                {
                    if (word != "")
                    {
                        objdocs = new TblDocument();
                        objdocs.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objdocs.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        objdocs.DocumentName = word;
                        objdocs.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        context.TblDocuments.InsertOnSubmit(objdocs);
                        context.SubmitChanges();
                    }

                }



                TblMember objmember = new TblMember();
                //Start process for entering members

                //Check for corporate Plan
                if (objmembershipregister.MembershipPlanId == Convert.ToInt32(SunCity.Core.Enum.MembershipPlan.Corporate_Plan))
                {
                    //Add Corporate Members here
                    if (Convert.ToString(Request["corpmem1firstname"]) != "" && Convert.ToString(Request["corpmem1middlename"]) != "" && Convert.ToString(Request["corpmem1lastname"]) != "")
                    {
                        //Enter Family-1 Member-1
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem1dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem1email"]);
                        objmember.Address = Convert.ToString(Request["corpmem1address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem1phone"]);
                        objmember.City = Convert.ToString(Request["corpmem1city"]);
                        objmember.State = Convert.ToString(Request["corpmem1state"]);
                        objmember.Country = Convert.ToString(Request["corpmem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddencorpmem1photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem2firstname"]) != "" && Convert.ToString(Request["corpmem2middlename"]) != "" && Convert.ToString(Request["corpmem2lastname"]) != "")
                    {
                        //Enter Family-1 Member-2
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem2firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem2middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem2lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem2dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem2gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem2email"]);
                        objmember.Address = Convert.ToString(Request["corpmem2address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem2phone"]);
                        objmember.City = Convert.ToString(Request["corpmem2city"]);
                        objmember.State = Convert.ToString(Request["corpmem2state"]);
                        objmember.Country = Convert.ToString(Request["corpmem2country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddencorpmem2photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem3firstname"]) != "" && Convert.ToString(Request["corpmem3middlename"]) != "" && Convert.ToString(Request["corpmem3lastname"]) != "")
                    {
                        //Enter Family-1 Member-3
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem3firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem3middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem3lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem3dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem3gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem3email"]);
                        objmember.Address = Convert.ToString(Request["corpmem3address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem3phone"]);
                        objmember.City = Convert.ToString(Request["corpmem3city"]);
                        objmember.State = Convert.ToString(Request["corpmem3state"]);
                        objmember.Country = Convert.ToString(Request["corpmem3country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddencorpmem3photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem4firstname"]) != "" && Convert.ToString(Request["corpmem4middlename"]) != "" && Convert.ToString(Request["corpmem4lastname"]) != "")
                    {
                        //Enter Family-1 Member-4
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem4firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem4middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem4lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem4dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem4gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem4email"]);
                        objmember.Address = Convert.ToString(Request["corpmem4address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem4phone"]);
                        objmember.City = Convert.ToString(Request["corpmem4city"]);
                        objmember.State = Convert.ToString(Request["corpmem4state"]);
                        objmember.Country = Convert.ToString(Request["corpmem4country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddencorpmem4photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem5firstname"]) != "" && Convert.ToString(Request["corpmem5middlename"]) != "" && Convert.ToString(Request["corpmem5lastname"]) != "")
                    {
                        //Enter Family-2 Member-1
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem5firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem5middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem5lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem5dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem5gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem5email"]);
                        objmember.Address = Convert.ToString(Request["corpmem5address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem5phone"]);
                        objmember.City = Convert.ToString(Request["corpmem5city"]);
                        objmember.State = Convert.ToString(Request["corpmem5state"]);
                        objmember.Country = Convert.ToString(Request["corpmem5country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddencorpmem5photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem6firstname"]) != "" && Convert.ToString(Request["corpmem6middlename"]) != "" && Convert.ToString(Request["corpmem6lastname"]) != "")
                    {
                        //Enter Family-2 Member-2
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem6firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem6middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem6lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem6dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem6gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem6email"]);
                        objmember.Address = Convert.ToString(Request["corpmem6address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem6phone"]);
                        objmember.City = Convert.ToString(Request["corpmem6city"]);
                        objmember.State = Convert.ToString(Request["corpmem6state"]);
                        objmember.Country = Convert.ToString(Request["corpmem6country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddencorpmem6photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }


                    if (Convert.ToString(Request["corpmem7firstname"]) != "" && Convert.ToString(Request["corpmem7middlename"]) != "" && Convert.ToString(Request["corpmem7lastname"]) != "")
                    {
                        //Enter Family-2 Member-3
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem7firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem7middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem7lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem7dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem7gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem7email"]);
                        objmember.Address = Convert.ToString(Request["corpmem7address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem7phone"]);
                        objmember.City = Convert.ToString(Request["corpmem7city"]);
                        objmember.State = Convert.ToString(Request["corpmem7state"]);
                        objmember.Country = Convert.ToString(Request["corpmem7country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddencorpmem7photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }

                    if (Convert.ToString(Request["corpmem8firstname"]) != "" && Convert.ToString(Request["corpmem8middlename"]) != "" && Convert.ToString(Request["corpmem8lastname"]) != "")
                    {
                        //Enter Family-2 Member-4
                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["corpmem8firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["corpmem8middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["corpmem8lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["corpmem8dob"]);
                        objmember.Gender = Convert.ToString(Request["corpmem8gen"]);
                        objmember.EmailId = Convert.ToString(Request["corpmem8email"]);
                        objmember.Address = Convert.ToString(Request["corpmem8address"]);
                        objmember.Phone = Convert.ToString(Request["corpmem8phone"]);
                        objmember.City = Convert.ToString(Request["corpmem8city"]);
                        objmember.State = Convert.ToString(Request["corpmem8state"]);
                        objmember.Country = Convert.ToString(Request["corpmem8country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddencorpmem8photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }

                }
                else
                {
                    if (Convert.ToInt32(Request["Noppl"]) == 1)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;


                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddenmem1photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();
                    }
                    else if (Convert.ToInt32(Request["Noppl"]) == 2)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;
                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddenmem1photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();





                        //Enter Member#2 entry

                        TblMember objmember2 = new TblMember();
                        objmember2.MemberFirstName = Convert.ToString(Request["mem2firstname"]);
                        objmember2.MemberMiddleName = Convert.ToString(Request["mem2middlename"]);
                        objmember2.MemberLastName = Convert.ToString(Request["mem2lastname"]);
                        objmember2.MemberDOB = Convert.ToDateTime(Request["mem2dob"]);
                        objmember2.Gender = Convert.ToString(Request["mem2gen"]);
                        objmember2.EmailId = Convert.ToString(Request["mem2email"]);
                        objmember2.Address = Convert.ToString(Request["mem2address"]);
                        objmember2.Phone = Convert.ToString(Request["mem2phone"]);
                        objmember2.City = Convert.ToString(Request["mem2city"]);
                        objmember2.State = Convert.ToString(Request["mem2state"]);
                        objmember2.Country = Convert.ToString(Request["mem2country"]);
                        objmember2.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember2.IsDeleted = false;
                        objmember2.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember2);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic2 = new TblProfilePicture();
                        objpic2.ProfiePictureName = Request["hiddenmem2photo"].ToString();
                        objpic2.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic2.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic2);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic2 = new TblMapMemberProfilePicture();
                        objmappic2.MemberId = objmember2.MemberId;
                        objmappic2.ProfilePictureId = objpic2.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic2);
                        context.SubmitChanges();
                    }
                    else if (Convert.ToInt32(Request["Noppl"]) == 3)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;
                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddenmem1photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();

                        //Enter Member#2 entry

                        TblMember objmember2 = new TblMember();
                        objmember2.MemberFirstName = Convert.ToString(Request["mem2firstname"]);
                        objmember2.MemberMiddleName = Convert.ToString(Request["mem2middlename"]);
                        objmember2.MemberLastName = Convert.ToString(Request["mem2lastname"]);
                        objmember2.MemberDOB = Convert.ToDateTime(Request["mem2dob"]);
                        objmember2.Gender = Convert.ToString(Request["mem2gen"]);
                        objmember2.EmailId = Convert.ToString(Request["mem2email"]);
                        objmember2.Address = Convert.ToString(Request["mem2address"]);
                        objmember2.Phone = Convert.ToString(Request["mem2phone"]);
                        objmember2.City = Convert.ToString(Request["mem2city"]);
                        objmember2.State = Convert.ToString(Request["mem2state"]);
                        objmember2.Country = Convert.ToString(Request["mem2country"]);
                        objmember2.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember2.IsDeleted = false;
                        objmember2.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember2);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic2 = new TblProfilePicture();
                        objpic2.ProfiePictureName = Request["hiddenmem2photo"].ToString();
                        objpic2.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic2.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic2);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic2 = new TblMapMemberProfilePicture();
                        objmappic2.MemberId = objmember2.MemberId;
                        objmappic2.ProfilePictureId = objpic2.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic2);
                        context.SubmitChanges();


                        //Enter Member#3 entry

                        TblMember objmember3 = new TblMember();
                        objmember3.MemberFirstName = Convert.ToString(Request["mem3firstname"]);
                        objmember3.MemberMiddleName = Convert.ToString(Request["mem3middlename"]);
                        objmember3.MemberLastName = Convert.ToString(Request["mem3lastname"]);
                        objmember3.MemberDOB = Convert.ToDateTime(Request["mem3dob"]);
                        objmember3.Gender = Convert.ToString(Request["mem3gen"]);
                        objmember3.EmailId = Convert.ToString(Request["mem3email"]);
                        objmember3.Address = Convert.ToString(Request["mem3address"]);
                        objmember3.Phone = Convert.ToString(Request["mem3phone"]);
                        objmember3.City = Convert.ToString(Request["mem3city"]);
                        objmember3.State = Convert.ToString(Request["mem3state"]);
                        objmember3.Country = Convert.ToString(Request["mem3country"]);
                        objmember3.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember3.IsDeleted = false;
                        objmember3.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember3);
                        context.SubmitChanges();

                        //Enter the picture in picture table

                        TblProfilePicture objpic3 = new TblProfilePicture();
                        objpic3.ProfiePictureName = Request["hiddenmem3photo"].ToString();
                        objpic3.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic3.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic3);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic3 = new TblMapMemberProfilePicture();
                        objmappic3.MemberId = objmember3.MemberId;
                        objmappic3.ProfilePictureId = objpic3.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic3);
                        context.SubmitChanges();
                    }
                    else if (Convert.ToInt32(Request["Noppl"]) == 4)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;
                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddenmem1photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();

                        //Enter Member#2 entry

                        TblMember objmember2 = new TblMember();
                        objmember2.MemberFirstName = Convert.ToString(Request["mem2firstname"]);
                        objmember2.MemberMiddleName = Convert.ToString(Request["mem2middlename"]);
                        objmember2.MemberLastName = Convert.ToString(Request["mem2lastname"]);
                        objmember2.MemberDOB = Convert.ToDateTime(Request["mem2dob"]);
                        objmember2.Gender = Convert.ToString(Request["mem2gen"]);
                        objmember2.EmailId = Convert.ToString(Request["mem2email"]);
                        objmember2.Address = Convert.ToString(Request["mem2address"]);
                        objmember2.Phone = Convert.ToString(Request["mem2phone"]);
                        objmember2.City = Convert.ToString(Request["mem2city"]);
                        objmember2.State = Convert.ToString(Request["mem2state"]);
                        objmember2.Country = Convert.ToString(Request["mem2country"]);
                        objmember2.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember2.IsDeleted = false;
                        objmember2.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember2);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic2 = new TblProfilePicture();
                        objpic2.ProfiePictureName = Request["hiddenmem2photo"].ToString();
                        objpic2.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic2.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic2);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic2 = new TblMapMemberProfilePicture();
                        objmappic2.MemberId = objmember2.MemberId;
                        objmappic2.ProfilePictureId = objpic2.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic2);
                        context.SubmitChanges();


                        //Enter Member#3 entry

                        TblMember objmember3 = new TblMember();
                        objmember3.MemberFirstName = Convert.ToString(Request["mem3firstname"]);
                        objmember3.MemberMiddleName = Convert.ToString(Request["mem3middlename"]);
                        objmember3.MemberLastName = Convert.ToString(Request["mem3lastname"]);
                        objmember3.MemberDOB = Convert.ToDateTime(Request["mem3dob"]);
                        objmember3.Gender = Convert.ToString(Request["mem3gen"]);
                        objmember3.EmailId = Convert.ToString(Request["mem3email"]);
                        objmember3.Address = Convert.ToString(Request["mem3address"]);
                        objmember3.Phone = Convert.ToString(Request["mem3phone"]);
                        objmember3.City = Convert.ToString(Request["mem3city"]);
                        objmember3.State = Convert.ToString(Request["mem3state"]);
                        objmember3.Country = Convert.ToString(Request["mem3country"]);
                        objmember3.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember3.IsDeleted = false;
                        objmember3.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember3);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic3 = new TblProfilePicture();
                        objpic3.ProfiePictureName = Request["hiddenmem3photo"].ToString();
                        objpic3.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic3.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic3);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic3 = new TblMapMemberProfilePicture();
                        objmappic3.MemberId = objmember3.MemberId;
                        objmappic3.ProfilePictureId = objpic3.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic3);
                        context.SubmitChanges();

                        //Enter Member#4 entry

                        TblMember objmember4 = new TblMember();
                        objmember4.MemberFirstName = Convert.ToString(Request["mem4firstname"]);
                        objmember4.MemberMiddleName = Convert.ToString(Request["mem4middlename"]);
                        objmember4.MemberLastName = Convert.ToString(Request["mem4lastname"]);
                        objmember4.MemberDOB = Convert.ToDateTime(Request["mem4dob"]);
                        objmember4.Gender = Convert.ToString(Request["mem4gen"]);
                        objmember4.EmailId = Convert.ToString(Request["mem4email"]);
                        objmember4.Address = Convert.ToString(Request["mem4address"]);
                        objmember4.Phone = Convert.ToString(Request["mem4phone"]);
                        objmember4.City = Convert.ToString(Request["mem4city"]);
                        objmember4.State = Convert.ToString(Request["mem4state"]);
                        objmember4.Country = Convert.ToString(Request["mem4country"]);
                        objmember4.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember4.IsDeleted = false;
                        objmember4.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember4);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic4 = new TblProfilePicture();
                        objpic4.ProfiePictureName = Request["hiddenmem4photo"].ToString();
                        objpic4.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic4.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic4);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic4 = new TblMapMemberProfilePicture();
                        objmappic4.MemberId = objmember4.MemberId;
                        objmappic4.ProfilePictureId = objpic4.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic4);
                        context.SubmitChanges();
                    }
                    else if (Convert.ToInt32(Request["Noppl"]) == 5)
                    {
                        //Enter Member#1 entry

                        objmember = new TblMember();
                        objmember.MemberFirstName = Convert.ToString(Request["mem1firstname"]);
                        objmember.MemberMiddleName = Convert.ToString(Request["mem1middlename"]);
                        objmember.MemberLastName = Convert.ToString(Request["mem1lastname"]);
                        objmember.MemberDOB = Convert.ToDateTime(Request["mem1dob"]);
                        objmember.Gender = Convert.ToString(Request["mem1gen"]);
                        objmember.EmailId = Convert.ToString(Request["mem1email"]);
                        objmember.Address = Convert.ToString(Request["mem1address"]);
                        objmember.Phone = Convert.ToString(Request["mem1phone"]);
                        objmember.City = Convert.ToString(Request["mem1city"]);
                        objmember.State = Convert.ToString(Request["mem1state"]);
                        objmember.Country = Convert.ToString(Request["mem1country"]);
                        objmember.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember.IsDeleted = false;
                        objmember.IsBlocked = false;
                        context.TblMembers.InsertOnSubmit(objmember);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic = new TblProfilePicture();
                        objpic.ProfiePictureName = Request["hiddenmem1photo"].ToString();
                        objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                        objmappic.MemberId = objmember.MemberId;
                        objmappic.ProfilePictureId = objpic.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                        context.SubmitChanges();

                        //Enter Member#2 entry

                        TblMember objmember2 = new TblMember();
                        objmember2.MemberFirstName = Convert.ToString(Request["mem2firstname"]);
                        objmember2.MemberMiddleName = Convert.ToString(Request["mem2middlename"]);
                        objmember2.MemberLastName = Convert.ToString(Request["mem2lastname"]);
                        objmember2.MemberDOB = Convert.ToDateTime(Request["mem2dob"]);
                        objmember2.Gender = Convert.ToString(Request["mem2gen"]);
                        objmember2.EmailId = Convert.ToString(Request["mem2email"]);
                        objmember2.Address = Convert.ToString(Request["mem2address"]);
                        objmember2.Phone = Convert.ToString(Request["mem2phone"]);
                        objmember2.City = Convert.ToString(Request["mem2city"]);
                        objmember2.State = Convert.ToString(Request["mem2state"]);
                        objmember2.Country = Convert.ToString(Request["mem2country"]);
                        objmember2.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember2.IsDeleted = false;
                        objmember2.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember2);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic2 = new TblProfilePicture();
                        objpic2.ProfiePictureName = Request["hiddenmem2photo"].ToString();
                        objpic2.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic2.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic2);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic2 = new TblMapMemberProfilePicture();
                        objmappic2.MemberId = objmember2.MemberId;
                        objmappic2.ProfilePictureId = objpic2.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic2);
                        context.SubmitChanges();

                        //Enter Member#3 entry

                        TblMember objmember3 = new TblMember();
                        objmember3.MemberFirstName = Convert.ToString(Request["mem3firstname"]);
                        objmember3.MemberMiddleName = Convert.ToString(Request["mem3middlename"]);
                        objmember3.MemberLastName = Convert.ToString(Request["mem3lastname"]);
                        objmember3.MemberDOB = Convert.ToDateTime(Request["mem3dob"]);
                        objmember3.Gender = Convert.ToString(Request["mem3gen"]);
                        objmember3.EmailId = Convert.ToString(Request["mem3email"]);
                        objmember3.Address = Convert.ToString(Request["mem3address"]);
                        objmember3.Phone = Convert.ToString(Request["mem3phone"]);
                        objmember3.City = Convert.ToString(Request["mem3city"]);
                        objmember3.State = Convert.ToString(Request["mem3state"]);
                        objmember3.Country = Convert.ToString(Request["mem3country"]);
                        objmember3.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember3.IsDeleted = false;
                        objmember3.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember3);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic3 = new TblProfilePicture();
                        objpic3.ProfiePictureName = Request["hiddenmem3photo"].ToString();
                        objpic3.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic3.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic3);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic3 = new TblMapMemberProfilePicture();
                        objmappic3.MemberId = objmember3.MemberId;
                        objmappic3.ProfilePictureId = objpic3.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic3);
                        context.SubmitChanges();

                        //Enter Member#4 entry

                        TblMember objmember4 = new TblMember();
                        objmember4.MemberFirstName = Convert.ToString(Request["mem4firstname"]);
                        objmember4.MemberMiddleName = Convert.ToString(Request["mem4middlename"]);
                        objmember4.MemberLastName = Convert.ToString(Request["mem4lastname"]);
                        objmember4.MemberDOB = Convert.ToDateTime(Request["mem4dob"]);
                        objmember4.Gender = Convert.ToString(Request["mem4gen"]);
                        objmember4.EmailId = Convert.ToString(Request["mem4email"]);
                        objmember4.Address = Convert.ToString(Request["mem4address"]);
                        objmember4.Phone = Convert.ToString(Request["mem4phone"]);
                        objmember4.City = Convert.ToString(Request["mem4city"]);
                        objmember4.State = Convert.ToString(Request["mem4state"]);
                        objmember4.Country = Convert.ToString(Request["mem4country"]);
                        objmember4.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember4.IsDeleted = false;
                        objmember4.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember4);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic4 = new TblProfilePicture();
                        objpic4.ProfiePictureName = Request["hiddenmem4photo"].ToString();
                        objpic4.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic4.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic4);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic4 = new TblMapMemberProfilePicture();
                        objmappic4.MemberId = objmember4.MemberId;
                        objmappic4.ProfilePictureId = objpic4.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic4);
                        context.SubmitChanges();

                        //Enter Member#5 entry

                        TblMember objmember5 = new TblMember();
                        objmember5.MemberFirstName = Convert.ToString(Request["mem5firstname"]);
                        objmember5.MemberMiddleName = Convert.ToString(Request["mem5middlename"]);
                        objmember5.MemberLastName = Convert.ToString(Request["mem5lastname"]);
                        objmember5.MemberDOB = Convert.ToDateTime(Request["mem5dob"]);
                        objmember5.Gender = Convert.ToString(Request["mem5gen"]);
                        objmember5.EmailId = Convert.ToString(Request["mem5email"]);
                        objmember5.Address = Convert.ToString(Request["mem5address"]);
                        objmember5.Phone = Convert.ToString(Request["mem5phone"]);
                        objmember5.City = Convert.ToString(Request["mem5city"]);
                        objmember5.State = Convert.ToString(Request["mem5state"]);
                        objmember5.Country = Convert.ToString(Request["mem5country"]);
                        objmember5.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember5.IsDeleted = false;
                        objmember5.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember5);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic5 = new TblProfilePicture();
                        objpic5.ProfiePictureName = Request["hiddenmem5photo"].ToString();
                        objpic5.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic5.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic5);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic5 = new TblMapMemberProfilePicture();
                        objmappic5.MemberId = objmember5.MemberId;
                        objmappic5.ProfilePictureId = objpic5.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic5);
                        context.SubmitChanges();
                        //Enter Member#6 entry

                        TblMember objmember6 = new TblMember();
                        objmember6.MemberFirstName = Convert.ToString(Request["mem6firstname"]);
                        objmember6.MemberMiddleName = Convert.ToString(Request["mem6middlename"]);
                        objmember6.MemberLastName = Convert.ToString(Request["mem6lastname"]);
                        objmember6.MemberDOB = Convert.ToDateTime(Request["mem6dob"]);
                        objmember6.Gender = Convert.ToString(Request["mem6gen"]);
                        objmember6.EmailId = Convert.ToString(Request["mem6email"]);
                        objmember6.Address = Convert.ToString(Request["mem6address"]);
                        objmember6.Phone = Convert.ToString(Request["mem6phone"]);
                        objmember6.City = Convert.ToString(Request["mem6city"]);
                        objmember6.State = Convert.ToString(Request["mem6state"]);
                        objmember6.Country = Convert.ToString(Request["mem6country"]);
                        objmember6.MembershipRegistrationId = objmembershipregister.MembershipRegistrationId;
                        objmember6.IsDeleted = false;
                        objmember6.IsBlocked = false;

                        context.TblMembers.InsertOnSubmit(objmember6);
                        context.SubmitChanges();
                        //Enter the picture in picture table

                        TblProfilePicture objpic6 = new TblProfilePicture();
                        objpic6.ProfiePictureName = Request["hiddenmem6photo"].ToString();
                        objpic6.CreatedBy = SunCity.Core.Session.Current.UserId;
                        objpic6.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                        context.TblProfilePictures.InsertOnSubmit(objpic6);
                        context.SubmitChanges();

                        //Enter the member-picture entry in diff table
                        TblMapMemberProfilePicture objmappic6 = new TblMapMemberProfilePicture();
                        objmappic6.MemberId = objmember6.MemberId;
                        objmappic6.ProfilePictureId = objpic6.ProfilePictureId;
                        context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic6);
                        context.SubmitChanges();
                    }

                }

                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Membership:" + objmembershipregister.MembershipRegistrationId + " has been registered by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("AddMembershipRegistration", "Function", new { MembershipRegestrationId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return RedirectToAction("AddMembershipRegistration", "Function", new { MembershipRegestrationId = 0 });
            }

        }

        public ActionResult ViewMembershipRegistration()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMembershipRegistration)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            var getregestrations = (from r in context.TblMembershipRegistrations orderby r.MembershipRegistrationId descending select r).ToList();
            List<MembershipRegistration> listreg = new List<MembershipRegistration>();
            MembershipRegistration objreg = new MembershipRegistration();
            foreach (var item in getregestrations)
            {
                objreg = new MembershipRegistration();
                objreg.Amount = Convert.ToDecimal(item.Amount);
                objreg.IsDeleted = Convert.ToBoolean(item.IsDeleted);
                objreg.MembershipEndDate = Convert.ToDateTime(item.MembershipEndDate);
                objreg.MembershipRegistrationId = item.MembershipRegistrationId;
                objreg.MembershipStartDate = Convert.ToDateTime(item.MembershipStartDate);
                var getname = (from m in context.TblMembers where m.MembershipRegistrationId == item.MembershipRegistrationId orderby m.MemberId select m).FirstOrDefault();
                if (getname != null)
                {
                    objreg.Name = getname.MemberFirstName + " " + getname.MemberMiddleName + " " + getname.MemberLastName;
                }
                else
                {
                    objreg.Name = "---";
                }
                listreg.Add(objreg);
            }
            if (getregestrations != null)
            {
                return View(listreg);
                //return View(getregestrations);
            }
            else
            {
                return View();
            }
        }

        public ActionResult DeleteMembership(int? MembershipRegestrationId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteMembership)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TblMembershipRegistration objmembership = new TblMembershipRegistration();
            if (MembershipRegestrationId != 0)
            {
                objmembership = (from k in context.TblMembershipRegistrations where k.MembershipRegistrationId == MembershipRegestrationId select k).FirstOrDefault();
            }
            objmembership.IsDeleted = true;
            context.SubmitChanges();

            var getallmembersofmembership = (from g in context.TblMembers where g.MembershipRegistrationId == MembershipRegestrationId select g).ToList();

            foreach (var item in getallmembersofmembership)
            {
                item.IsDeleted = true;
                item.IsBlocked = true;
                context.SubmitChanges();
            }
            TempData["deletestatus"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Membership:" + objmembership.MembershipRegistrationId + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("ViewMembershipRegistration", "Function");
        }

        public ActionResult ViewMembership(int? MembershipRegestrationId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMembership)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblMembershipRegistration getobjreg = new TblMembershipRegistration();
            getobjreg = (from reg in context.TblMembershipRegistrations where reg.MembershipRegistrationId == MembershipRegestrationId select reg).FirstOrDefault();
            getobjreg.listmembers = (from mem in context.TblMembers where mem.MembershipRegistrationId == MembershipRegestrationId && mem.IsDeleted == false select mem).ToList();
            //getobjreg.listmembers = (from mem in context.TblMembers where mem.MembershipRegistrationId == MembershipRegestrationId && mem.IsDeleted == true && mem.IsBlocked == true select mem).ToList();
            getobjreg.listMembershipPlan = getobjreg.getListMembershipPlan();
            getobjreg.listPackages = getobjreg.getListPackage();
            getobjreg.listdocuments = (from d in context.TblDocuments where d.MembershipRegistrationId == MembershipRegestrationId select d).ToList();
            return View(getobjreg);
        }

        [HttpPost]
        public ActionResult ViewMembership(TblMembershipRegistration FrmMembershipRegistration, int MembershipRegistrationId = 0)
        {
            if (Request["hiddencancelflag"] != "0")
            {
                return RedirectToAction("ViewMembershipRegistration", "Function");
            }
            if (Request["hiddendeleteflag"] != "0")
            {
                TblMembershipRegistration getobjregdel = new TblMembershipRegistration();
                getobjregdel = (from e in context.TblMembershipRegistrations where e.MembershipRegistrationId == MembershipRegistrationId select e).FirstOrDefault();
                getobjregdel.IsDeleted = true;
                context.SubmitChanges();

                //Delete and block existing members of the membership
                getobjregdel.listmembers = (from mem in context.TblMembers where mem.MembershipRegistrationId == MembershipRegistrationId select mem).ToList();
                TblMember objmember = new TblMember();
                foreach (var item in getobjregdel.listmembers)
                {
                    objmember = new TblMember();
                    objmember = (from o in context.TblMembers where o.MemberId == item.MemberId select o).FirstOrDefault();
                    objmember.IsDeleted = true;
                    objmember.IsBlocked = true;
                    context.SubmitChanges();
                }
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Membership:" + getobjregdel.MembershipRegistrationId + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("ViewMembershipRegistration", "Function");
            }
            //updating of membership details
            TblMembershipRegistration getobjreg = new TblMembershipRegistration();
            getobjreg = (from e in context.TblMembershipRegistrations where e.MembershipRegistrationId == MembershipRegistrationId select e).FirstOrDefault();
            getobjreg.MembershipPlanId = FrmMembershipRegistration.MembershipPlanId;
            getobjreg.PackageId = FrmMembershipRegistration.PackageId;
            getobjreg.Amount = FrmMembershipRegistration.Amount;
            DateTime stdt = Convert.ToDateTime(FrmMembershipRegistration.MembershipStartDate);
            getobjreg.MembershipStartDate = FrmMembershipRegistration.MembershipStartDate;

            var getmembershiplan = (from m in context.TblMembershipPlans where m.MembershipPlanId == FrmMembershipRegistration.MembershipPlanId select m).FirstOrDefault();
            getobjreg.MembershipEndDate = stdt.AddYears(Convert.ToInt32(getmembershiplan.MembershipPeriod));
            if (Request["IsDeleted"] == null)
            {
                getobjreg.IsDeleted = false;
            }
            else
            {
                getobjreg.IsDeleted = true;
            }
            context.SubmitChanges();

            //Add documents entry
            TblDocument objdocs = new TblDocument();
            string getdocs = Convert.ToString(Request["hiddendocs"]);
            string[] words = getdocs.Split(',');
            foreach (string word in words)
            {
                if (word != "")
                {
                    objdocs = new TblDocument();
                    objdocs.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objdocs.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    objdocs.DocumentName = word;
                    objdocs.MembershipRegistrationId = getobjreg.MembershipRegistrationId;
                    context.TblDocuments.InsertOnSubmit(objdocs);
                    context.SubmitChanges();
                }

            }

            //getobjreg.listmembers = (from mem in context.TblMembers where mem.MembershipRegistrationId == MembershipRegistrationId select mem).ToList();

            //if (Request["hiddencorporateflag"] == null)
            //{
            //    //Updating of member details
            //    TblMember objmember = new TblMember();
            //    foreach (var item in getobjreg.listmembers)
            //    {
            //        objmember = new TblMember();
            //        objmember = (from o in context.TblMembers where o.MemberId == item.MemberId select o).FirstOrDefault();

            //        objmember.MemberFirstName = Convert.ToString(Request[item.MemberId + " firstname"]);
            //        objmember.MemberMiddleName = Convert.ToString(Request[item.MemberId + " middlename"]);
            //        objmember.MemberLastName = Convert.ToString(Request[item.MemberId + " lastname"]);
            //        objmember.MemberDOB = Convert.ToDateTime(Request[item.MemberId + " dob"]);
            //        //objmember.Gender = Convert.ToString(Request[item.MemberId + " gender"]);
            //        string g = Convert.ToString(Request[item.MemberId + " gender"]);
            //        objmember.EmailId = Convert.ToString(Request[item.MemberId + " email"]);
            //        objmember.Address = Convert.ToString(Request[item.MemberId + " address"]);
            //        objmember.Phone = Convert.ToString(Request[item.MemberId + " phone"]);
            //        objmember.City = Convert.ToString(Request[item.MemberId + " city"]);
            //        objmember.State = Convert.ToString(Request[item.MemberId + " state"]);
            //        objmember.Country = Convert.ToString(Request[item.MemberId + " country"]);
            //        if (Request[item.MemberId + " IsDeleted"] == null)
            //        {
            //            objmember.IsDeleted = false;
            //        }
            //        else
            //        {
            //            objmember.IsDeleted = true;
            //        }
            //        if (Request[item.MemberId + " IsBlocked"] == null)
            //        {
            //            objmember.IsBlocked = false;
            //        }
            //        else
            //        {
            //            objmember.IsBlocked = true;
            //        }
            //        //objmember.IsDeleted = Convert.ToBoolean(Request[item.MemberId + " IsDeleted"]);
            //        //objmember.IsBlocked = Convert.ToBoolean(Request[item.MemberId + " IsBlocked"]);

            //        context.SubmitChanges();
            //    }
            //}
            //else
            //{
            //    var j = Convert.ToInt32(Request["hiddencorporateflag"]);
            //    //Backing up the Previous data

            //    BackUpMember objbkp = new BackUpMember();
            //    TblMember upmember = new TblMember();
            //    var getmembers = (from mem in context.TblMembers where mem.MembershipRegistrationId == MembershipRegistrationId select mem).ToList();
            //    foreach (var itemmember in getmembers)
            //    {
            //        objbkp = new BackUpMember();
            //        objbkp.Address = itemmember.Address;
            //        objbkp.City = itemmember.City;
            //        objbkp.Country = itemmember.Country;
            //        objbkp.EmailId = itemmember.EmailId;
            //        objbkp.Gender = itemmember.Gender;
            //        objbkp.IsBlocked = itemmember.IsBlocked;
            //        objbkp.IsDeleted = itemmember.IsDeleted;
            //        objbkp.MemberDOB = itemmember.MemberDOB;
            //        objbkp.MemberFirstName = itemmember.MemberFirstName;
            //        objbkp.MemberLastName = itemmember.MemberLastName;
            //        objbkp.MemberMiddleName = itemmember.MemberMiddleName;
            //        objbkp.MembershipRegistrationId = MembershipRegistrationId;
            //        objbkp.Phone = itemmember.Phone;
            //        objbkp.State = itemmember.State;

            //        context.BackUpMembers.InsertOnSubmit(objbkp);
            //        context.SubmitChanges();

            //        upmember = new TblMember();
            //        upmember = (from u in context.TblMembers where u.MemberId == itemmember.MemberId select u).FirstOrDefault();
            //        upmember.IsDeleted = true;
            //        upmember.IsBlocked = true;
            //        context.SubmitChanges();
            //    }

            //    //Updating of member details
            //    TblMember objmember = new TblMember();
            //    foreach (var item in getobjreg.listmembers)
            //    {
            //        objmember = new TblMember();
            //        //objmember = (from o in context.TblMembers where o.MemberId == item.MemberId select o).FirstOrDefault();

            //        objmember.MemberFirstName = Convert.ToString(Request[item.MemberId + " firstname"]);
            //        objmember.MemberMiddleName = Convert.ToString(Request[item.MemberId + " middlename"]);
            //        objmember.MemberLastName = Convert.ToString(Request[item.MemberId + " lastname"]);
            //        objmember.MemberDOB = Convert.ToDateTime(Request[item.MemberId + " dob"]);
            //        objmember.Gender = Convert.ToString(Request[item.MemberId + " gender"]);
            //        string g = Convert.ToString(Request[item.MemberId + " gender"]);
            //        objmember.EmailId = Convert.ToString(Request[item.MemberId + " email"]);
            //        objmember.Address = Convert.ToString(Request[item.MemberId + " address"]);
            //        objmember.Phone = Convert.ToString(Request[item.MemberId + " phone"]);
            //        objmember.City = Convert.ToString(Request[item.MemberId + " city"]);
            //        objmember.State = Convert.ToString(Request[item.MemberId + " state"]);
            //        objmember.Country = Convert.ToString(Request[item.MemberId + " country"]);
            //        if (Request[item.MemberId + " IsDeleted"] == null)
            //        {
            //            objmember.IsDeleted = false;
            //        }
            //        else
            //        {
            //            objmember.IsDeleted = true;
            //        }
            //        if (Request[item.MemberId + " IsBlocked"] == null)
            //        {
            //            objmember.IsBlocked = false;
            //        }
            //        else
            //        {
            //            objmember.IsBlocked = true;
            //        }
            //        //objmember.IsDeleted = Convert.ToBoolean(Request[item.MemberId + " IsDeleted"]);
            //        //objmember.IsBlocked = Convert.ToBoolean(Request[item.MemberId + " IsBlocked"]);
            //        context.TblMembers.InsertOnSubmit(objmember);
            //        context.SubmitChanges();
            //    }
            //}


            TempData["alertstatus"] = "Success";
            return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = MembershipRegistrationId });
        }


        [HttpPost]
        public ActionResult AddNewMember(string fname, string lname, string mname, string email, string adob, string aadd, string aphone, string acity, string astate, string acountry, string agen, string MembershipRegistrationId, string picname)
        {

            TblMember objmember = new TblMember();
            objmember.MemberFirstName = fname;
            objmember.MemberMiddleName = mname;
            objmember.MemberLastName = lname;
            objmember.EmailId = email;
            objmember.MemberDOB = Convert.ToDateTime(adob);
            objmember.Address = aadd;
            objmember.City = acity;
            objmember.Country = acountry;
            objmember.Gender = agen;
            objmember.IsBlocked = false;
            objmember.IsDeleted = false;
            objmember.MembershipRegistrationId = Convert.ToInt32(MembershipRegistrationId);
            objmember.Phone = aphone;
            objmember.State = astate;
            context.TblMembers.InsertOnSubmit(objmember);
            context.SubmitChanges();

            //Enter the picture in picture table

            TblProfilePicture objpic = new TblProfilePicture();
            objpic.ProfiePictureName = picname;
            objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
            objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
            context.TblProfilePictures.InsertOnSubmit(objpic);
            context.SubmitChanges();

            //Enter the member-picture entry in diff table
            TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
            objmappic.MemberId = objmember.MemberId;
            objmappic.ProfilePictureId = objpic.ProfilePictureId;
            context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
            context.SubmitChanges();

            TempData["alertstatus"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Member:" + objmember.MemberFirstName + " " + objmember.MemberLastName + " has been added to membership: " + MembershipRegistrationId + " by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = MembershipRegistrationId });
        }

        public ActionResult DeleteDocument(int? Documentid, int? MembershipRegistrationId)
        {
            TblDocument objdocument = new TblDocument();
            if (Documentid != 0)
            {
                objdocument = (from k in context.TblDocuments where k.DocumentId == Documentid select k).FirstOrDefault();
            }

            //Code to delete file from folder
            //Delete the previous file from folder
            string path = Server.MapPath("~/DocumentFolder/");
            string localFileDest = path + "/" + objdocument.DocumentName;
            if (System.IO.File.Exists(localFileDest))
            {
                System.IO.File.Delete(localFileDest);
            }

            if (objdocument != null)
            {
                context.TblDocuments.DeleteOnSubmit(objdocument);
            }
            context.SubmitChanges();

            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Document:" + objdocument.DocumentName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }

            return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = MembershipRegistrationId });
        }

        //----------------------------End of Membership Registration Block-------------------------------------//


        //----------------------------Start of Members i.e. View, update , block and unblock Block-------------------------------------//
        [HttpPost]
        public ActionResult UpdateProfilePic(int memberid, string picname)
        {
            try
            {
                //get profile pic values
                var getmapprofilemember = (from m in context.TblMapMemberProfilePictures where m.MemberId == memberid select m).FirstOrDefault();
                //if getmapprofilemember is not null then we need to update the profile pic else insert a new one
                if (getmapprofilemember != null)
                {
                    //update first the entry in profile pic table
                    var getprofilepic = (from p in context.TblProfilePictures where p.ProfilePictureId == getmapprofilemember.ProfilePictureId select p).FirstOrDefault();
                    if (getprofilepic != null)
                    {
                        //Delete the previous file from folder
                        string path = Server.MapPath("~/UserProfile/");
                        string localFileDest = path + "/" + getprofilepic.ProfiePictureName;
                        if (System.IO.File.Exists(localFileDest))
                        {
                            System.IO.File.Delete(localFileDest);
                        }
                        //getprofilepic.ProfiePictureName = Request["hiddeneditphoto"].ToString();
                        getprofilepic.ProfiePictureName = picname;
                        context.SubmitChanges();
                    }
                }
                else
                {
                    //enter first the value in profile pic table
                    TblProfilePicture objpic = new TblProfilePicture();
                    // objpic.ProfiePictureName = Request["hiddeneditphoto"].ToString();
                    objpic.ProfiePictureName = picname;
                    objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                    objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TblProfilePictures.InsertOnSubmit(objpic);
                    context.SubmitChanges();

                    // enter its value
                    TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                    objmappic.ProfilePictureId = objpic.ProfilePictureId;
                    objmappic.MemberId = memberid;
                    context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                    context.SubmitChanges();
                }
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Photo of Member#:" + memberid + " has been updated by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }

                return Content("true");
            }
            catch (Exception ex)
            {
                return Content("false");
            }
        }

        public ActionResult ViewMember(int MemberId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMember)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            Member objgetmember = new Member();
            List<Gender> listgender = new List<Gender>();
            Gender objgender = new Gender();
            objgender = new Gender();
            objgender.gendername = "Male";
            objgender.gendervalue = "Male";
            listgender.Add(objgender);
            objgender = new Gender();
            objgender.gendervalue = "Female";
            objgender.gendername = "Female";
            listgender.Add(objgender);
            objgetmember.listgender = listgender;
            if (MemberId == 0)
            {

                return View(objgetmember);
            }
            else
            {
                var mem = (from o in context.TblMembers where o.MemberId == MemberId select o).FirstOrDefault();
                objgetmember.Address = mem.Address;
                objgetmember.City = mem.City;
                objgetmember.Country = mem.Country;
                objgetmember.EmailId = mem.EmailId;
                objgetmember.Gender = mem.Gender;
                objgetmember.IsBlocked = Convert.ToBoolean(mem.IsBlocked);
                objgetmember.IsDeleted = Convert.ToBoolean(mem.IsDeleted);
                objgetmember.MemberDOB = Convert.ToDateTime(mem.MemberDOB);
                objgetmember.MemberFirstName = mem.MemberFirstName;
                objgetmember.MemberId = mem.MemberId;
                objgetmember.MemberLastName = mem.MemberLastName;
                objgetmember.MemberMiddleName = mem.MemberMiddleName;
                objgetmember.MembershipRegistrationId = Convert.ToInt32(mem.MembershipRegistrationId);
                objgetmember.Phone = mem.Phone;
                objgetmember.State = mem.State;
                return View(objgetmember);
            }

        }

        public ActionResult EditMember(int MemberId = 0, int rtflag = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditMember)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            Member objgetmember = new Member();
            List<Gender> listgender = new List<Gender>();
            Gender objgender = new Gender();
            objgender = new Gender();
            objgender.gendername = "Male";
            objgender.gendervalue = "Male";
            listgender.Add(objgender);
            objgender = new Gender();
            objgender.gendervalue = "Female";
            objgender.gendername = "Female";
            listgender.Add(objgender);
            objgetmember.listgender = listgender;
            if (SunCity.Core.Session.Current.flgrt == 0)
            {
                SunCity.Core.Session.Current.flgrt = rtflag;
            }
            if (MemberId == 0)
            {

                return View(objgetmember);
            }
            else
            {
                var mem = (from o in context.TblMembers where o.MemberId == MemberId select o).FirstOrDefault();
                objgetmember.Address = mem.Address;
                objgetmember.City = mem.City;
                objgetmember.Country = mem.Country;
                objgetmember.EmailId = mem.EmailId;
                objgetmember.Gender = mem.Gender;
                objgetmember.IsBlocked = Convert.ToBoolean(mem.IsBlocked);
                objgetmember.IsDeleted = Convert.ToBoolean(mem.IsDeleted);
                objgetmember.MemberDOB = Convert.ToDateTime(mem.MemberDOB).Date;
                objgetmember.MemberFirstName = mem.MemberFirstName;
                objgetmember.MemberId = mem.MemberId;
                objgetmember.MemberLastName = mem.MemberLastName;
                objgetmember.MemberMiddleName = mem.MemberMiddleName;
                objgetmember.MembershipRegistrationId = Convert.ToInt32(mem.MembershipRegistrationId);
                objgetmember.Phone = mem.Phone;
                objgetmember.State = mem.State;

                //Get profile details
                var getprofile = (from p in context.TblProfilePictures
                                  join
                                  m in context.TblMapMemberProfilePictures on p.ProfilePictureId equals m.ProfilePictureId
                                  where m.MemberId == Convert.ToInt32(objgetmember.MemberId)
                                  select p).FirstOrDefault();
                if (getprofile != null)
                {
                    objgetmember.profilePictureId = getprofile.ProfilePictureId;
                    objgetmember.ProfilePictreName = getprofile.ProfiePictureName;
                }

                //Get plan id
                var getmembership = (from m in context.TblMembershipRegistrations where m.MembershipRegistrationId == Convert.ToInt32(mem.MembershipRegistrationId) select m).FirstOrDefault();
                if (getmembership != null)
                {
                    objgetmember.MembershipPlanId = Convert.ToInt32(getmembership.MembershipPlanId);
                }

                return View(objgetmember);
            }

        }

        [HttpPost]
        public ActionResult ViewMember(Member frmmember, int MemberId = 0)
        {
            try
            {
                TblMember objmembers = new TblMember();
                if (MemberId != 0)
                {
                    objmembers = (from k in context.TblMembers where k.MemberId == MemberId select k).FirstOrDefault();
                }

                objmembers.MemberFirstName = frmmember.MemberFirstName;
                objmembers.MemberLastName = frmmember.MemberLastName;
                objmembers.MemberMiddleName = frmmember.MemberMiddleName;
                objmembers.Address = frmmember.Address;
                objmembers.City = frmmember.City;
                objmembers.Country = frmmember.Country;
                objmembers.EmailId = frmmember.EmailId;
                objmembers.Gender = frmmember.Gender;
                objmembers.MemberDOB = frmmember.MemberDOB;
                objmembers.Phone = frmmember.Phone;
                objmembers.State = frmmember.State;

                if (Request["IsDeleted"] == null)
                {
                    objmembers.IsDeleted = false;
                }
                else
                {
                    objmembers.IsDeleted = true;
                }
                if (Request["IsBlocked"] == null)
                {
                    objmembers.IsBlocked = false;
                }
                else
                {
                    objmembers.IsBlocked = true;
                }

                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Member:" + objmembers.MemberFirstName + " " + objmembers.MemberLastName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("ViewMember", "Function", new { MemberId = 0 });

            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }

        }
        [HttpPost]
        public ActionResult EditMember(Member frmmember, int MemberId = 0, int rtflag = 0)
        {
            try
            {
                //Block to cancel the changes
                if (Request["hiddencancelflag"] != "0")
                {
                    TblMember objmembers = new TblMember();
                    if (MemberId != 0)
                    {
                        objmembers = (from k in context.TblMembers where k.MemberId == MemberId select k).FirstOrDefault();
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("ViewMember", "Function", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = objmembers.MembershipRegistrationId });
                    }

                }
                //Code to delete member
                if (Request["hiddendeleteflag"] != "0")
                {
                    TblMember objmembers = new TblMember();
                    if (MemberId != 0)
                    {
                        objmembers = (from k in context.TblMembers where k.MemberId == MemberId select k).FirstOrDefault();
                        if (objmembers != null)
                        {
                            objmembers.IsDeleted = true;
                            context.SubmitChanges();
                        }
                    }
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Member:" + objmembers.MemberFirstName + " " + objmembers.MemberLastName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("ViewMember", "Function", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = objmembers.MembershipRegistrationId });
                    }

                }
                //Code to block member
                if (Request["hiddenblockflag"] != "0")
                {
                    TblMember objmembers = new TblMember();
                    if (MemberId != 0)
                    {
                        objmembers = (from k in context.TblMembers where k.MemberId == MemberId select k).FirstOrDefault();
                        if (objmembers != null)
                        {
                            objmembers.IsBlocked = true;
                            context.SubmitChanges();
                        }
                    }
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Member:" + objmembers.MemberFirstName + " " + objmembers.MemberLastName + " has been blocked by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("ViewMember", "Function", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = objmembers.MembershipRegistrationId });
                    }

                }
                if (Request["hiddencorporateflag"] == "0")
                {
                    TblMember objmembers = new TblMember();
                    if (MemberId != 0)
                    {
                        objmembers = (from k in context.TblMembers where k.MemberId == MemberId select k).FirstOrDefault();
                    }
                    //rtflag = Convert.ToInt32(Request["flgrt"].ToString());
                    objmembers.MemberFirstName = frmmember.MemberFirstName;
                    objmembers.MemberLastName = frmmember.MemberLastName;
                    objmembers.MemberMiddleName = frmmember.MemberMiddleName;
                    objmembers.Address = frmmember.Address;
                    objmembers.City = frmmember.City;
                    objmembers.Country = frmmember.Country;
                    objmembers.EmailId = frmmember.EmailId;
                    objmembers.Gender = frmmember.Gender;
                    objmembers.MemberDOB = frmmember.MemberDOB;
                    objmembers.Phone = frmmember.Phone;
                    objmembers.State = frmmember.State;

                    if (Request["IsDeleted"] == null)
                    {
                        objmembers.IsDeleted = false;
                    }
                    else
                    {
                        objmembers.IsDeleted = true;
                    }
                    if (Request["IsBlocked"] == null)
                    {
                        objmembers.IsBlocked = false;
                    }
                    else
                    {
                        objmembers.IsBlocked = true;
                    }

                    context.SubmitChanges();
                    TempData["alertstatus"] = "Success";
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Member:" + objmembers.MemberFirstName + " " + objmembers.MemberLastName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("ViewMember", "Function", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = objmembers.MembershipRegistrationId });
                    }
                }
                else
                {

                    BackUpMember objbkp = new BackUpMember();
                    TblMember upmember = new TblMember();
                    var getmembers = (from mem in context.TblMembers where mem.MemberId == MemberId select mem).FirstOrDefault();

                    objbkp = new BackUpMember();
                    objbkp.Address = getmembers.Address;
                    objbkp.City = getmembers.City;
                    objbkp.Country = getmembers.Country;
                    objbkp.EmailId = getmembers.EmailId;
                    objbkp.Gender = getmembers.Gender;
                    objbkp.IsBlocked = getmembers.IsBlocked;
                    objbkp.IsDeleted = getmembers.IsDeleted;
                    objbkp.MemberDOB = getmembers.MemberDOB;
                    objbkp.MemberFirstName = getmembers.MemberFirstName;
                    objbkp.MemberLastName = getmembers.MemberLastName;
                    objbkp.MemberMiddleName = getmembers.MemberMiddleName;
                    objbkp.MembershipRegistrationId = getmembers.MembershipRegistrationId;
                    objbkp.Phone = getmembers.Phone;
                    objbkp.State = getmembers.State;

                    context.BackUpMembers.InsertOnSubmit(objbkp);
                    context.SubmitChanges();

                    upmember = new TblMember();
                    upmember = (from u in context.TblMembers where u.MemberId == getmembers.MemberId select u).FirstOrDefault();
                    upmember.IsDeleted = true;
                    upmember.IsBlocked = true;
                    context.SubmitChanges();


                    //Updating of member details
                    TblMember objmembers = new TblMember();
                    objmembers.MemberFirstName = frmmember.MemberFirstName;
                    objmembers.MemberLastName = frmmember.MemberLastName;
                    objmembers.MemberMiddleName = frmmember.MemberMiddleName;
                    objmembers.Address = frmmember.Address;
                    objmembers.City = frmmember.City;
                    objmembers.Country = frmmember.Country;
                    objmembers.EmailId = frmmember.EmailId;
                    objmembers.Gender = frmmember.Gender;
                    objmembers.MemberDOB = frmmember.MemberDOB;
                    objmembers.Phone = frmmember.Phone;
                    objmembers.State = frmmember.State;
                    objmembers.MembershipRegistrationId = objbkp.MembershipRegistrationId;
                    if (Request["IsDeleted"] == null)
                    {
                        objmembers.IsDeleted = false;
                    }
                    else
                    {
                        objmembers.IsDeleted = true;
                    }
                    if (Request["IsBlocked"] == null)
                    {
                        objmembers.IsBlocked = false;
                    }
                    else
                    {
                        objmembers.IsBlocked = true;
                    }
                    context.TblMembers.InsertOnSubmit(objmembers);
                    context.SubmitChanges();
                    TempData["alertstatus"] = "Success";
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Member:" + objmembers.MemberFirstName + " " + objmembers.MemberLastName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("ViewMember", "Function", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = objmembers.MembershipRegistrationId });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }

        }

        public ActionResult _GridMembers()
        {
            var getmembers = (from r in context.TblMembers where r.IsDeleted == false orderby r.MemberId descending select r).ToList();
            if (getmembers != null)
            {
                return View(getmembers);
            }
            else
            {
                return View();
            }
        }

        public ActionResult BlockedMembers()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.BlockedMembers)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            var getmembers = (from r in context.TblMembers where r.IsBlocked == true orderby r.MemberId descending select r).ToList();
            if (getmembers != null)
            {
                return View(getmembers);
            }
            else
            {
                return View();
            }
        }

        public ActionResult UnblockExistingMember(int? MemberId)
        {
            TblMember objmembers = new TblMember();
            if (MemberId != 0)
            {
                objmembers = (from k in context.TblMembers where k.MemberId == MemberId select k).FirstOrDefault();
            }
            objmembers.IsBlocked = false;
            context.SubmitChanges();
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Member:" + objmembers.MemberFirstName + " " + objmembers.MemberLastName + " has been unblocked by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("BlockedMembers", "Function");
        }

        public ActionResult UnblockedMembers()
        {
            var getmembers = (from r in context.TblMembers where r.IsBlocked == false orderby r.MemberId descending select r).ToList();
            if (getmembers != null)
            {
                return View(getmembers);
            }
            else
            {
                return View();
            }
        }
        /// <summary>
        /// Below function is used to block a member so restrict his access.
        /// </summary>
        /// <param name="MemberId">
        /// MemberId of the member is the required field
        /// </param>
        /// <returns>After blocking the member it will redirect to the respected page from where the fucntion has been called.</returns>
        public ActionResult BlockExistingMember(int? MemberId)
        {
            TblMember objmembers = new TblMember();

            objmembers = (from k in context.TblMembers where k.MemberId == MemberId select k).FirstOrDefault();

            if (objmembers != null)
            {
                objmembers.IsBlocked = true;
                context.SubmitChanges();
            }
            TempData["blockstatusmember"] = "Success";
            if (SunCity.Core.Session.Current.flgrt == 0)
            {
                return RedirectToAction("ViewMember", "Function", new { MemberId = 0 });
            }
            else
            {
                return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = objmembers.MembershipRegistrationId });
            }
        }
        /// <summary>
        /// Below function is used to delete a member
        /// </summary>
        /// <param name="MemberId">
        /// MemberId of the member is the required field
        /// </param>
        /// <returns>
        /// After deleteing the member it will redirect to the respected page from where the fucntion has been called.
        /// </returns>
        public ActionResult DeleteMember(int? MemberId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteMember)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblMember objmember = new TblMember();


            var getallmembersofmembership = (from g in context.TblMembers where g.MemberId == MemberId select g).FirstOrDefault();

            if (getallmembersofmembership != null)
            {
                getallmembersofmembership.IsDeleted = true;
                getallmembersofmembership.IsBlocked = true;
                context.SubmitChanges();
            }
            TempData["deletestatusmember"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Member:" + getallmembersofmembership.MemberFirstName + " " + getallmembersofmembership.MemberLastName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            if (SunCity.Core.Session.Current.flgrt == 0)
            {
                return RedirectToAction("ViewMember", "Function", new { MemberId = 0 });
            }
            else
            {
                return RedirectToAction("ViewMembership", "Function", new { MembershipRegestrationId = getallmembersofmembership.MembershipRegistrationId });
            }
        }
        //----------------------------End of Members i.e. View, update , block and unblock Block-------------------------------------//

        /// <summary>
        /// Below function is used to set the view of Permission page. It will take all the roles and the Permission list and will display with the required permission granted to the relative role.
        /// </summary>
        /// <returns></returns>
        public ActionResult Permission()
        {
            if (SunCity.Core.Session.Current.UserId <= 0)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ManagePermission)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            MasterPermission objpermission = new MasterPermission();

            var roles = (from i in context.TblRoles select i).ToList();
            objpermission.AllRole = roles;

            List<PermissionList> lpl = new List<PermissionList>();
            var permissions = (from i in context.PermissionRecords select i).ToList();
            foreach (var each in permissions)
            {
                var permissionsroles = (from i in context.PermissionRecord_Role_Mappings
                                        where i.PermissionRecord_Id == each.Id
                                        select i).ToList();
                PermissionList pl = new PermissionList();
                pl.AllPermission = each;
                pl.PermissionRoleMap = permissionsroles;
                lpl.Add(pl);
            }
            objpermission.Permission = lpl;

            return View(objpermission);
        }

        /// <summary>
        /// below action is used to update the permission grants to the particular role.
        /// </summary>
        /// <param name="check"></param>
        /// <param name="uncheck"></param>
        /// <returns></returns>
        [HttpPost]
        public int SavePermission(string check, string uncheck)
        {
            if (SunCity.Core.Session.Current.UserId <= 0)
            {
                return 0;
            }

            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ManagePermission)))
            {
                return 0;
            }

            try
            {
                string[] uncheckall = uncheck.Split(',');
                for (int uc = 1; uc < uncheckall.Count(); uc++)
                {
                    string[] uncheckper = uncheckall[uc].Split('_');
                    int permissionid = Convert.ToInt32(uncheckper[0]);
                    int roleid = Convert.ToInt32(uncheckper[1]);

                    var perrole = context.PermissionRecord_Role_Mappings.Where(x => x.PermissionRecord_Id == permissionid && x.Role_Id == roleid).FirstOrDefault();
                    if (perrole != null)
                    {
                        context.PermissionRecord_Role_Mappings.DeleteOnSubmit(perrole);
                        context.SubmitChanges();
                    }
                }

                string[] checkall = check.Split(',');
                for (int c = 1; c < checkall.Count(); c++)
                {
                    string[] checkper = checkall[c].Split('_');
                    int permissionid = Convert.ToInt32(checkper[0]);
                    int roleid = Convert.ToInt32(checkper[1]);

                    var perrole = context.PermissionRecord_Role_Mappings.Where(x => x.PermissionRecord_Id == permissionid && x.Role_Id == roleid).FirstOrDefault();
                    if (perrole == null)
                    {
                        PermissionRecord_Role_Mapping prm = new PermissionRecord_Role_Mapping();
                        prm.PermissionRecord_Id = permissionid;
                        prm.Role_Id = roleid;
                        context.PermissionRecord_Role_Mappings.InsertOnSubmit(prm);
                        context.SubmitChanges();
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                //UtilityFunction.LogExceptionDetail(ex);
                return 0;
            }
        }

        /// <summary>
        /// Below action will display the Access Denied message if the particular role have no permission for the functionality.
        /// </summary>
        /// <returns></returns>
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult NewFestival()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.NewFestival)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            return View();
        }

        [HttpPost]
        public ActionResult NewFestival(TblFestival Objfestival, int notificationid = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.NewFestival)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                TblFestival objfev = new TblFestival();
                if (notificationid != 0)
                {
                    objfev = (from k in context.TblFestivals where k.NotificationId == notificationid select k).FirstOrDefault();
                }


                objfev.IsDeleted = false;
                objfev.FestivalName = Objfestival.FestivalName;
                objfev.FestivalNote = Objfestival.FestivalNote;

                if (notificationid == 0)
                {
                    objfev.FestivalDate = DateTime.UtcNow.AddHours(5.50).Date;
                    objfev.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    objfev.CreatedBy = SunCity.Core.Session.Current.UserId;
                    context.TblFestivals.InsertOnSubmit(objfev);
                }
                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Festival:" + objfev.FestivalName + " has been added by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("NewFestival", "Function", new { notificationid = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        public ActionResult EditFestival(int notificationid = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditFestival)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblFestival objfes = new TblFestival();
            if (notificationid != 0)
            {
                objfes = (from k in context.TblFestivals where k.NotificationId == notificationid select k).FirstOrDefault();
            }
            return View(objfes);
        }

        [HttpPost]
        public ActionResult EditFestival(TblFestival Frmfestival, int notificationid = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditFestival)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                //Cancel button clicked
                if (Request["hiddencancelflag"] != "0")
                {
                    return RedirectToAction("ViewFestival", "Function");
                }
                //Delete button handling
                if (Request["hiddendeleteflag"] != "0")
                {
                    TblFestival objfes = new TblFestival();
                    if (notificationid != 0)
                    {
                        objfes = (from k in context.TblFestivals where k.NotificationId == notificationid select k).FirstOrDefault();
                    }
                    if (objfes != null)
                    {
                        objfes.IsDeleted = true;
                    }
                    context.SubmitChanges();
                    TempData["deletestatus"] = "Success";
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = "Festival:" + objfes.FestivalName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    return RedirectToAction("ViewFestival", "Function");
                }

                TblFestival objfestival = new TblFestival();
                if (notificationid != 0)
                {
                    objfestival = (from k in context.TblFestivals where k.NotificationId == notificationid select k).FirstOrDefault();
                }

                objfestival.FestivalName = Frmfestival.FestivalName;
                objfestival.FestivalNote = Frmfestival.FestivalNote;
                objfestival.FestivalDate = Frmfestival.FestivalDate;

                if (Request["IsDeleted"] == null)
                {
                    objfestival.IsDeleted = false;
                }
                else
                {
                    objfestival.IsDeleted = true;
                }
                context.SubmitChanges();
                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Activity:" + objfestival.FestivalName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("ViewFestival", "Function", new { notificationid = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return View();
            }
        }

        public ActionResult ViewFestival(int notificationid = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewFestival)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblFestival objfestival = new TblFestival();
            objfestival = (from r in context.TblFestivals where r.NotificationId == notificationid select r).FirstOrDefault();
            return View(objfestival);

        }

        public ActionResult _GridFestival()
        {
            var getfestival = (from r in context.TblFestivals where r.IsDeleted== false orderby r.FestivalDate descending select r).ToList();
            if (getfestival != null)
            {
                return View(getfestival);
            }
            else
            {
                return View();
            }
        }
        public ActionResult DeleteFestival(int? notificationid)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteActivity)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TblFestival objfes = new TblFestival();
            if (notificationid != 0)
            {
                objfes = (from k in context.TblFestivals where k.NotificationId == notificationid select k).FirstOrDefault();
            }
            objfes.IsDeleted = true;
            context.SubmitChanges();
            TempData["deletestatus"] = "Success";
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = " Activity:" + objfes.FestivalName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("ViewFestival", "Function", new { ActivityId = 0 });
        }
    }
}
