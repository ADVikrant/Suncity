using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SunCity.Core;
using SunCity.DAL;
using System.Configuration;
using System.IO;
using ZXing;
using System.Drawing;

namespace SunCity.Controllers
{
    public class AccountsController : Controller
    {
        //
        // GET: /Accounts/
        SuncityDataContext context = new SuncityDataContext();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Below Action is for view of Login
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// Below action is used to check the login functionality of the users. It will take the object of TblUser and will pass on those value for required authentication.
        /// </summary>
        /// <param name="objuser">Username and password will be pass on to the action below</param>
        /// <returns>Will redirect to the required view as per the role</returns>
        [HttpPost]
        public ActionResult Login(TblUser objuser)
        {
            try {

                if (objuser.Username == null || objuser.Userpwd == null)
                {
                    TempData["error"] = "Please enter valid Username/Password";
                    return View(objuser);
                }

                string passphrase = ConfigurationManager.AppSettings["PassPhrase"];
                string password = SunCity.Core.UtilityFunction.EncryptData(objuser.Userpwd, passphrase);

                var login = (from i in context.TblUsers where i.Username == objuser.Username && i.Userpwd == password && i.IsDeleted == false select i).FirstOrDefault();

                if (login != null)
                {
                    SunCity.Core.Session.Current.UserId = login.UserId;
                    SunCity.Core.Session.Current.EmployeeName = login.FirstName +" " + login.LastName;
                    SunCity.Core.Session.Current.RoleId = Convert.ToInt32(login.RoleId);
                    Session.Timeout = 900;
                    ////Log Activity
                    //using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.Now.Date.ToString("dd-MM-yyyy") + "/" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt"))
                    //{
                    //    string msg =  SunCity.Core.Session.Current.EmployeeName + " logged into the system";
                    //    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    //}
                    if (login.RoleId == Convert.ToInt32(SunCity.Core.Enum.Rollname.SuperAdmin))
                    {
                        SunCity.Core.Session.Current.RoleName = "SuperAdmin";
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else if (login.RoleId == Convert.ToInt32(SunCity.Core.Enum.Rollname.Admin))
                    {
                        SunCity.Core.Session.Current.RoleName = "Admin";
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else if (login.RoleId == Convert.ToInt32(SunCity.Core.Enum.Rollname.Manager))
                    {
                        SunCity.Core.Session.Current.RoleName = "Manager";
                        return RedirectToAction("Index", "Dashboard");
                    }
                    
                }
                else
                {
                    TempData["error"] = "Username or password incorrect";
                    return View(objuser);
                }
                return View(objuser);

            
            }
            catch (Exception ex)
            {
                return View(objuser);
            }
        }
        /// <summary>
        /// Below action will log the user out of the system
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
            {
            try
            {
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = SunCity.Core.Session.Current.EmployeeName + " logged out of system";
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }

                Session.Abandon();
               
                return RedirectToAction("Login", "Accounts");

            }
            catch (Exception ex)
            {
                //UtilityFunction.LogException(ex);
                return RedirectToAction("Login", "Accounts");
            }

        }
        /// <summary>
        /// Below action is used to pass the values of the logged in user to the view. It will take the session "Userid" and will be passed to get the info of the users and then the object will be passed on to the view
        /// </summary>
        /// <returns> Will return object of TblUser</returns>
        public ActionResult MyProfile()
        {
            TblUser objuser = new TblUser();
            string passphrase = ConfigurationManager.AppSettings["PassPhrase"];
            objuser = (from u in context.TblUsers where u.UserId == Convert.ToInt32(SunCity.Core.Session.Current.UserId) select u).FirstOrDefault();
            objuser.Userpwd = SunCity.Core.UtilityFunction.DecryptString(objuser.Userpwd, passphrase);
            objuser.listRole = objuser.getListRole();


            return View(objuser);
        }
        /// <summary>
        /// Below function will edit the user profile as per the info. passed to the system.
        /// </summary>
        /// <param name="FrmUser">TblUser object is passed on to the function</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MyProfile(TblUser FrmUser)
        {
            TblUser objuser = new TblUser();
            string passphrase = ConfigurationManager.AppSettings["PassPhrase"];

            objuser = (from k in context.TblUsers where k.UserId == Convert.ToInt32(SunCity.Core.Session.Current.UserId) select k).FirstOrDefault();
            

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
            //Log Activity
            using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
            {
                string msg = SunCity.Core.Session.Current.EmployeeName + " updated his profile";
                SunCity.Core.UtilityFunction.LogActivity(msg, sw);
            }
            return RedirectToAction("MyProfile","Accounts");
        }

        public ActionResult Testfun()
        {
            DateTime saveNow = DateTime.Now;
            DateTime saveUtcNow = DateTime.UtcNow;

            DateTime IndiaUtcNow = DateTime.UtcNow.AddHours(5.50);

            ViewData["saveNow"] = saveNow;
            ViewData["saveUtcNow"]=saveUtcNow;
            ViewData["IndiaUtcNow"]=IndiaUtcNow;

            IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
            var result = writer.Write("Hello");
            var barcodeBitmap = new Bitmap(result);
            barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/Hello.png"));
            //pictureBox1.Image = barcodeBitmap;

            return View();
        }
    }
}
