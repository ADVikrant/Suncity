using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SunCity.Models;
using SunCity.DAL;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections;
using System.Configuration;
using AlphaDez.TemplateParser;
using SunCity.Core;
using ZXing;
using System.Drawing;
namespace SunCity.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/
        SuncityDataContext context = new SuncityDataContext();
        /// <summary>
        /// Below action will make the Dashboard view of the system. It will show the ActivityCounter,MembersCounter,MembershipCounter,MembershipPlanCounter,UserCounter
        ///  etc.. Moreover it also checks for automatic blocking of membership  and it's member if it is not renewed.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Dashboard objdashboard = new Dashboard();

            objdashboard.ActivityCounter = (from a in context.TblActivities where a.IsDeleted == false select a).Count();
            objdashboard.MembersCounter = (from m in context.TempMembers where m.IsDeleted == false select m).Count();
            objdashboard.MembershipCounter = (from mb in context.TempMembershipRegs where mb.IsDeleted == false select mb).Count();
            objdashboard.MembershipPlanCounter = (from p in context.TblMembershipPlans where p.IsDeleted == false select p).Count();
            objdashboard.PackageCounter = (from pk in context.TblPackages where pk.IsDeleted == false select pk).Count();
            objdashboard.UserCounter = (from u in context.TblUsers where u.IsDeleted == false select u).Count();

            objdashboard.AmountTillDate = Convert.ToDecimal(context.TempMembershipRegs.Sum(x => x.Amount));

            //Create Activity log folder and create a log file to keep track of records. First check for exist of file and folder if not then create one
            var path = Server.MapPath("~/ActivityLog/");
            var foldername = DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy");
            if (!Directory.Exists(path + foldername))
            {
                Directory.CreateDirectory(path + foldername);
            }
            string filename = DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt";
            if (!System.IO.File.Exists(path + foldername + "/" + filename))
            {
                System.IO.File.Create(path + foldername + "/" + filename);
            }

            
            return View(objdashboard);
        }
        /// <summary>
        /// This json call will get the next upcoming event and will pass the data to show on dashboard as a notification
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUpcomingfestival()
        {
            Festival objfest = new Festival();
            var getfest = (from fe in context.TblFestivals
                           where fe.FestivalDate > DateTime.UtcNow.AddHours(5.50)
                               && fe.IsDeleted == false
                           select fe).FirstOrDefault();
            if (getfest != null)
            {
                objfest.NotificationId = getfest.NotificationId;
                DateTime strdt = Convert.ToDateTime(getfest.FestivalDate);
                objfest.FestivalDate = strdt.ToString("dd-MM-yyyy");
                objfest.FestivalName = getfest.FestivalName;
                objfest.FestivalNote = getfest.FestivalNote;
            }
            return Json(objfest, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Fucntion to send email notification to members of the club. Just pass the Memberid and the greeting type i.e if birthday greeting then greetingtype=1 and if marriage anniversary then greetingtype=2
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="greetingtype"></param>
        /// <returns></returns>
        public ActionResult SendEmailNotification(int MemberId = 0, int greetingtype = 0)
        {
            try
            {
                string m_listOfFiles = string.Empty;
                String MailTo = string.Empty;
                String MailToName = string.Empty;
                string MailSubject = string.Empty;

                var getmemberdetail = (from m in context.TempMembers where m.TempMemberId == MemberId select m).FirstOrDefault();
                var lstgetmemberdetail = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(getmemberdetail.UserDataobj);
                //get main member details

                //var getmembership = (from o in context.TempMembershipRegs where o.TempMembershipRegistrationId == getmemberdetail.TempMembershipRegistrationId select o).FirstOrDefault();

                var getname = (from m in context.TempMembers where m.TempMembershipRegistrationId == getmemberdetail.TempMembershipRegistrationId orderby m.TempMemberId select m).FirstOrDefault();
                var lstgetname = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(getname.UserDataobj);

                Hashtable hshDetails = new Hashtable();
                hshDetails.Add("MemberName", lstgetmemberdetail[0].value);

                MailTo = lstgetname[9].value;
                MailToName = lstgetmemberdetail[0].value;
                string TemplatePath="";
                if (greetingtype == 1)
                {
                    MailSubject = "Birthday Greetings - Suncity Club";
                    TemplatePath = Request.PhysicalApplicationPath.ToString() + ConfigurationManager.AppSettings["Birthday"].ToString();
                }
                else if (greetingtype == 2)
                {
                    MailSubject = "Anniversary Greetings - Suncity Club";
                    TemplatePath = Request.PhysicalApplicationPath.ToString() + ConfigurationManager.AppSettings["Anniversary"].ToString();
                }
                Parser parserData = new Parser(TemplatePath, hshDetails);
                bool isMailSent = SunCity.Core.UtilityFunction.SendHtmlMail(MailTo, MailToName, MailSubject, parserData.Parse(), "");
                if (isMailSent == true)
                {
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("false", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Dashboard"); 
                return Json("false", JsonRequestBehavior.AllowGet); 
            }
        }

        public ActionResult CheckConditions()
        {
            // Check the condition for 18+ child and if found then block the member.
            var get18member = (from me in context.TempMembers where me.UserTypeId == Convert.ToInt32(SunCity.Core.Enum.UserType.Child) select me).ToList();
            foreach (var item in get18member)
            {
                var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(item.UserDataobj);
                var dobdate = lsta[3].value;
                DateTime dtcompare = Convert.ToDateTime(dobdate).Date;
                DateTime dt = DateTime.UtcNow.AddHours(5.50).Date;
                var ag = dt.Year - dtcompare.Year;
                if (dt.Year - dtcompare.Year >= 18)
                { 
                    //block the current user whose age hase gone past 18
                    item.IsBlocked = true;
                    item.IsDeleted = true;
                    context.SubmitChanges();
                }
            }
            


            //Code to auto deactivate  membership once  date have passed
            var getmembership = (from m in context.TempMembershipRegs where m.TempEndDate == DateTime.UtcNow.AddHours(5.50).Date select m).ToList();
            foreach (var item in getmembership)
            {
                //Set is deleted to true for those membership
                item.IsDeleted = true;
                context.SubmitChanges();

                //get list of members of the same membership
                var getmembers = (from me in context.TempMembers where me.TempMembershipRegistrationId == item.TempMembershipRegistrationId select me).ToList();
                foreach (var itemmember in getmembers)
                {
                    itemmember.IsDeleted = true;
                    itemmember.IsBlocked = true;
                    context.SubmitChanges();
                }
            }

            return View();
        }

        /// <summary>
        /// Below action is used to get the birthdate and the marriage anniversary details of the members enrolled.
        /// </summary>
        /// <returns></returns>
        public ActionResult AsyncNoti()
        {
            //get all the members
            NotifyModel objnoti = new NotifyModel();
            DisplayFields objdisp = new DisplayFields();
            List<DisplayFields> bdaylist = new List<DisplayFields>();
            List<DisplayFields> mrglist = new List<DisplayFields>();
            var getallmembers = (from m in context.TempMembers where m.IsDeleted == false orderby m.TempMemberId descending select m).ToList();
            foreach (var item in getallmembers)
            {
                var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(item.UserDataobj);
                objdisp = new DisplayFields();
                if (item.UserTypeId == 1)
                {

                    var dobdate = lsta[11].value;
                    DateTime dtcompare = Convert.ToDateTime(dobdate);
                    var dtcompareday = dtcompare.Day;
                    var dtcomparemonth = dtcompare.Month;
                    DateTime dt = DateTime.UtcNow.AddHours(5.50);
                    var dttoday = dt.Day;
                    var dttomonth = dt.Month;

                    //if (dobdate == dt.ToString("yyyy-MM-dd"))
                    if ((dtcompareday == dttoday) && (dtcomparemonth == dttomonth))
                    {
                        objdisp.MemberId = item.TempMemberId;
                        objdisp.Membername = lsta[0].value + " " + lsta[1].value + " " + lsta[2].value;
                        bdaylist.Add(objdisp);
                    }

                    var marriagedate = lsta[15].value;
                    if (marriagedate != "")
                    {
                        DateTime mardtcompare = Convert.ToDateTime(marriagedate);
                        var mardtcompareday = mardtcompare.Day;
                        var mardtcomparemonth = mardtcompare.Month;
                        DateTime mardt = DateTime.UtcNow.AddHours(5.50);
                        var mardttoday = mardt.Day;
                        var mardttomonth = mardt.Month;

                        //if (marriagedate == dt.ToString("yyyy-MM-dd"))
                        if ((mardtcompareday == mardttoday) && (mardtcomparemonth == mardttomonth))
                        {
                            objdisp.MemberId = item.TempMemberId;
                            objdisp.Membername = lsta[0].value + " " + lsta[1].value + " " + lsta[2].value;
                            mrglist.Add(objdisp);
                        }
                    }
                }
                else if (item.UserTypeId == 2)
                {
                    var dobdate = lsta[4].value;
                    DateTime dtcompare = Convert.ToDateTime(dobdate);
                    var dtcompareday = dtcompare.Day;
                    var dtcomparemonth = dtcompare.Month;
                    DateTime dt = DateTime.UtcNow.AddHours(5.50);
                    var dttoday = dt.Day;
                    var dttomonth = dt.Month;
                    //if (dobdate == dt.ToString("yyyy-MM-dd"))
                    if ((dtcompareday == dttoday) && (dtcomparemonth == dttomonth))
                    {
                        objdisp.MemberId = item.TempMemberId;
                        objdisp.Membername = lsta[0].value + " " + lsta[1].value + " " + lsta[2].value;
                        bdaylist.Add(objdisp);
                    }


                }
                else if (item.UserTypeId == 3)
                {
                    var dobdate = lsta[3].value;
                    DateTime dtcompare = Convert.ToDateTime(dobdate);
                    var dtcompareday = dtcompare.Day;
                    var dtcomparemonth = dtcompare.Month;
                    DateTime dt = DateTime.UtcNow.AddHours(5.50);
                    var dttoday = dt.Day;
                    var dttomonth = dt.Month;
                    //if (dobdate == dt.ToString("yyyy-MM-dd"))
                    if ((dtcompareday == dttoday) && (dtcomparemonth == dttomonth))
                    {
                        objdisp.MemberId = item.TempMemberId;
                        objdisp.Membername = lsta[0].value;
                        bdaylist.Add(objdisp);
                    }


                }
                else if (item.UserTypeId == 4)
                {
                    var dobdate = lsta[3].value;
                    DateTime dtcompare = Convert.ToDateTime(dobdate);
                    var dtcompareday = dtcompare.Day;
                    var dtcomparemonth = dtcompare.Month;
                    DateTime dt = DateTime.UtcNow.AddHours(5.50);
                    var dttoday = dt.Day;
                    var dttomonth = dt.Month;
                    //if (dobdate == dt.ToString("yyyy-MM-dd"))
                    if ((dtcompareday == dttoday) && (dtcomparemonth == dttomonth))
                    {
                        objdisp.MemberId = item.TempMemberId;
                        objdisp.Membername = lsta[0].value;
                        bdaylist.Add(objdisp);
                    }


                }

            }
            objnoti.listbirthday = bdaylist;
            objnoti.listmarriage = mrglist;
            return View(objnoti);
        }
        /// <summary>
        /// Below action is used to store the userprofile image of the the user. It will add the current date time and bind it before the filename so that the files are not overwritten
        /// </summary>
        /// <param name="fileData">fileData will contain necessary info to store the file on to the server folder</param>
        /// <returns>filename </returns>
        public string StoreImage(HttpPostedFileBase fileData)
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
        /// <summary>
        /// Below action is used to store the membership documents of the the user. It will add the current date time and bind it before the filename so that the files are not overwritten
        /// </summary>
        /// <param name="fileData">fileData will contain necessary info to store the file on to the server folder</param>
        /// <returns>filename </returns>
        public string StoreDocs(HttpPostedFileBase fileData)
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
                var currpath = Path.Combine(Server.MapPath("~/DocumentFolder/"), imageName);
                fileData.SaveAs(currpath);
                return imageName;
            }
            else
            {
                return string.Empty;
            }
        }

        public ActionResult tupload()
        {

            return View();
        }

        /// <summary>
        /// Below function is used to download a file which is attached during the enrollment of membership
        /// </summary>
        /// <param name="docid">id of the file to be downloaded</param>
        /// <returns></returns>
        public FileResult Download(int docid)
        {

            string filepatth = Server.MapPath("~/DocumentFolder/");
            string CurrentFileName = (from fls in context.TblDocuments
                                      where fls.DocumentId == docid
                                      select fls.DocumentName).FirstOrDefault();
            string fpath = filepatth + CurrentFileName;

            string contentType = string.Empty;

            if (fpath.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (fpath.Contains(".doc"))
            {
                contentType = "application/msword";
            }
            else if (fpath.Contains(".docx"))
            {
                contentType = "application/docx";
            }
            else if (fpath.Contains(".xls"))
            {
                contentType = "application/vnd.ms-excel";
            }
            else if (fpath.Contains(".jpeg"))
            {
                contentType = "image/jpeg";
            }
            else if (fpath.Contains(".jpg"))
            {
                contentType = "image/jpeg";
            }
            else if (fpath.Contains(".png"))
            {
                contentType = "image/png";
            }
            else if (fpath.Contains(".txt"))
            {
                contentType = "text/plain";
            }
            return File(fpath, contentType, CurrentFileName);

        }
        [HttpPost]
        public ActionResult tupload(int i = 0)
        {

            return Content("true");
        }


        public ActionResult TestMembershipPlan(int MembershipPlanId = 0)
        {

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

        public ActionResult TestAddRegister(int MembershipPlanId = 0)
        {
            TblMembershipRegistration objgetregdetails = new TblMembershipRegistration();
            objgetregdetails.listMembershipPlan = objgetregdetails.getListMembershipPlan();
            objgetregdetails.listPackages = objgetregdetails.getListPackage();
            return View(objgetregdetails);

        }

        public ActionResult GetPlanDetails(string planid)
        {
            //string orderid = Request["orderID"];
            var sentresult = (from m in context.TblMembershipPlans where m.MembershipPlanId == Convert.ToInt32(planid) select m).FirstOrDefault();

            return Json(sentresult, JsonRequestBehavior.AllowGet);

        }

        public ActionResult TempMembershipPlan()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TempMembershipReg objgetregdetails = new TempMembershipReg();

            objgetregdetails.templistMembershipPlan = objgetregdetails.tempgetListMembershipPlan();
            objgetregdetails.listPackages = objgetregdetails.getListPackage();
            return View(objgetregdetails);


        }

        [HttpPost]
        public ActionResult TempMembershipPlan(TempMembershipReg FrmMembershipRegistration, int MembershipRegestrationId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            try
            {

                ModelTempMembershipPlan objm = new ModelTempMembershipPlan();
                var getm = (from m in context.TempMembershipPlans where m.TempPlanId == FrmMembershipRegistration.TempPlanId select m).FirstOrDefault();
                if (getm != null)
                {
                    objm.CreatedBy = Convert.ToInt32(getm.CreatedBy);
                    objm.CreatedDate = Convert.ToDateTime(getm.CreatedDate);
                    objm.IsDeleted = Convert.ToBoolean(getm.IsDeleted);
                    objm.TempAmount = Convert.ToDecimal(getm.TempAmount);
                    objm.TempPeriod = Convert.ToInt32(getm.TempPeriod);
                    objm.TempPlanId = getm.TempPlanId;
                    objm.TempPlanName = getm.TempPlanName;

                }

                //get the usertype list from mapplanusertype table
                var getusertypelist = (from l in context.TempMapPlanUserTypes where l.TempPlanId == getm.TempPlanId select l).ToList();
                List<MapPlanUserType> lstmapplanusertype = new List<MapPlanUserType>();
                MapPlanUserType objmapusertype = new MapPlanUserType();
                List<MapFieldUser> listmapfielduser = new List<MapFieldUser>();
                MapFieldUser objmapfielduser = new MapFieldUser();
                foreach (var item in getusertypelist)
                {
                    //Count the usertype
                    objmapusertype = new MapPlanUserType();
                    objmapusertype.MapPlanUserTypeId = item.MapPlanUserTypeId;
                    objmapusertype.TempPlanId = Convert.ToInt32(item.TempPlanId);
                    objmapusertype.Usercount = Convert.ToInt32(item.Usercount);
                    objmapusertype.UserTypeId = Convert.ToInt32(item.UserTypeId);
                    lstmapplanusertype.Add(objmapusertype);

                    //Bind the fields

                    var getmapplanusertype = (from gm in context.TblMapFieldUserTypes
                                              join
                                                  cm in context.TblFields on gm.FieldId equals cm.FieldId
                                              where gm.UserTypeId == item.UserTypeId
                                              select new { gm, cm }).ToList();

                    foreach (var itemf in getmapplanusertype)
                    {
                        objmapfielduser = new MapFieldUser();
                        objmapfielduser.fieldid = itemf.cm.FieldId;
                        objmapfielduser.FieldName = itemf.cm.FieldName;
                        objmapfielduser.FieldText = itemf.cm.FieldText;
                        objmapfielduser.FieldType = itemf.cm.FieldType;
                        objmapfielduser.UserTypeId = Convert.ToInt32(item.UserTypeId);
                        listmapfielduser.Add(objmapfielduser);
                    }
                }
                objm.Mapplanusertypelist = lstmapplanusertype;
                objm.Mapfielduser = listmapfielduser;
                var getusertypecout = lstmapplanusertype.Count;

                //Get posted values

                TempMembershipReg objreg = new TempMembershipReg();

                objreg.CreatedBy = SunCity.Core.Session.Current.UserId;
                objreg.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                objreg.IsDeleted = false;
                DateTime stdt = Convert.ToDateTime(FrmMembershipRegistration.TempStartDate);
                objreg.TempStartDate = FrmMembershipRegistration.TempStartDate;
                var getmembershiplan = (from m in context.TempMembershipPlans where m.TempPlanId == FrmMembershipRegistration.TempPlanId select m).FirstOrDefault();
                objreg.TempEndDate = stdt.AddYears(Convert.ToInt32(getmembershiplan.TempPeriod));
                objreg.TempPlanId = FrmMembershipRegistration.TempPlanId;
                objreg.TempPackageId = FrmMembershipRegistration.TempPackageId;
                objreg.Amount = getmembershiplan.TempAmount;
                context.TempMembershipRegs.InsertOnSubmit(objreg);
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
                        objdocs.MembershipRegistrationId = objreg.TempMembershipRegistrationId;
                        context.TblDocuments.InsertOnSubmit(objdocs);
                        context.SubmitChanges();
                    }

                }

                List<GetPostValue> listgetpost = new List<GetPostValue>();
                GetPostValue objgetpo = new GetPostValue();
                List<KeyValue> listkey = new List<KeyValue>();
                for (int k = 0; k < getusertypecout; k++)
                {
                    if (objm.Mapplanusertypelist[k].UserTypeId == 1)
                    {
                        if (objm.Mapplanusertypelist[k].Usercount > 0)
                        {
                            //Check for no. of user for particular usertype
                            for (int c = 0; c < objm.Mapplanusertypelist[k].Usercount; c++)
                            {
                                TempMember objtempmem = new TempMember();
                                objtempmem.TempMembershipRegistrationId = objreg.TempMembershipRegistrationId;
                                objtempmem.UserTypeId = objm.Mapplanusertypelist[k].UserTypeId;
                                objtempmem.IsDeleted = false;
                                context.TempMembers.InsertOnSubmit(objtempmem);
                                context.SubmitChanges();

                                //Add code to generate barcode and save it in the db
                                IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
                                var result = writer.Write("Member#" + objtempmem.TempMemberId);
                                var barcodeBitmap = new Bitmap(result);
                                barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/MemberId-" + objtempmem.TempMemberId + ".png"));
                                objtempmem.BarcodeImage = "MemberId-" + objtempmem.TempMemberId + ".png";
                                context.SubmitChanges();

                                //Enter the picture in picture table

                                TblProfilePicture objpic = new TblProfilePicture();
                                objpic.ProfiePictureName = Request["hiddenmem" + c + "photo"].ToString();
                                objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                                objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                                context.TblProfilePictures.InsertOnSubmit(objpic);
                                context.SubmitChanges();

                                //Enter the member-picture entry in diff table
                                TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                                objmappic.MemberId = objtempmem.TempMemberId;
                                objmappic.ProfilePictureId = objpic.ProfilePictureId;
                                context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                                context.SubmitChanges();

                                listkey = new List<KeyValue>();

                                objgetpo = new GetPostValue();

                                objgetpo.memberid = objtempmem.TempMemberId;
                                //objgetpo.memberid = k;


                                KeyValue keyv = new KeyValue();



                                for (int j = 0; j < objm.Mapfielduser.Count; j++)
                                {
                                    keyv = new KeyValue();
                                    if (objm.Mapfielduser[j].UserTypeId == objm.Mapplanusertypelist[k].UserTypeId)
                                    {
                                        if (objm.Mapfielduser[j].FieldType == "select")
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            if (objm.Mapfielduser[j].FieldText == "Sex")
                                            {
                                                getvar = Request["mem_" + c + "_gen"];
                                            }
                                            else if (objm.Mapfielduser[j].FieldText == "Maritial Status")
                                            {
                                                getvar = Request["mem_" + c + "_mar"];
                                            }
                                            keyv.value = getvar;
                                        }
                                        else
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Request["mem_" + c + "_" + objm.Mapfielduser[j].FieldName];
                                            keyv.value = getvar;
                                        }
                                        listkey.Add(keyv);
                                    }
                                }

                                objgetpo.listkey = listkey;
                                listgetpost.Add(objgetpo);
                                var jsono = new JavaScriptSerializer().Serialize(listkey);
                                objtempmem.UserDataobj = jsono;
                                context.SubmitChanges();
                            }
                        }

                    }
                    if (objm.Mapplanusertypelist[k].UserTypeId == 2)
                    {
                        if (objm.Mapplanusertypelist[k].Usercount > 0)
                        {
                            //Check for no. of user for particular usertype
                            for (int c = 0; c < objm.Mapplanusertypelist[k].Usercount; c++)
                            {
                                TempMember objtempmem = new TempMember();
                                objtempmem.TempMembershipRegistrationId = objreg.TempMembershipRegistrationId;
                                objtempmem.UserTypeId = objm.Mapplanusertypelist[k].UserTypeId;
                                objtempmem.IsDeleted = false;
                                context.TempMembers.InsertOnSubmit(objtempmem);
                                context.SubmitChanges();

                                //Add code to generate barcode and save it in the db
                                IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
                                var result = writer.Write("Member#" + objtempmem.TempMemberId);
                                var barcodeBitmap = new Bitmap(result);
                                barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/MemberId-" + objtempmem.TempMemberId + ".png"));
                                objtempmem.BarcodeImage = "MemberId-" + objtempmem.TempMemberId + ".png";
                                context.SubmitChanges();

                                //Enter the picture in picture table

                                TblProfilePicture objpic = new TblProfilePicture();
                                objpic.ProfiePictureName = Request["shiddenmem" + c + "photo"].ToString();
                                objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                                objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                                context.TblProfilePictures.InsertOnSubmit(objpic);
                                context.SubmitChanges();

                                //Enter the member-picture entry in diff table
                                TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                                objmappic.MemberId = objtempmem.TempMemberId;
                                objmappic.ProfilePictureId = objpic.ProfilePictureId;
                                context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                                context.SubmitChanges();


                                listkey = new List<KeyValue>();

                                objgetpo = new GetPostValue();

                                objgetpo.memberid = objtempmem.TempMemberId;
                                //objgetpo.memberid = k;


                                KeyValue keyv = new KeyValue();



                                for (int j = 0; j < objm.Mapfielduser.Count; j++)
                                {
                                    keyv = new KeyValue();
                                    if (objm.Mapfielduser[j].UserTypeId == objm.Mapplanusertypelist[k].UserTypeId)
                                    {

                                        var getvar = "";
                                        keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                        //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                        getvar = Request["smem_" + c + "_" + objm.Mapfielduser[j].FieldName];
                                        keyv.value = getvar;
                                        listkey.Add(keyv);
                                    }
                                }

                                objgetpo.listkey = listkey;
                                listgetpost.Add(objgetpo);
                                var jsono = new JavaScriptSerializer().Serialize(listkey);
                                objtempmem.UserDataobj = jsono;
                                context.SubmitChanges();
                            }
                        }

                    }
                    if (objm.Mapplanusertypelist[k].UserTypeId == 3)
                    {
                        if (objm.Mapplanusertypelist[k].Usercount > 0)
                        {
                            //Check for no. of user for particular usertype
                            for (int c = 0; c < objm.Mapplanusertypelist[k].Usercount; c++)
                            {
                                TempMember objtempmem = new TempMember();
                                objtempmem.TempMembershipRegistrationId = objreg.TempMembershipRegistrationId;
                                objtempmem.UserTypeId = objm.Mapplanusertypelist[k].UserTypeId;
                                objtempmem.IsDeleted = false;
                                context.TempMembers.InsertOnSubmit(objtempmem);
                                context.SubmitChanges();

                                //Add code to generate barcode and save it in the db
                                IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
                                var result = writer.Write("Member#" + objtempmem.TempMemberId);
                                var barcodeBitmap = new Bitmap(result);
                                barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/MemberId-" + objtempmem.TempMemberId + ".png"));
                                objtempmem.BarcodeImage = "MemberId-" + objtempmem.TempMemberId + ".png";
                                context.SubmitChanges();

                                //Enter the picture in picture table

                                TblProfilePicture objpic = new TblProfilePicture();
                                objpic.ProfiePictureName = Request["chiddenmem" + c + "photo"].ToString();
                                objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                                objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                                context.TblProfilePictures.InsertOnSubmit(objpic);
                                context.SubmitChanges();

                                //Enter the member-picture entry in diff table
                                TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                                objmappic.MemberId = objtempmem.TempMemberId;
                                objmappic.ProfilePictureId = objpic.ProfilePictureId;
                                context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                                context.SubmitChanges();

                                listkey = new List<KeyValue>();

                                objgetpo = new GetPostValue();

                                objgetpo.memberid = objtempmem.TempMemberId;
                                //objgetpo.memberid = k;


                                KeyValue keyv = new KeyValue();



                                for (int j = 0; j < objm.Mapfielduser.Count; j++)
                                {
                                    keyv = new KeyValue();
                                    if (objm.Mapfielduser[j].UserTypeId == objm.Mapplanusertypelist[k].UserTypeId)
                                    {
                                        if (objm.Mapfielduser[j].FieldType == "select")
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Request["cmem_" + c + "_gen"];
                                            keyv.value = getvar;
                                        }
                                        else
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Request["cmem_" + c + "_" + objm.Mapfielduser[j].FieldName];
                                            keyv.value = getvar;

                                        }
                                        listkey.Add(keyv);
                                    }
                                }

                                objgetpo.listkey = listkey;
                                listgetpost.Add(objgetpo);
                                var jsono = new JavaScriptSerializer().Serialize(listkey);
                                objtempmem.UserDataobj = jsono;
                                context.SubmitChanges();
                            }
                        }

                    }
                    if (objm.Mapplanusertypelist[k].UserTypeId == 4)
                    {
                        if (objm.Mapplanusertypelist[k].Usercount > 0)
                        {
                            //Check for no. of user for particular usertype
                            for (int c = 0; c < objm.Mapplanusertypelist[k].Usercount; c++)
                            {
                                TempMember objtempmem = new TempMember();
                                objtempmem.TempMembershipRegistrationId = objreg.TempMembershipRegistrationId;
                                objtempmem.UserTypeId = objm.Mapplanusertypelist[k].UserTypeId;
                                objtempmem.IsDeleted = false;
                                context.TempMembers.InsertOnSubmit(objtempmem);
                                context.SubmitChanges();

                                //Add code to generate barcode and save it in the db
                                IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
                                var result = writer.Write("Member#" + objtempmem.TempMemberId);
                                var barcodeBitmap = new Bitmap(result);
                                barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/MemberId-" + objtempmem.TempMemberId + ".png"));
                                objtempmem.BarcodeImage = "MemberId-" + objtempmem.TempMemberId + ".png";
                                context.SubmitChanges();


                                //Enter the picture in picture table

                                TblProfilePicture objpic = new TblProfilePicture();
                                objpic.ProfiePictureName = Convert.ToString(Request["seniormem" + c + "photo"]);
                                objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                                objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                                context.TblProfilePictures.InsertOnSubmit(objpic);
                                context.SubmitChanges();

                                //Enter the member-picture entry in diff table
                                TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                                objmappic.MemberId = objtempmem.TempMemberId;
                                objmappic.ProfilePictureId = objpic.ProfilePictureId;
                                context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                                context.SubmitChanges();

                                listkey = new List<KeyValue>();

                                objgetpo = new GetPostValue();

                                objgetpo.memberid = objtempmem.TempMemberId;
                                //objgetpo.memberid = k;


                                KeyValue keyv = new KeyValue();



                                for (int j = 0; j < objm.Mapfielduser.Count; j++)
                                {
                                    keyv = new KeyValue();
                                    if (objm.Mapfielduser[j].UserTypeId == objm.Mapplanusertypelist[k].UserTypeId)
                                    {
                                        if (objm.Mapfielduser[j].FieldType == "select")
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Request["semem_" + c + "_gen"];
                                            keyv.value = getvar;
                                        }
                                        else
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Request["semem_" + c + "_" + objm.Mapfielduser[j].FieldName];
                                            keyv.value = getvar;

                                        }
                                        listkey.Add(keyv);
                                    }
                                }

                                objgetpo.listkey = listkey;
                                listgetpost.Add(objgetpo);
                                var jsono = new JavaScriptSerializer().Serialize(listkey);
                                objtempmem.UserDataobj = jsono;
                                context.SubmitChanges();
                            }

                        }
                    }
                }
                //TempMapMember objmap = new TempMapMember();
                //foreach (var item in listgetpost)
                //{
                //    objmap = new TempMapMember();
                //    objmap.TempMemberId = item.memberid;
                //    var json = new JavaScriptSerializer().Serialize(item.listkey);
                //    objmap.UserDataobj = json;
                //    context.TempMapMembers.InsertOnSubmit(objmap);
                //    context.SubmitChanges();
                //}

                TempData["alertstatus"] = "Success";
                return RedirectToAction("AddMembershipRegistration", "Function", new { MembershipRegestrationId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return RedirectToAction("AddMembershipRegistration", "Function", new { MembershipRegestrationId = 0 });
            }


        }


        public ActionResult TempViewMembership(int? MembershipRegestrationId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMembership)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }


            TempViewRegModel objmodel = new TempViewRegModel();
            objmodel.objtempreg = (from c in context.TempMembershipRegs where c.TempMembershipRegistrationId == MembershipRegestrationId select c).FirstOrDefault();
            objmodel.listtempmember = (from m in context.TempMembers where m.TempMembershipRegistrationId == MembershipRegestrationId select m).ToList();
            objmodel.listMembershipPlan = objmodel.getListMembershipPlan();
            objmodel.listPackages = objmodel.getListPackage();
            objmodel.listdocuments = (from d in context.TblDocuments where d.MembershipRegistrationId == MembershipRegestrationId select d).ToList();
            List<TempViewRegModel.DeserializeMemberObj> listdes = new List<TempViewRegModel.DeserializeMemberObj>();
            TempViewRegModel.DeserializeMemberObj objd = new TempViewRegModel.DeserializeMemberObj();



            foreach (var item in objmodel.listtempmember)
            {
                objd = new TempViewRegModel.DeserializeMemberObj();
                List<TempViewRegModel.KeyValue> lst = new List<TempViewRegModel.KeyValue>();
                var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(item.UserDataobj);
                //        lst = jsono;
                objd.MemberId = item.TempMemberId;
                objd.TempregistrationId = Convert.ToInt32(item.TempMembershipRegistrationId);
                objd.UserTypeId = Convert.ToInt32(item.UserTypeId);
                objd.debobj = lsta;
                listdes.Add(objd);

            }
            objmodel.lstdeserialize = listdes;

            return View(objmodel);
        }

        [HttpPost]
        public ActionResult TempViewMembership(TempViewRegModel TempViewRegModel, int? MembershipRegestrationId)
        {
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
                    objdocs.MembershipRegistrationId = TempViewRegModel.objtempreg.TempMembershipRegistrationId;
                    context.TblDocuments.InsertOnSubmit(objdocs);
                    context.SubmitChanges();
                }

            }

            TempData["alertstatus"] = "Success";
            return RedirectToAction("TempViewMembership", "Dashboard", new { MembershipRegestrationId = TempViewRegModel.objtempreg.TempMembershipRegistrationId });
        }




        public ActionResult TempEditMember(int MemberId = 0, int rtflag = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditMember)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TempMember objgetmember = new TempMember();

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

            List<SunCity.Models.TempViewRegModel.MrgOption> listmrg = new List<SunCity.Models.TempViewRegModel.MrgOption>();
            SunCity.Models.TempViewRegModel.MrgOption objmrg = new SunCity.Models.TempViewRegModel.MrgOption();
            objmrg = new SunCity.Models.TempViewRegModel.MrgOption();
            objmrg.mrgname = "Married";
            objmrg.mrgvalue = "Married";
            listmrg.Add(objmrg);
            objmrg = new SunCity.Models.TempViewRegModel.MrgOption();
            objmrg.mrgname = "Unmarried";
            objmrg.mrgvalue = "Unmarried";
            listmrg.Add(objmrg);



            objgetmember = (from o in context.TempMembers where o.TempMemberId == MemberId select o).FirstOrDefault();
            TempViewRegModel.DeserializeMemberObj objd = new TempViewRegModel.DeserializeMemberObj();

            if (SunCity.Core.Session.Current.flgrt == 0)
            {
                SunCity.Core.Session.Current.flgrt = rtflag;
            }
            if (MemberId == 0)
            {
                objd = new TempViewRegModel.DeserializeMemberObj();
                objd.listgender = listgender;
                objd.listmarriage = listmrg;
                return View(objd);
            }
            else
            {
                objd = new TempViewRegModel.DeserializeMemberObj();
                objd.listgender = listgender;
                objd.listmarriage = listmrg;
                var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(objgetmember.UserDataobj);
                //        lst = jsono;
                objd.MemberId = objgetmember.TempMemberId;
                objd.TempregistrationId = Convert.ToInt32(objgetmember.TempMembershipRegistrationId);
                objd.UserTypeId = Convert.ToInt32(objgetmember.UserTypeId);
                objd.debobj = lsta;


                //Get profile details
                var getprofile = (from p in context.TblProfilePictures
                                  join
                                  m in context.TblMapMemberProfilePictures on p.ProfilePictureId equals m.ProfilePictureId
                                  where m.MemberId == Convert.ToInt32(objgetmember.TempMemberId)
                                  select p).FirstOrDefault();
                if (getprofile != null)
                {
                    objd.profilePictureId = getprofile.ProfilePictureId;
                    objd.ProfilePictreName = getprofile.ProfiePictureName;
                }
                return View(objd);
            }

        }

        [HttpPost]
        public ActionResult TempEditMember(TempViewRegModel.DeserializeMemberObj getobjde, int MemberId = 0, int rtflag = 0)
        {
            try
            {

                //Block to cancel the changes
                if (Request["hiddencancelflag"] != "0")
                {
                    TempMember tempobjmembers = new TempMember();
                    if (MemberId != 0)
                    {
                        tempobjmembers = (from k in context.TempMembers where k.TempMemberId == MemberId select k).FirstOrDefault();
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("TempViewMember", "Dashboard", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("TempViewMembership", "Dashboard", new { MembershipRegestrationId = tempobjmembers.TempMembershipRegistrationId });
                    }

                }


                if (Request["hiddencorporateflag"] == "0")
                {
                    TempMember objmembers = new TempMember();
                    if (MemberId != 0)
                    {
                        objmembers = (from k in context.TempMembers where k.TempMemberId == MemberId select k).FirstOrDefault();
                    }
                    var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(objmembers.UserDataobj);
                    for (int l = 0; l < lsta.Count; l++)
                    {
                        lsta[l].value = getobjde.debobj[l].value;
                    }
                    var jsono = new JavaScriptSerializer().Serialize(lsta);
                    objmembers.UserDataobj = jsono;


                    context.SubmitChanges();
                    TempData["alertstatus"] = "Success";
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Member:" + getobjde.debobj[1].value + " " + getobjde.debobj[2].value + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("TempViewMember", "Dashboard", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("TempViewMembership", "Dashboard", new { MembershipRegestrationId = objmembers.TempMembershipRegistrationId });
                    }
                }

                return RedirectToAction("TempViewMember", "Dashboard", new { MemberId = 0 });
            }
            catch (Exception ex)
            {

                TempData["alertstatus"] = "Error";
                return View();
            }

        }

        //-------------Block for New Membership Plan Coding Starts Here-----------------------------------------
        /// <summary>
        /// Below function will check for the rights to view membership plan and if it has rights then only it will be allowed to view the page else will be sent to access denied view.
        /// </summary>
        /// <returns></returns>

        public ActionResult NewMembershipPlan(int MembershipPlanId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TempMembershipPlan objmembershipplan = new TempMembershipPlan();

            objmembershipplan.IsDeleted = false;
            objmembershipplan.templistUsertype = objmembershipplan.tempgetListUsertype();
            return View(objmembershipplan);
        }
        /// <summary>
        /// Below action is used to add new plan to the system. It takes as input the object of membership plan and then adds it to the system. Moreover we do log that action into the text file of the created date.
        /// </summary>
        /// <param name="objmembershipplan">the object will contain the necessary data to enroll a plan.</param>
        /// <param name="MembershipPlanId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewMembershipPlan(TempMembershipPlan objmembershipplan, int MembershipPlanId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.AddMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                TempMembershipPlan storeobj = new TempMembershipPlan();
                if (MembershipPlanId != 0)
                {
                    storeobj = (from k in context.TempMembershipPlans where k.TempPlanId == MembershipPlanId select k).FirstOrDefault();
                }
                storeobj.TempAmount = objmembershipplan.TempAmount;
                storeobj.TempPeriod = objmembershipplan.TempPeriod;
                storeobj.TempPlanName = objmembershipplan.TempPlanName;
                storeobj.TempMembershipCode = objmembershipplan.TempMembershipCode;
                storeobj.IsDeleted = false;
                if (MembershipPlanId == 0)
                {
                    storeobj.CreatedBy = SunCity.Core.Session.Current.UserId;
                    storeobj.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                    context.TempMembershipPlans.InsertOnSubmit(storeobj);
                    context.SubmitChanges();

                    //Insert values into the mapping table
                    storeobj.templistUsertype = objmembershipplan.tempgetListUsertype();
                    TempMapPlanUserType objmapuserplan = new TempMapPlanUserType();
                    for (int k = 1; k <= storeobj.templistUsertype.Count; k++)
                    {
                        objmapuserplan = new TempMapPlanUserType();
                        objmapuserplan.TempPlanId = storeobj.TempPlanId;
                        objmapuserplan.UserTypeId = storeobj.templistUsertype[k - 1].UserTypeId;
                        //var gt = Convert.ToInt32(Request["input_" + k].ToString());
                        objmapuserplan.Usercount = Convert.ToInt32(Request["input_" + k].ToString());
                        context.TempMapPlanUserTypes.InsertOnSubmit(objmapuserplan);
                        context.SubmitChanges();
                    }

                }

                TempData["alertstatus"] = "Success";

                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Membership Plan:" + storeobj.TempPlanName + " has been added by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("NewMembershipPlan", "Dashboard", new { MembershipPlanId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return RedirectToAction("NewMembershipPlan", "Dashboard", new { MembershipPlanId = 0 });
            }
        }
        /// <summary>
        /// Below function is used to view the Membership Plan. We need to pass the TempPlanId to retrive the data
        /// </summary>
        /// <param name="TempPlanId">Id of the Membership Plan</param>
        /// <returns></returns>
        public ActionResult EditMembershipPlan(int TempPlanId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            TempMembershipDetail modeldetail = new TempMembershipDetail();
            TempMembershipPlan objgetplan = new TempMembershipPlan();
            objgetplan = (from m in context.TempMembershipPlans where m.TempPlanId == TempPlanId select m).FirstOrDefault();

            modeldetail.CreatedBy = Convert.ToInt32(objgetplan.CreatedBy);
            modeldetail.CreatedDate = Convert.ToDateTime(objgetplan.CreatedDate);
            modeldetail.IsDeleted = Convert.ToBoolean(objgetplan.IsDeleted);
            modeldetail.TempAmount = Convert.ToDecimal(objgetplan.TempAmount);
            modeldetail.TempPeriod = Convert.ToInt32(objgetplan.TempPeriod);
            modeldetail.TempPlanId = objgetplan.TempPlanId;
            modeldetail.TempPlanName = objgetplan.TempPlanName;
            modeldetail.TempMembershipCode = objgetplan.TempMembershipCode;

            var getlst = (from m in context.TblUserTypes
                          join
                          n in context.TempMapPlanUserTypes on m.UserTypeId equals n.UserTypeId
                          where n.TempPlanId == objgetplan.TempPlanId
                          select new { m, n }).ToList();
            List<BindCounter> lstbind = new List<BindCounter>();
            BindCounter bnd = new BindCounter();
            foreach (var item in getlst)
            {
                bnd = new BindCounter();
                bnd.MembershipPlanId = Convert.ToInt32(item.n.TempPlanId);
                bnd.UserType = item.m.UserType;
                bnd.UserCount = Convert.ToInt32(item.n.Usercount);
                bnd.UserTypeId = Convert.ToInt32(item.n.UserTypeId);
                lstbind.Add(bnd);
            }
            modeldetail.ListBindCounter = lstbind;

            return View(modeldetail);
        }
        /// <summary>
        /// Below action is used to edit the membership plan entered. For that we need to pass the TempPlanId so that we can update the existing plan.
        /// </summary>
        /// <param name="getmembership">object used to update membership plan</param>
        /// <param name="TempPlanId">Id of the Membership Plan</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditMembershipPlan(TempMembershipDetail getmembership, int TempPlanId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                TempMembershipPlan setboj = new TempMembershipPlan();
                setboj = (from s in context.TempMembershipPlans where s.TempPlanId == TempPlanId select s).FirstOrDefault();

                setboj.TempPeriod = getmembership.TempPeriod;
                setboj.TempPlanName = getmembership.TempPlanName;
                setboj.TempAmount = getmembership.TempAmount;
                setboj.TempMembershipCode = getmembership.TempMembershipCode;
                context.SubmitChanges();


                var getlst = (from m in context.TblUserTypes
                              join
                              n in context.TempMapPlanUserTypes on m.UserTypeId equals n.UserTypeId
                              where n.TempPlanId == TempPlanId
                              select new { m, n }).ToList();
                List<BindCounter> lstbind = new List<BindCounter>();
                BindCounter bnd = new BindCounter();
                foreach (var item in getlst)
                {
                    bnd = new BindCounter();
                    bnd.MembershipPlanId = Convert.ToInt32(item.n.TempPlanId);
                    bnd.UserType = item.m.UserType;
                    bnd.UserCount = Convert.ToInt32(item.n.Usercount);
                    bnd.UserTypeId = Convert.ToInt32(item.n.UserTypeId);
                    lstbind.Add(bnd);
                }
                var ListBindCounter = lstbind;
                foreach (var item in ListBindCounter)
                {
                    TempMapPlanUserType obj = new TempMapPlanUserType();
                    obj = (from o in context.TempMapPlanUserTypes where o.TempPlanId == item.MembershipPlanId && o.UserTypeId == item.UserTypeId select o).FirstOrDefault();

                    obj.Usercount = Convert.ToInt32(Request["input_" + item.UserTypeId].ToString());
                    context.SubmitChanges();
                }

                TempData["alertstatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Membership Plan:" + setboj.TempPlanName + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("GridMembershipPlans", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return RedirectToAction("EditMembershipPlan", "Dashboard", new { TempPlanId = 0 });
            }
        }
        /// <summary>
        /// Below action is used to list down all the Membership Plan listed in the system
        /// </summary>
        /// <returns>Object containing list of Plans in the system</returns>
        public ActionResult GridMembershipPlans()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            List<TempMembershipPlan> listmem = new List<DAL.TempMembershipPlan>();
            listmem = (from u in context.TempMembershipPlans orderby u.TempPlanId descending select u).ToList();
            if (listmem != null)
            {
                return View(listmem);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Below action is used to delete a Plan in the system. Id  of the plan need to be send to delete the plan.
        /// </summary>
        /// <param name="TempPlanId">Id of the plan to be send to delete the plan</param>
        /// <returns></returns>
        public ActionResult DeleteMembershipPlan(int? TempPlanId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteMembershipPlan)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                TempMembershipPlan objmembershipplan = new TempMembershipPlan();
                if (TempPlanId != 0)
                {
                    objmembershipplan = (from k in context.TempMembershipPlans where k.TempPlanId == TempPlanId select k).FirstOrDefault();
                }
                objmembershipplan.IsDeleted = true;
                context.SubmitChanges();
                TempData["deletestatus"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Membership Plan:" + objmembershipplan.TempPlanName + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("GridMembershipPlans", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return RedirectToAction("GridMembershipPlans", "Dashboard");
            }
        }

        //--------------Block ends here----------------------------------------


        //--------------------Block for New Members Edit Start Here-------------------------------
        /// <summary>
        /// Below action is used to get the details of the member which we want to edit.
        /// </summary>
        /// <param name="MemberId">Id of the member to be edited</param>
        /// <param name="rtflag">this flag is used to differentiate the view to be showed after it is updated.</param>
        /// <returns></returns>
        public ActionResult NewEditMember(int MemberId = 0, int rtflag = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditMember)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            try
            {
                TempMember objgetmember = new TempMember();

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

                List<SunCity.Models.TempViewRegModel.MrgOption> listmrg = new List<SunCity.Models.TempViewRegModel.MrgOption>();
                SunCity.Models.TempViewRegModel.MrgOption objmrg = new SunCity.Models.TempViewRegModel.MrgOption();
                objmrg = new SunCity.Models.TempViewRegModel.MrgOption();
                objmrg.mrgname = "Married";
                objmrg.mrgvalue = "Married";
                listmrg.Add(objmrg);
                objmrg = new SunCity.Models.TempViewRegModel.MrgOption();
                objmrg.mrgname = "Unmarried";
                objmrg.mrgvalue = "Unmarried";
                listmrg.Add(objmrg);



                objgetmember = (from o in context.TempMembers where o.TempMemberId == MemberId select o).FirstOrDefault();
                TempViewRegModel.DeserializeMemberObj objd = new TempViewRegModel.DeserializeMemberObj();

                if (SunCity.Core.Session.Current.flgrt == 0)
                {
                    SunCity.Core.Session.Current.flgrt = rtflag;
                }
                if (MemberId == 0)
                {
                    objd = new TempViewRegModel.DeserializeMemberObj();
                    objd.listgender = listgender;
                    objd.listmarriage = listmrg;
                    return View(objd);
                }
                else
                {
                    objd = new TempViewRegModel.DeserializeMemberObj();
                    objd.listgender = listgender;
                    objd.listmarriage = listmrg;
                    var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(objgetmember.UserDataobj);
                    //        lst = jsono;
                    objd.MemberId = objgetmember.TempMemberId;
                    objd.TempregistrationId = Convert.ToInt32(objgetmember.TempMembershipRegistrationId);
                    objd.UserTypeId = Convert.ToInt32(objgetmember.UserTypeId);
                    objd.debobj = lsta;

                    //Get barcode detail\

                    objd.BarcodeImage = objgetmember.BarcodeImage;
                    objd.PinNo = Convert.ToString(objgetmember.PinNo);

                    //Get profile details
                    var getprofile = (from p in context.TblProfilePictures
                                      join
                                      m in context.TblMapMemberProfilePictures on p.ProfilePictureId equals m.ProfilePictureId
                                      where m.MemberId == Convert.ToInt32(objgetmember.TempMemberId)
                                      orderby m.MemberProfileId descending
                                      select p).FirstOrDefault();
                    if (getprofile != null)
                    {
                        objd.profilePictureId = getprofile.ProfilePictureId;
                        objd.ProfilePictreName = getprofile.ProfiePictureName;
                    }
                    return View(objd);
                }
            }
            catch (Exception ex)
            {
                if (SunCity.Core.Session.Current.flgrt == 0)
                {
                    return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
                }
                else
                {
                    TempMember objgetmember = new TempMember();
                    objgetmember = (from o in context.TempMembers where o.TempMemberId == MemberId select o).FirstOrDefault();
                    return RedirectToAction("NewViewMembership", "Dashboard", new { MembershipRegestrationId = objgetmember.TempMembershipRegistrationId });
                }
            }

        }
        /// <summary>
        /// Below action is used to update the members. We need the getobjde object which will contain all the data to be updated for the particular member. MemberID will be the id of the member to be updated.
        /// </summary>
        /// <param name="getobjde">object containing necessary data to be updated for a particular member.</param>
        /// <param name="MemberId">Id of  the member to be updated</param>
        /// <param name="rtflag">If it used to differentiate the display view after the data has been updated. If rtflag=0 then we will be directed to the List of members view and if rtflag !=0 then we will be directed to the Membership detail page from where we clicked the edit button to edit a particular member.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewEditMember(TempViewRegModel.DeserializeMemberObj getobjde, int MemberId = 0, int rtflag = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.EditMember)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {

                //Block to cancel the changes
                if (Request["hiddencancelflag"] != "0")
                {
                    TempMember tempobjmembers = new TempMember();
                    if (MemberId != 0)
                    {
                        tempobjmembers = (from k in context.TempMembers where k.TempMemberId == MemberId select k).FirstOrDefault();
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("NewViewMembership", "Dashboard", new { MembershipRegestrationId = tempobjmembers.TempMembershipRegistrationId });
                    }

                }


                if (Request["hiddencorporateflag"] == "0")
                {
                    TempMember objmembers = new TempMember();
                    if (MemberId != 0)
                    {
                        objmembers = (from k in context.TempMembers where k.TempMemberId == MemberId select k).FirstOrDefault();
                    }
                    var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(objmembers.UserDataobj);
                    for (int l = 0; l < lsta.Count; l++)
                    {
                        lsta[l].value = getobjde.debobj[l].value;
                    }
                    var jsono = new JavaScriptSerializer().Serialize(lsta);
                    objmembers.UserDataobj = jsono;

                    objmembers.PinNo =Convert.ToString(getobjde.PinNo);

                    context.SubmitChanges();
                    TempData["alertstatus"] = "Success";
                    //Log Activity
                    using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        string msg = " Member:" + getobjde.debobj[1].value + " " + getobjde.debobj[2].value + " has been edited by " + SunCity.Core.Session.Current.EmployeeName;
                        SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                    }
                    if (SunCity.Core.Session.Current.flgrt == 0)
                    {
                        return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
                    }
                    else
                    {
                        return RedirectToAction("NewViewMembership", "Dashboard", new { MembershipRegestrationId = objmembers.TempMembershipRegistrationId });
                    }
                }

                return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
            }
            catch (Exception ex)
            {

                TempData["alertstatus"] = "Error";
                return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
            }

        }

        /// <summary>
        /// Below action is used to delete a particular member. We need to pass the MemberId to delete it.
        /// </summary>
        /// <param name="MemberId">Id of the member to be deleted.</param>
        /// <returns></returns>
        public ActionResult DeleteMember(int? MemberId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteMember)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                TempMember objmember = new TempMember();


                var getallmembersofmembership = (from g in context.TempMembers where g.TempMemberId == MemberId select g).FirstOrDefault();

                if (getallmembersofmembership != null)
                {
                    getallmembersofmembership.IsDeleted = true;
                    getallmembersofmembership.IsBlocked = true;
                    context.SubmitChanges();
                }
                TempData["deletestatusmember"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Member Id:" + getallmembersofmembership.TempMemberId + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                if (SunCity.Core.Session.Current.flgrt == 0)
                {
                    return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
                }
                else
                {
                    return RedirectToAction("NewViewMembership", "Dashboard", new { MembershipRegestrationId = getallmembersofmembership.TempMembershipRegistrationId });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
            }
        }

        /// <summary>
        /// Below action is used to list down all the members in the system. It will list only those members whose delete and block flag is false.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewMemberList()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMember)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                List<ListMember> lstmember = new List<ListMember>();
                ListMember objmember = new ListMember();
                var getmembers = (from r in context.TempMembers where r.IsDeleted == false && r.IsBlocked == false orderby r.TempMemberId descending select r).ToList();
                foreach (var item in getmembers)
                {
                    objmember = new ListMember();
                    var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(item.UserDataobj);
                    objmember.IsBlocked = Convert.ToBoolean(item.IsBlocked);
                    objmember.IsDeleted = Convert.ToBoolean(item.IsDeleted);
                    objmember.TempMemberId = Convert.ToInt32(item.TempMemberId);
                    objmember.TempMembershipRegistrationId = Convert.ToInt32(item.TempMembershipRegistrationId);
                    objmember.UserTypeId = Convert.ToInt32(item.UserTypeId);
                    if (item.UserTypeId == 1)
                    {
                        objmember.sname = lsta[0].value;
                        objmember.fname = lsta[1].value;
                        objmember.mname = lsta[2].value;
                        objmember.phone = lsta[7].value;
                        objmember.dob = lsta[11].value;
                    }
                    else if (item.UserTypeId == 2)
                    {
                        objmember.sname = lsta[0].value;
                        objmember.fname = lsta[1].value;
                        objmember.mname = lsta[2].value;
                        objmember.dob = lsta[4].value;
                        objmember.phone = lsta[13].value;
                    }
                    else if (item.UserTypeId == 3)
                    {
                        objmember.fname = lsta[0].value;
                        objmember.sname = "";
                        objmember.mname = "";
                        objmember.dob = lsta[3].value;
                        objmember.phone = "-";
                    }
                    else if (item.UserTypeId == 4)
                    {
                        objmember.fname = lsta[0].value;
                        objmember.sname = "";
                        objmember.mname = "";
                        objmember.dob = lsta[3].value;
                        objmember.phone = lsta[2].value;
                    }
                    lstmember.Add(objmember);
                }

                if (lstmember != null)
                {
                    return View(lstmember);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        /// <summary>
        /// Below action will list down all the members of the system whose block flag is set to true.
        /// </summary>
        /// <returns></returns>
        public ActionResult NewBlockedMembers()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.BlockedMembers)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                List<ListMember> lstmember = new List<ListMember>();
                ListMember objmember = new ListMember();
                var getmembers = (from r in context.TempMembers where r.IsBlocked == true orderby r.TempMemberId descending select r).ToList();
                foreach (var item in getmembers)
                {
                    objmember = new ListMember();
                    var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(item.UserDataobj);
                    objmember.IsBlocked = Convert.ToBoolean(item.IsBlocked);
                    objmember.IsDeleted = Convert.ToBoolean(item.IsDeleted);
                    objmember.TempMemberId = Convert.ToInt32(item.TempMemberId);
                    objmember.TempMembershipRegistrationId = Convert.ToInt32(item.TempMembershipRegistrationId);
                    objmember.UserTypeId = Convert.ToInt32(item.UserTypeId);
                    if (item.UserTypeId == 1)
                    {
                        objmember.sname = lsta[0].value;
                        objmember.fname = lsta[1].value;
                        objmember.mname = lsta[2].value;
                        objmember.phone = lsta[7].value;
                        objmember.dob = lsta[11].value;
                    }
                    else if (item.UserTypeId == 2)
                    {
                        objmember.sname = lsta[0].value;
                        objmember.fname = lsta[1].value;
                        objmember.mname = lsta[2].value;
                        objmember.dob = lsta[4].value;
                        objmember.phone = lsta[13].value;
                    }
                    else if (item.UserTypeId == 3)
                    {
                        objmember.fname = lsta[0].value;
                        objmember.sname = "";
                        objmember.mname = "";
                        objmember.dob = lsta[3].value;
                        objmember.phone = "-";
                    }
                    else if (item.UserTypeId == 4)
                    {
                        objmember.fname = lsta[0].value;
                        objmember.sname = "";
                        objmember.mname = "";
                        objmember.dob = lsta[3].value;
                        objmember.phone = lsta[2].value;
                    }
                    lstmember.Add(objmember);
                }

                if (lstmember != null)
                {
                    return View(lstmember);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        /// <summary>
        /// Below action is used to block an existing member in the system. We need to just pass the Memberid of the member
        /// </summary>
        /// <param name="MemberId">Id of the member to be blocked</param>
        /// <returns></returns>
        public ActionResult BlockExistingMember(int? MemberId)
        {
            try
            {
                TempMember objmembers = new TempMember();

                objmembers = (from k in context.TempMembers where k.TempMemberId == MemberId select k).FirstOrDefault();

                if (objmembers != null)
                {
                    objmembers.IsBlocked = true;
                    context.SubmitChanges();
                }
                TempData["blockstatusmember"] = "Success";
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Member Id:" + objmembers.TempMemberId + " has been blocked by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }

                //if (SunCity.Core.Session.Current.flgrt == 0)
                //{
                return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
                //}
                //else
                //{
                //    return RedirectToAction("NewViewMembership", "Dashboard", new { MembershipRegestrationId = objmembers.TempMembershipRegistrationId });
                //}
            }
            catch (Exception ex)
            {
                return RedirectToAction("ViewMemberList", "Dashboard", new { MemberId = 0 });
            }
        }

        /// <summary>
        /// Below action is used to unblock an existing member. We need to pass the memberid of the member.
        /// </summary>
        /// <param name="MemberId"></param>
        /// <returns></returns>
        public ActionResult UnblockExistingMember(int? MemberId)
        {
            try
            {
                TempMember objmembers = new TempMember();
                if (MemberId != 0)
                {
                    objmembers = (from k in context.TempMembers where k.TempMemberId == MemberId select k).FirstOrDefault();
                }
                objmembers.IsBlocked = false;
                context.SubmitChanges();
                //Log Activity
                using (StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/ActivityLog/") + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + "/" + DateTime.UtcNow.AddHours(5.50).Date.ToString("dd-MM-yyyy") + ".txt"))
                {
                    string msg = " Member Id:" + objmembers.TempMemberId + " has been unblocked by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("NewBlockedMembers", "Dashboard");

            }
            catch (Exception ex)
            {
                return RedirectToAction("NewBlockedMembers", "Dashboard");
            }
        }
        //--------------------Block for New Members Edit Ends here--------------------------------


        //-------------------Block for New Membership Enrolment Starts------------------------------------
        public ActionResult Getusertype()
        {
            var getusertype = (from u in context.TblUserTypes select u).ToList();
            return Json(getusertype, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getuserfields(string usertypeid)
        {
            List<MapFieldUser> listmapfielduser = new List<MapFieldUser>();
            MapFieldUser objmapfielduser = new MapFieldUser();
            var getmapplanusertype = (from gm in context.TblMapFieldUserTypes
                                      join
                                          cm in context.TblFields on gm.FieldId equals cm.FieldId
                                      where gm.UserTypeId == Convert.ToInt32(usertypeid)
                                      select new { gm, cm }).ToList();

            foreach (var itemf in getmapplanusertype)
            {
                objmapfielduser = new MapFieldUser();
                objmapfielduser.fieldid = itemf.cm.FieldId;
                objmapfielduser.FieldName = itemf.cm.FieldName;
                objmapfielduser.FieldText = itemf.cm.FieldText;
                objmapfielduser.FieldType = itemf.cm.FieldType;
                objmapfielduser.UserTypeId = Convert.ToInt32(usertypeid);
                listmapfielduser.Add(objmapfielduser);
            }
            return Json(listmapfielduser, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Below action is used to get all the details of a membership. We need to pass the registrationid which will fetch all the data as per the id
        /// </summary>
        /// <param name="MembershipRegestrationId">Id of the membership Enrolled</param>
        /// <returns></returns>
        public ActionResult NewViewMembership(int? MembershipRegestrationId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMembership)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {

                TempViewRegModel objmodel = new TempViewRegModel();
                objmodel.objtempreg = (from c in context.TempMembershipRegs where c.TempMembershipRegistrationId == MembershipRegestrationId select c).FirstOrDefault();
                objmodel.listtempmember = (from m in context.TempMembers where m.TempMembershipRegistrationId == MembershipRegestrationId select m).ToList();
                objmodel.templistMembershipPlan = objmodel.tempgetListMembershipPlan();
                objmodel.listPackages = objmodel.getListPackage();
                objmodel.listdocuments = (from d in context.TblDocuments where d.MembershipRegistrationId == MembershipRegestrationId select d).ToList();
                List<TempViewRegModel.DeserializeMemberObj> listdes = new List<TempViewRegModel.DeserializeMemberObj>();
                TempViewRegModel.DeserializeMemberObj objd = new TempViewRegModel.DeserializeMemberObj();



                foreach (var item in objmodel.listtempmember)
                {
                    objd = new TempViewRegModel.DeserializeMemberObj();
                    List<TempViewRegModel.KeyValue> lst = new List<TempViewRegModel.KeyValue>();
                    var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(item.UserDataobj);
                    //        lst = jsono;
                    objd.MemberId = item.TempMemberId;
                    objd.TempregistrationId = Convert.ToInt32(item.TempMembershipRegistrationId);
                    objd.UserTypeId = Convert.ToInt32(item.UserTypeId);
                    objd.Isdeleted = Convert.ToBoolean(item.IsDeleted);
                    objd.debobj = lsta;
                    listdes.Add(objd);

                }
                objmodel.lstdeserialize = listdes;

                return View(objmodel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        /// <summary>
        /// Below action is used to updated the documents which if we upload during the viewing of the membership.
        /// </summary>
        /// <param name="TempViewRegModel">Will contain necessary info which needs to be updated.</param>
        /// <param name="MembershipRegestrationId">Id of the membership</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewViewMembership(TempViewRegModel TempViewRegModel, int? MembershipRegestrationId)
        {
            //Edit the addtional fields
            TempMembershipReg objreg = new TempMembershipReg();
            objreg = (from o in context.TempMembershipRegs where o.TempMembershipRegistrationId == TempViewRegModel.objtempreg.TempMembershipRegistrationId select o).FirstOrDefault();
            if (TempViewRegModel.objtempreg.TempPackageId != null)
            {
                objreg.TempPackageId = TempViewRegModel.objtempreg.TempPackageId;
            }
            else
            {
                objreg.TempPackageId = 0;
            }

            objreg.ReferenceName = TempViewRegModel.objtempreg.ReferenceName;
            objreg.RelationtoMember = TempViewRegModel.objtempreg.RelationtoMember;
            objreg.OtherMembershipType = TempViewRegModel.objtempreg.OtherMembershipType;
            objreg.NameofClub = TempViewRegModel.objtempreg.NameofClub;
            objreg.HowLong = TempViewRegModel.objtempreg.HowLong;
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
                    objdocs.MembershipRegistrationId = TempViewRegModel.objtempreg.TempMembershipRegistrationId;
                    context.TblDocuments.InsertOnSubmit(objdocs);
                    context.SubmitChanges();
                }

            }

            TempData["alertstatus"] = "Success";
            return RedirectToAction("NewViewMembership", "Dashboard", new { MembershipRegestrationId = TempViewRegModel.objtempreg.TempMembershipRegistrationId });
        }

        /// <summary>
        /// Below class is used to hold the memberid and the List of keyvalue pair
        /// </summary>
        public class GetPostValue
        {
            public int memberid { get; set; }
            public List<KeyValue> listkey { get; set; }


        }

        /// <summary>
        /// Below class is used for getting key-value pair.
        /// </summary>
        public class KeyValue
        {

            public string key { get; set; }
            public string value { get; set; }
        }

        /// <summary>
        /// Below class is used to get memberid, usertypeid, registrationid and list of keyvalue object.
        /// </summary>
        public class DeserializeMemberObj
        {
            public int MemberId { get; set; }
            public int UserTypeId { get; set; }
            public int TempregistrationId { get; set; }
            public List<KeyValue> debobj { get; set; }
        }

        /// <summary>
        /// Below action is used to get all the necessary info related to a particular plan. It will get all the details related to Membership Plan, Plus the usertype attached to the plan and its required details.
        /// </summary>
        /// <param name="planid">Id of the Membership Plan of the system</param>
        /// <returns></returns>
        public ActionResult GetPlanHistory(string planid)
        {
            //string orderid = Request["orderID"];
            ModelTempMembershipPlan objm = new ModelTempMembershipPlan();
            //var getm = (from m in context.TempMembershipPlans where m.TempPlanId == 1 select m).FirstOrDefault();
            var getm = (from m in context.TempMembershipPlans where m.TempPlanId == Convert.ToInt32(planid) select m).FirstOrDefault();
            if (getm != null)
            {
                objm.CreatedBy = Convert.ToInt32(getm.CreatedBy);
                objm.CreatedDate = Convert.ToDateTime(getm.CreatedDate);
                objm.IsDeleted = Convert.ToBoolean(getm.IsDeleted);
                objm.TempAmount = Convert.ToDecimal(getm.TempAmount);
                objm.TempPeriod = Convert.ToInt32(getm.TempPeriod);
                objm.TempPlanId = getm.TempPlanId;
                objm.TempPlanName = getm.TempPlanName;

            }

            //get the usertype list from mapplanusertype table
            var getusertypelist = (from l in context.TempMapPlanUserTypes where l.TempPlanId == getm.TempPlanId select l).ToList();
            List<MapPlanUserType> lstmapplanusertype = new List<MapPlanUserType>();
            MapPlanUserType objmapusertype = new MapPlanUserType();
            List<MapFieldUser> listmapfielduser = new List<MapFieldUser>();
            MapFieldUser objmapfielduser = new MapFieldUser();
            foreach (var item in getusertypelist)
            {
                //Count the usertype
                objmapusertype = new MapPlanUserType();
                objmapusertype.MapPlanUserTypeId = item.MapPlanUserTypeId;
                objmapusertype.TempPlanId = Convert.ToInt32(item.TempPlanId);
                objmapusertype.Usercount = Convert.ToInt32(item.Usercount);
                objmapusertype.UserTypeId = Convert.ToInt32(item.UserTypeId);
                lstmapplanusertype.Add(objmapusertype);

                //Bind the fields

                var getmapplanusertype = (from gm in context.TblMapFieldUserTypes
                                          join
                                              cm in context.TblFields on gm.FieldId equals cm.FieldId
                                          where gm.UserTypeId == item.UserTypeId
                                          select new { gm, cm }).ToList();

                foreach (var itemf in getmapplanusertype)
                {
                    objmapfielduser = new MapFieldUser();
                    objmapfielduser.fieldid = itemf.cm.FieldId;
                    objmapfielduser.FieldName = itemf.cm.FieldName;
                    objmapfielduser.FieldText = itemf.cm.FieldText;
                    objmapfielduser.FieldType = itemf.cm.FieldType;
                    objmapfielduser.UserTypeId = Convert.ToInt32(item.UserTypeId);
                    listmapfielduser.Add(objmapfielduser);
                }
            }
            objm.Mapplanusertypelist = lstmapplanusertype;
            objm.Mapfielduser = listmapfielduser;



            return Json(objm, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Below action is used to get details to be viewed when we open an Membership Form. It will get list of Membership Plan and List of packages and will send to the view.
        /// </summary>
        /// <returns></returns>
        public ActionResult NewMembership()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.NewEnrollment)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            TempMembershipReg objgetregdetails = new TempMembershipReg();

            objgetregdetails.templistMembershipPlan = objgetregdetails.tempgetListMembershipPlan();
            objgetregdetails.listPackages = objgetregdetails.getListPackage();
            return View(objgetregdetails);


        }

        /// <summary>
        /// Below action will contain and object FrmMembershipRegistration of Table TempMembershipReg which will store the membership details. It will also add the documents attached with the membership in a table TblDocument.
        /// It will then add values to the Table TempMembers, TblProfilePicture &  TblMapMemberProfilePicture. this all table contains value associated to the members enrolled in the membership
        /// </summary>
        /// <param name="FrmMembershipRegistration"></param>
        /// <param name="MembershipRegestrationId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewMembership(TempMembershipReg FrmMembershipRegistration, int MembershipRegestrationId = 0)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.NewEnrollment)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            try
            {

                ModelTempMembershipPlan objm = new ModelTempMembershipPlan();
                var getm = (from m in context.TempMembershipPlans where m.TempPlanId == FrmMembershipRegistration.TempPlanId select m).FirstOrDefault();
                if (getm != null)
                {
                    objm.CreatedBy = Convert.ToInt32(getm.CreatedBy);
                    objm.CreatedDate = Convert.ToDateTime(getm.CreatedDate);
                    objm.IsDeleted = Convert.ToBoolean(getm.IsDeleted);
                    objm.TempAmount = Convert.ToDecimal(getm.TempAmount);
                    objm.TempPeriod = Convert.ToInt32(getm.TempPeriod);
                    objm.TempPlanId = getm.TempPlanId;
                    objm.TempPlanName = getm.TempPlanName;

                }

                //get the usertype list from mapplanusertype table
                var getusertypelist = (from l in context.TempMapPlanUserTypes where l.TempPlanId == getm.TempPlanId select l).ToList();
                List<MapPlanUserType> lstmapplanusertype = new List<MapPlanUserType>();
                MapPlanUserType objmapusertype = new MapPlanUserType();
                List<MapFieldUser> listmapfielduser = new List<MapFieldUser>();
                MapFieldUser objmapfielduser = new MapFieldUser();
                foreach (var item in getusertypelist)
                {
                    //Count the usertype
                    objmapusertype = new MapPlanUserType();
                    objmapusertype.MapPlanUserTypeId = item.MapPlanUserTypeId;
                    objmapusertype.TempPlanId = Convert.ToInt32(item.TempPlanId);
                    objmapusertype.Usercount = Convert.ToInt32(item.Usercount);
                    objmapusertype.UserTypeId = Convert.ToInt32(item.UserTypeId);
                    lstmapplanusertype.Add(objmapusertype);

                    //Bind the fields

                    var getmapplanusertype = (from gm in context.TblMapFieldUserTypes
                                              join
                                                  cm in context.TblFields on gm.FieldId equals cm.FieldId
                                              where gm.UserTypeId == item.UserTypeId
                                              select new { gm, cm }).ToList();

                    foreach (var itemf in getmapplanusertype)
                    {
                        objmapfielduser = new MapFieldUser();
                        objmapfielduser.fieldid = itemf.cm.FieldId;
                        objmapfielduser.FieldName = itemf.cm.FieldName;
                        objmapfielduser.FieldText = itemf.cm.FieldText;
                        objmapfielduser.FieldType = itemf.cm.FieldType;
                        objmapfielduser.UserTypeId = Convert.ToInt32(item.UserTypeId);
                        listmapfielduser.Add(objmapfielduser);
                    }
                }
                objm.Mapplanusertypelist = lstmapplanusertype;
                objm.Mapfielduser = listmapfielduser;
                var getusertypecout = lstmapplanusertype.Count;

                //Get posted values

                TempMembershipReg objreg = new TempMembershipReg();

                objreg.CreatedBy = SunCity.Core.Session.Current.UserId;
                objreg.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                objreg.IsDeleted = false;
                DateTime stdt = Convert.ToDateTime(FrmMembershipRegistration.TempStartDate);
                objreg.TempStartDate = FrmMembershipRegistration.TempStartDate;
                var getmembershiplan = (from m in context.TempMembershipPlans where m.TempPlanId == FrmMembershipRegistration.TempPlanId select m).FirstOrDefault();
                objreg.TempEndDate = stdt.AddYears(Convert.ToInt32(getmembershiplan.TempPeriod));
                objreg.TempPlanId = FrmMembershipRegistration.TempPlanId;
                if (FrmMembershipRegistration.TempPackageId != null)
                {
                    objreg.TempPackageId = FrmMembershipRegistration.TempPackageId;
                }
                else
                {
                    objreg.TempPackageId = 0;
                }
                objreg.Amount = getmembershiplan.TempAmount;
                //Additional fields
                objreg.HowLong = FrmMembershipRegistration.HowLong;
                objreg.NameofClub = FrmMembershipRegistration.NameofClub;
                objreg.OtherMembershipType = FrmMembershipRegistration.OtherMembershipType;
                objreg.ReferenceName = FrmMembershipRegistration.ReferenceName;
                objreg.RelationtoMember = FrmMembershipRegistration.RelationtoMember;

                context.TempMembershipRegs.InsertOnSubmit(objreg);
                context.SubmitChanges();

                //Adding the membership code in the registration table
                //objreg.TempMembershipNo = getmembershiplan.TempMembershipCode + "-" + objreg.TempMembershipRegistrationId;
                objreg.TempMembershipNo = objreg.TempMembershipNo;
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
                        objdocs.MembershipRegistrationId = objreg.TempMembershipRegistrationId;
                        context.TblDocuments.InsertOnSubmit(objdocs);
                        context.SubmitChanges();
                    }

                }

                List<GetPostValue> listgetpost = new List<GetPostValue>();
                GetPostValue objgetpo = new GetPostValue();
                List<KeyValue> listkey = new List<KeyValue>();
                for (int k = 0; k < getusertypecout; k++)
                {
                    if (objm.Mapplanusertypelist[k].UserTypeId == 1)
                    {
                        if (objm.Mapplanusertypelist[k].Usercount > 0)
                        {
                            //Check for no. of user for particular usertype
                            for (int c = 0; c < objm.Mapplanusertypelist[k].Usercount; c++)
                            {
                                TempMember objtempmem = new TempMember();
                                objtempmem.TempMembershipRegistrationId = objreg.TempMembershipRegistrationId;
                                objtempmem.UserTypeId = objm.Mapplanusertypelist[k].UserTypeId;
                                objtempmem.IsDeleted = false;
                                objtempmem.IsBlocked = false;
                                objtempmem.PinNo = "0000";
                                context.TempMembers.InsertOnSubmit(objtempmem);
                                context.SubmitChanges();

                                //Add code to generate barcode and save it in the db
                                IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
                                var result = writer.Write("Member#" + objtempmem.TempMemberId);
                                var barcodeBitmap = new Bitmap(result);
                                barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/MemberId-" + objtempmem.TempMemberId+ ".png"));
                                objtempmem.BarcodeImage = "MemberId-" + objtempmem.TempMemberId + ".png";
                                context.SubmitChanges();


                                //Enter the picture in picture table

                                TblProfilePicture objpic = new TblProfilePicture();
                                objpic.ProfiePictureName = Convert.ToString(Request["hiddenmem" + c + "photo"]);
                                objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                                objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                                context.TblProfilePictures.InsertOnSubmit(objpic);
                                context.SubmitChanges();

                                //Enter the member-picture entry in diff table
                                TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                                objmappic.MemberId = objtempmem.TempMemberId;
                                objmappic.ProfilePictureId = objpic.ProfilePictureId;
                                context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                                context.SubmitChanges();

                                listkey = new List<KeyValue>();

                                objgetpo = new GetPostValue();

                                objgetpo.memberid = objtempmem.TempMemberId;
                                //objgetpo.memberid = k;


                                KeyValue keyv = new KeyValue();



                                for (int j = 0; j < objm.Mapfielduser.Count; j++)
                                {
                                    keyv = new KeyValue();
                                    if (objm.Mapfielduser[j].UserTypeId == objm.Mapplanusertypelist[k].UserTypeId)
                                    {
                                        if (objm.Mapfielduser[j].FieldType == "select")
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            if (objm.Mapfielduser[j].FieldText == "Sex")
                                            {
                                                getvar = Convert.ToString(Request["mem_" + c + "_gen"]);
                                            }
                                            else if (objm.Mapfielduser[j].FieldText == "Maritial Status")
                                            {
                                                getvar = Convert.ToString(Request["mem_" + c + "_mar"]);
                                            }
                                            keyv.value = getvar;
                                        }
                                        else
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Convert.ToString(Request["mem_" + c + "_" + objm.Mapfielduser[j].FieldName]);
                                            keyv.value = getvar;
                                        }
                                        listkey.Add(keyv);
                                    }
                                }

                                objgetpo.listkey = listkey;
                                listgetpost.Add(objgetpo);
                                var jsono = new JavaScriptSerializer().Serialize(listkey);
                                objtempmem.UserDataobj = jsono;
                                context.SubmitChanges();
                            }
                        }

                    }
                    if (objm.Mapplanusertypelist[k].UserTypeId == 2)
                    {
                        if (objm.Mapplanusertypelist[k].Usercount > 0)
                        {
                            //Check for no. of user for particular usertype
                            for (int c = 0; c < objm.Mapplanusertypelist[k].Usercount; c++)
                            {
                                TempMember objtempmem = new TempMember();
                                objtempmem.TempMembershipRegistrationId = objreg.TempMembershipRegistrationId;
                                objtempmem.UserTypeId = objm.Mapplanusertypelist[k].UserTypeId;
                                objtempmem.IsDeleted = false;
                                objtempmem.IsBlocked = false;
                                objtempmem.PinNo = "0000";
                                context.TempMembers.InsertOnSubmit(objtempmem);
                                context.SubmitChanges();

                                //Add code to generate barcode and save it in the db
                                IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
                                var result = writer.Write("Member#" + objtempmem.TempMemberId);
                                var barcodeBitmap = new Bitmap(result);
                                barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/MemberId-" + objtempmem.TempMemberId + ".png"));
                                objtempmem.BarcodeImage = "MemberId-" + objtempmem.TempMemberId + ".png";
                                context.SubmitChanges();

                                //Enter the picture in picture table

                                TblProfilePicture objpic = new TblProfilePicture();
                                objpic.ProfiePictureName = Convert.ToString(Request["shiddenmem" + c + "photo"]);
                                objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                                objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                                context.TblProfilePictures.InsertOnSubmit(objpic);
                                context.SubmitChanges();

                                //Enter the member-picture entry in diff table
                                TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                                objmappic.MemberId = objtempmem.TempMemberId;
                                objmappic.ProfilePictureId = objpic.ProfilePictureId;
                                context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                                context.SubmitChanges();


                                listkey = new List<KeyValue>();

                                objgetpo = new GetPostValue();

                                objgetpo.memberid = objtempmem.TempMemberId;
                                //objgetpo.memberid = k;


                                KeyValue keyv = new KeyValue();



                                for (int j = 0; j < objm.Mapfielduser.Count; j++)
                                {
                                    keyv = new KeyValue();
                                    if (objm.Mapfielduser[j].UserTypeId == objm.Mapplanusertypelist[k].UserTypeId)
                                    {

                                        var getvar = "";
                                        keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                        //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                        getvar = Convert.ToString(Request["smem_" + c + "_" + objm.Mapfielduser[j].FieldName]);
                                        keyv.value = getvar;
                                        listkey.Add(keyv);
                                    }
                                }

                                objgetpo.listkey = listkey;
                                listgetpost.Add(objgetpo);
                                var jsono = new JavaScriptSerializer().Serialize(listkey);
                                objtempmem.UserDataobj = jsono;
                                context.SubmitChanges();
                            }
                        }

                    }
                    if (objm.Mapplanusertypelist[k].UserTypeId == 3)
                    {
                        if (objm.Mapplanusertypelist[k].Usercount > 0)
                        {
                            //Check for no. of user for particular usertype
                            for (int c = 0; c < objm.Mapplanusertypelist[k].Usercount; c++)
                            {
                                TempMember objtempmem = new TempMember();
                                objtempmem.TempMembershipRegistrationId = objreg.TempMembershipRegistrationId;
                                objtempmem.UserTypeId = objm.Mapplanusertypelist[k].UserTypeId;
                                objtempmem.IsDeleted = false;
                                objtempmem.IsBlocked = false;
                                objtempmem.PinNo = "0000";
                                context.TempMembers.InsertOnSubmit(objtempmem);
                                context.SubmitChanges();

                                //Add code to generate barcode and save it in the db
                                IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
                                var result = writer.Write("Member#" + objtempmem.TempMemberId);
                                var barcodeBitmap = new Bitmap(result);
                                barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/MemberId-" + objtempmem.TempMemberId + ".png"));
                                objtempmem.BarcodeImage = "MemberId-" + objtempmem.TempMemberId + ".png";
                                context.SubmitChanges();

                                //Enter the picture in picture table

                                TblProfilePicture objpic = new TblProfilePicture();
                                objpic.ProfiePictureName = Convert.ToString(Request["chiddenmem" + c + "photo"]);
                                objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                                objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                                context.TblProfilePictures.InsertOnSubmit(objpic);
                                context.SubmitChanges();

                                //Enter the member-picture entry in diff table
                                TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                                objmappic.MemberId = objtempmem.TempMemberId;
                                objmappic.ProfilePictureId = objpic.ProfilePictureId;
                                context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                                context.SubmitChanges();

                                listkey = new List<KeyValue>();

                                objgetpo = new GetPostValue();

                                objgetpo.memberid = objtempmem.TempMemberId;
                                //objgetpo.memberid = k;


                                KeyValue keyv = new KeyValue();



                                for (int j = 0; j < objm.Mapfielduser.Count; j++)
                                {
                                    keyv = new KeyValue();
                                    if (objm.Mapfielduser[j].UserTypeId == objm.Mapplanusertypelist[k].UserTypeId)
                                    {
                                        if (objm.Mapfielduser[j].FieldType == "select")
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Convert.ToString(Request["cmem_" + c + "_gen"]);
                                            keyv.value = getvar;
                                        }
                                        else
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Convert.ToString(Request["cmem_" + c + "_" + objm.Mapfielduser[j].FieldName]);
                                            keyv.value = getvar;

                                        }
                                        listkey.Add(keyv);
                                    }
                                }

                                objgetpo.listkey = listkey;
                                listgetpost.Add(objgetpo);
                                var jsono = new JavaScriptSerializer().Serialize(listkey);
                                objtempmem.UserDataobj = jsono;
                                context.SubmitChanges();
                            }
                        }

                    }
                    if (objm.Mapplanusertypelist[k].UserTypeId == 4)
                    {
                        if (objm.Mapplanusertypelist[k].Usercount > 0)
                        {
                            //Check for no. of user for particular usertype
                            for (int c = 0; c < objm.Mapplanusertypelist[k].Usercount; c++)
                            {
                                TempMember objtempmem = new TempMember();
                                objtempmem.TempMembershipRegistrationId = objreg.TempMembershipRegistrationId;
                                objtempmem.UserTypeId = objm.Mapplanusertypelist[k].UserTypeId;
                                objtempmem.IsDeleted = false;
                                objtempmem.IsBlocked = false;
                                objtempmem.PinNo = "0000";
                                context.TempMembers.InsertOnSubmit(objtempmem);
                                context.SubmitChanges();

                                //Add code to generate barcode and save it in the db
                                IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128 };
                                var result = writer.Write("Member#" + objtempmem.TempMemberId);
                                var barcodeBitmap = new Bitmap(result);
                                barcodeBitmap.Save(Server.MapPath("~/BarCodeMember/MemberId-" + objtempmem.TempMemberId + ".png"));
                                objtempmem.BarcodeImage = "MemberId-" + objtempmem.TempMemberId + ".png";
                                context.SubmitChanges();

                                //Enter the picture in picture table

                                TblProfilePicture objpic = new TblProfilePicture();
                                objpic.ProfiePictureName = Convert.ToString(Request["seniormem" + c + "photo"]);
                                objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
                                objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
                                context.TblProfilePictures.InsertOnSubmit(objpic);
                                context.SubmitChanges();

                                //Enter the member-picture entry in diff table
                                TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
                                objmappic.MemberId = objtempmem.TempMemberId;
                                objmappic.ProfilePictureId = objpic.ProfilePictureId;
                                context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
                                context.SubmitChanges();

                                listkey = new List<KeyValue>();

                                objgetpo = new GetPostValue();

                                objgetpo.memberid = objtempmem.TempMemberId;
                                //objgetpo.memberid = k;


                                KeyValue keyv = new KeyValue();



                                for (int j = 0; j < objm.Mapfielduser.Count; j++)
                                {
                                    keyv = new KeyValue();
                                    if (objm.Mapfielduser[j].UserTypeId == objm.Mapplanusertypelist[k].UserTypeId)
                                    {
                                        if (objm.Mapfielduser[j].FieldType == "select")
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Convert.ToString(Request["semem_" + c + "_gen"]);
                                            keyv.value = getvar;
                                        }
                                        else
                                        {
                                            var getvar = "";
                                            keyv.key = Convert.ToString(objm.Mapfielduser[j].fieldid);
                                            //getvar = Request["mem_" + k + "_" + objm.Mapfielduser[j].FieldName];
                                            getvar = Convert.ToString(Request["semem_" + c + "_" + objm.Mapfielduser[j].FieldName]);
                                            keyv.value = getvar;

                                        }
                                        listkey.Add(keyv);
                                    }
                                }

                                objgetpo.listkey = listkey;
                                listgetpost.Add(objgetpo);
                                var jsono = new JavaScriptSerializer().Serialize(listkey);
                                objtempmem.UserDataobj = jsono;
                                context.SubmitChanges();
                            }

                        }
                    }
                }
                //TempMapMember objmap = new TempMapMember();
                //foreach (var item in listgetpost)
                //{
                //    objmap = new TempMapMember();
                //    objmap.TempMemberId = item.memberid;
                //    var json = new JavaScriptSerializer().Serialize(item.listkey);
                //    objmap.UserDataobj = json;
                //    context.TempMapMembers.InsertOnSubmit(objmap);
                //    context.SubmitChanges();
                //}

                TempData["alertstatus"] = "Success";
                return RedirectToAction("NewMembership", "Dashboard", new { MembershipRegestrationId = 0 });
            }
            catch (Exception ex)
            {
                TempData["alertstatus"] = "Error";
                return RedirectToAction("NewMembership", "Dashboard", new { MembershipRegestrationId = 0 });
            }


        }

        /// <summary>
        /// Below action is used to get list of all the membership enrolled in the system.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewMembershipList()
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.ViewMembershipRegistration)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }
            try
            {
                var getregestrations = (from r in context.TempMembershipRegs orderby r.TempMembershipRegistrationId descending select r).ToList();
                List<MembershipRegistration> listreg = new List<MembershipRegistration>();
                MembershipRegistration objreg = new MembershipRegistration();
                foreach (var item in getregestrations)
                {
                    objreg = new MembershipRegistration();
                    objreg.Amount = Convert.ToDecimal(item.Amount);
                    objreg.IsDeleted = Convert.ToBoolean(item.IsDeleted);
                    objreg.MembershipEndDate = Convert.ToDateTime(item.TempEndDate);
                    objreg.MembershipRegistrationId = item.TempMembershipRegistrationId;
                    objreg.MembershipStartDate = Convert.ToDateTime(item.TempStartDate);
                    objreg.MembershipNo = item.TempMembershipNo;
                    var getname = (from m in context.TempMembers where m.TempMembershipRegistrationId == item.TempMembershipRegistrationId orderby m.TempMemberId select m).FirstOrDefault();
                    if (getname != null)
                    {
                        var lsta = new JavaScriptSerializer().Deserialize<List<TempViewRegModel.KeyValue>>(getname.UserDataobj);
                        //objreg.Name = getname.UserDataobj + " " + getname.MemberMiddleName + " " + getname.MemberLastName;
                        if (getname.UserTypeId == 1 || getname.UserTypeId == 2)
                        {
                            objreg.Name = lsta[0].value + " " + lsta[1].value + " " + lsta[2].value;
                        }
                        else if (getname.UserTypeId == 3 || getname.UserTypeId == 4)
                        {
                            objreg.Name = lsta[0].value;
                        }
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
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        /// <summary>
        /// Below action is used to delete a membership. we need to pass the MembershipRegestrationId so that it can be deleted.
        /// </summary>
        /// <param name="MembershipRegestrationId"></param>
        /// <returns></returns>
        public ActionResult DeleteMembership(int? MembershipRegestrationId)
        {
            if (!SunCity.Core.UtilityFunction.hasPermission(Convert.ToInt32(SunCity.Core.Enum.Permission.DeleteMembership)))
            {
                return RedirectToAction("AccessDenied", "Function");
            }

            try
            {
                TempMembershipReg objmembership = new TempMembershipReg();
                if (MembershipRegestrationId != 0)
                {
                    objmembership = (from k in context.TempMembershipRegs where k.TempMembershipRegistrationId == MembershipRegestrationId select k).FirstOrDefault();
                }
                objmembership.IsDeleted = true;
                context.SubmitChanges();

                var getallmembersofmembership = (from g in context.TempMembers where g.TempMembershipRegistrationId == MembershipRegestrationId select g).ToList();

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
                    string msg = " Membership:" + objmembership.TempMembershipRegistrationId + " has been deleted by " + SunCity.Core.Session.Current.EmployeeName;
                    SunCity.Core.UtilityFunction.LogActivity(msg, sw);
                }
                return RedirectToAction("ViewMembershipList", "Dashboard");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ViewMembershipList", "Dashboard");
            }
        }

        /// <summary>
        /// Below action is used to add an additional main member in the existing membership. we need to pass on the required data and then we will be able to add it to the existing enrolled membership.
        /// It will then add the additional cost to enroll the new member into the existing amount of the membership.
        /// </summary>
        /// <param name="MembershipRegistrationId"></param>
        /// <param name="usertype"></param>
        /// <param name="profilename"></param>
        /// <param name="sname"></param>
        /// <param name="fname"></param>
        /// <param name="mname"></param>
        /// <param name="sex"></param>
        /// <param name="father"></param>
        /// <param name="address"></param>
        /// <param name="phoneres"></param>
        /// <param name="phonem"></param>
        /// <param name="nationality"></param>
        /// <param name="email"></param>
        /// <param name="pan"></param>
        /// <param name="adob"></param>
        /// <param name="maritial"></param>
        /// <param name="education"></param>
        /// <param name="blood"></param>
        /// <param name="marriagedate"></param>
        /// <param name="religion"></param>
        /// <param name="physical"></param>
        /// <param name="interest"></param>
        /// <param name="firm"></param>
        /// <param name="businessaddress"></param>
        /// <param name="addressoffice"></param>
        /// <param name="emailoffice"></param>
        /// <param name="phoneoffice"></param>
        /// <param name="businesscard"></param>
        /// <param name="designation"></param>
        /// <param name="annualincome"></param>
        /// <param name="extraamount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditNewMainMember(string MembershipRegistrationId, string usertype, string profilename, string sname, string fname, string mname, string sex, string father, string address, string phoneres, string phonem, string nationality, string email, string pan, string adob, string maritial, string education, string blood, string marriagedate, string religion, string physical, string interest, string firm, string businessaddress, string addressoffice, string emailoffice, string phoneoffice, string businesscard, string designation, string annualincome, string extraamount)
        {
            TempMember objmember = new TempMember();
            objmember.IsBlocked = false;
            objmember.IsDeleted = false;
            objmember.TempMembershipRegistrationId = Convert.ToInt32(MembershipRegistrationId);
            objmember.UserTypeId = Convert.ToInt32(usertype);
            context.TempMembers.InsertOnSubmit(objmember);
            context.SubmitChanges();

            //Enter the picture in picture table

            TblProfilePicture objpic = new TblProfilePicture();
            objpic.ProfiePictureName = Convert.ToString(profilename);
            objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
            objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
            context.TblProfilePictures.InsertOnSubmit(objpic);
            context.SubmitChanges();

            //Enter the member-picture entry in diff table
            TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
            objmappic.MemberId = objmember.TempMemberId;
            objmappic.ProfilePictureId = objpic.ProfilePictureId;
            context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
            context.SubmitChanges();

            List<KeyValue> listkey = new List<KeyValue>();
            KeyValue keyv = new KeyValue();
            var getmapplanusertype = (from gm in context.TblMapFieldUserTypes
                                      join
                                          cm in context.TblFields on gm.FieldId equals cm.FieldId
                                      where gm.UserTypeId == Convert.ToInt32(usertype)
                                      select new { gm, cm }).ToList();

            for (int y = 0; y < getmapplanusertype.Count; y++)
            {
                keyv = new KeyValue();
                keyv.key = Convert.ToString(getmapplanusertype[y].gm.FieldId);
                //var st = "smem_" + usertype + "_" + item.cm.FieldName;
                if (y == 0)
                {
                    keyv.value = Convert.ToString(sname);
                }
                else if (y == 1)
                {
                    keyv.value = Convert.ToString(fname);
                }
                else if (y == 2)
                {
                    keyv.value = Convert.ToString(mname);
                }
                else if (y == 3)
                {
                    keyv.value = Convert.ToString(sex);
                }
                else if (y == 4)
                {
                    keyv.value = Convert.ToString(father);
                }
                else if (y == 5)
                {
                    keyv.value = Convert.ToString(address);
                }
                else if (y == 6)
                {
                    keyv.value = Convert.ToString(phoneres);
                }
                else if (y == 7)
                {
                    keyv.value = Convert.ToString(phonem);
                }
                else if (y == 8)
                {
                    keyv.value = Convert.ToString(nationality);
                }
                else if (y == 9)
                {
                    keyv.value = Convert.ToString(email);
                }
                else if (y == 10)
                {
                    keyv.value = Convert.ToString(pan);
                }
                else if (y == 11)
                {
                    keyv.value = Convert.ToString(adob);
                }
                else if (y == 12)
                {
                    keyv.value = Convert.ToString(maritial);
                }
                else if (y == 13)
                {
                    keyv.value = Convert.ToString(education);
                }

                else if (y == 14)
                {
                    keyv.value = Convert.ToString(blood);
                }
                else if (y == 15)
                {
                    keyv.value = Convert.ToString(marriagedate);
                }
                else if (y == 16)
                {
                    keyv.value = Convert.ToString(religion);
                }
                else if (y == 17)
                {
                    keyv.value = Convert.ToString(physical);
                }
                else if (y == 18)
                {
                    keyv.value = Convert.ToString(interest);
                }
                else if (y == 19)
                {
                    keyv.value = Convert.ToString(firm);
                }
                else if (y == 20)
                {
                    keyv.value = Convert.ToString(businessaddress);
                }
                else if (y == 21)
                {
                    keyv.value = Convert.ToString(addressoffice);
                }
                else if (y == 22)
                {
                    keyv.value = Convert.ToString(emailoffice);
                }
                else if (y == 23)
                {
                    keyv.value = Convert.ToString(phoneoffice);
                }
                else if (y == 24)
                {
                    keyv.value = Convert.ToString(businesscard);
                }
                else if (y == 25)
                {
                    keyv.value = Convert.ToString(designation);
                }
                else if (y == 26)
                {
                    keyv.value = Convert.ToString(annualincome);
                }
                listkey.Add(keyv);
            }
            var jsono = new JavaScriptSerializer().Serialize(listkey);
            objmember.UserDataobj = jsono;
            context.SubmitChanges();

            //Update the amount of membership
            var getmembership = (from m in context.TempMembershipRegs where m.TempMembershipRegistrationId == Convert.ToInt32(MembershipRegistrationId) select m).FirstOrDefault();

            getmembership.Amount = getmembership.Amount + Convert.ToDecimal(extraamount);
            context.SubmitChanges();

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Below action is used to add an additional spouse member in the existing membership. we need to pass on the required data and then we will be able to add it to the existing enrolled membership.
        /// It will then add the additional cost to enroll the new member into the existing amount of the membership.
        /// </summary>
        /// <param name="MembershipRegistrationId"></param>
        /// <param name="usertype"></param>
        /// <param name="profilename"></param>
        /// <param name="sname"></param>
        /// <param name="fname"></param>
        /// <param name="mname"></param>
        /// <param name="blood"></param>
        /// <param name="sdob"></param>
        /// <param name="nation"></param>
        /// <param name="email"></param>
        /// <param name="pan"></param>
        /// <param name="edu"></param>
        /// <param name="occup"></param>
        /// <param name="designation"></param>
        /// <param name="annual"></param>
        /// <param name="interest"></param>
        /// <param name="phone"></param>
        /// <param name="extraamount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNewSpouseMember(string MembershipRegistrationId, string usertype, string profilename, string sname, string fname, string mname, string blood, string sdob, string nation, string email, string pan, string edu, string occup, string designation, string annual, string interest, string phone, string extraamount)
        {
            TempMember objmember = new TempMember();
            objmember.IsBlocked = false;
            objmember.IsDeleted = false;
            objmember.TempMembershipRegistrationId = Convert.ToInt32(MembershipRegistrationId);
            objmember.UserTypeId = Convert.ToInt32(usertype);
            context.TempMembers.InsertOnSubmit(objmember);
            context.SubmitChanges();

            //Enter the picture in picture table

            TblProfilePicture objpic = new TblProfilePicture();
            objpic.ProfiePictureName = Convert.ToString(profilename);
            objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
            objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
            context.TblProfilePictures.InsertOnSubmit(objpic);
            context.SubmitChanges();

            //Enter the member-picture entry in diff table
            TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
            objmappic.MemberId = objmember.TempMemberId;
            objmappic.ProfilePictureId = objpic.ProfilePictureId;
            context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
            context.SubmitChanges();


            List<KeyValue> listkey = new List<KeyValue>();
            KeyValue keyv = new KeyValue();


            var getmapplanusertype = (from gm in context.TblMapFieldUserTypes
                                      join
                                          cm in context.TblFields on gm.FieldId equals cm.FieldId
                                      where gm.UserTypeId == Convert.ToInt32(usertype)
                                      select new { gm, cm }).ToList();

            //foreach (var item in getmapplanusertype)
            for (int y = 0; y < getmapplanusertype.Count; y++)
            {
                keyv = new KeyValue();
                keyv.key = Convert.ToString(getmapplanusertype[y].gm.FieldId);
                //var st = "smem_" + usertype + "_" + item.cm.FieldName;
                if (y == 0)
                {
                    keyv.value = Convert.ToString(sname);
                }
                else if (y == 1)
                {
                    keyv.value = Convert.ToString(fname);
                }
                else if (y == 2)
                {
                    keyv.value = Convert.ToString(mname);
                }
                else if (y == 3)
                {
                    keyv.value = Convert.ToString(blood);
                }
                else if (y == 4)
                {
                    keyv.value = Convert.ToString(sdob);
                }
                else if (y == 5)
                {
                    keyv.value = Convert.ToString(nation);
                }
                else if (y == 6)
                {
                    keyv.value = Convert.ToString(email);
                }
                else if (y == 7)
                {
                    keyv.value = Convert.ToString(pan);
                }
                else if (y == 8)
                {
                    keyv.value = Convert.ToString(edu);
                }
                else if (y == 9)
                {
                    keyv.value = Convert.ToString(occup);
                }
                else if (y == 10)
                {
                    keyv.value = Convert.ToString(designation);
                }
                else if (y == 11)
                {
                    keyv.value = Convert.ToString(annual);
                }
                else if (y == 12)
                {
                    keyv.value = Convert.ToString(interest);
                }
                else if (y == 13)
                {
                    keyv.value = Convert.ToString(phone);
                }
                listkey.Add(keyv);
            }
            var jsono = new JavaScriptSerializer().Serialize(listkey);
            objmember.UserDataobj = jsono;
            context.SubmitChanges();

            //Update the amount of membership
            var getmembership = (from m in context.TempMembershipRegs where m.TempMembershipRegistrationId == Convert.ToInt32(MembershipRegistrationId) select m).FirstOrDefault();

            getmembership.Amount = getmembership.Amount + Convert.ToDecimal(extraamount);
            context.SubmitChanges();

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Below action is used to add an additional child member in the existing membership. we need to pass on the required data and then we will be able to add it to the existing enrolled membership.
        /// It will then add the additional cost to enroll the new member into the existing amount of the membership.
        /// </summary>
        /// <param name="MembershipRegistrationId"></param>
        /// <param name="usertype"></param>
        /// <param name="profilename"></param>
        /// <param name="name"></param>
        /// <param name="blood"></param>
        /// <param name="gender"></param>
        /// <param name="childdob"></param>
        /// <param name="school"></param>
        /// <param name="interest"></param>
        /// <param name="allergy"></param>
        /// <param name="extraamount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNewChildMember(string MembershipRegistrationId, string usertype, string profilename, string name, string blood, string gender, string childdob, string school, string interest, string allergy, string extraamount)
        {
            TempMember objmember = new TempMember();
            objmember.IsBlocked = false;
            objmember.IsDeleted = false;
            objmember.TempMembershipRegistrationId = Convert.ToInt32(MembershipRegistrationId);
            objmember.UserTypeId = Convert.ToInt32(usertype);
            context.TempMembers.InsertOnSubmit(objmember);
            context.SubmitChanges();

            //Enter the picture in picture table

            TblProfilePicture objpic = new TblProfilePicture();
            objpic.ProfiePictureName = Convert.ToString(profilename);
            objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
            objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
            context.TblProfilePictures.InsertOnSubmit(objpic);
            context.SubmitChanges();

            //Enter the member-picture entry in diff table
            TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
            objmappic.MemberId = objmember.TempMemberId;
            objmappic.ProfilePictureId = objpic.ProfilePictureId;
            context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
            context.SubmitChanges();


            List<KeyValue> listkey = new List<KeyValue>();
            KeyValue keyv = new KeyValue();


            var getmapplanusertype = (from gm in context.TblMapFieldUserTypes
                                      join
                                          cm in context.TblFields on gm.FieldId equals cm.FieldId
                                      where gm.UserTypeId == Convert.ToInt32(usertype)
                                      select new { gm, cm }).ToList();

            //foreach (var item in getmapplanusertype)
            for (int y = 0; y < getmapplanusertype.Count; y++)
            {
                keyv = new KeyValue();
                keyv.key = Convert.ToString(getmapplanusertype[y].gm.FieldId);
                //var st = "smem_" + usertype + "_" + item.cm.FieldName;
                if (y == 0)
                {
                    keyv.value = Convert.ToString(name);
                }
                else if (y == 1)
                {
                    keyv.value = Convert.ToString(blood);
                }
                else if (y == 2)
                {
                    keyv.value = Convert.ToString(gender);
                }
                else if (y == 3)
                {
                    keyv.value = Convert.ToString(childdob);
                }
                else if (y == 4)
                {
                    keyv.value = Convert.ToString(school);
                }
                else if (y == 5)
                {
                    keyv.value = Convert.ToString(interest);
                }
                else if (y == 6)
                {
                    keyv.value = Convert.ToString(allergy);
                }
                listkey.Add(keyv);
            }
            var jsono = new JavaScriptSerializer().Serialize(listkey);
            objmember.UserDataobj = jsono;
            context.SubmitChanges();

            //Update the amount of membership
            var getmembership = (from m in context.TempMembershipRegs where m.TempMembershipRegistrationId == Convert.ToInt32(MembershipRegistrationId) select m).FirstOrDefault();

            getmembership.Amount = getmembership.Amount + Convert.ToDecimal(extraamount);
            context.SubmitChanges();

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Below action is used to add an additional senior member in the existing membership. we need to pass on the required data and then we will be able to add it to the existing enrolled membership.
        /// It will then add the additional cost to enroll the new member into the existing amount of the membership.
        /// </summary>
        /// <param name="MembershipRegistrationId"></param>
        /// <param name="usertype"></param>
        /// <param name="profilename"></param>
        /// <param name="seniorname"></param>
        /// <param name="senioraddress"></param>
        /// <param name="seniorphone"></param>
        /// <param name="seniordob"></param>
        /// <param name="extraamount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNewSeniorMember(string MembershipRegistrationId, string usertype, string profilename, string seniorname, string senioraddress, string seniorphone, string seniordob, string extraamount)
        {
            TempMember objmember = new TempMember();
            objmember.IsBlocked = false;
            objmember.IsDeleted = false;
            objmember.TempMembershipRegistrationId = Convert.ToInt32(MembershipRegistrationId);
            objmember.UserTypeId = Convert.ToInt32(usertype);
            context.TempMembers.InsertOnSubmit(objmember);
            context.SubmitChanges();

            //Enter the picture in picture table

            TblProfilePicture objpic = new TblProfilePicture();
            objpic.ProfiePictureName = Convert.ToString(profilename);
            objpic.CreatedBy = SunCity.Core.Session.Current.UserId;
            objpic.CreatedDate = DateTime.UtcNow.AddHours(5.50);
            context.TblProfilePictures.InsertOnSubmit(objpic);
            context.SubmitChanges();

            //Enter the member-picture entry in diff table
            TblMapMemberProfilePicture objmappic = new TblMapMemberProfilePicture();
            objmappic.MemberId = objmember.TempMemberId;
            objmappic.ProfilePictureId = objpic.ProfilePictureId;
            context.TblMapMemberProfilePictures.InsertOnSubmit(objmappic);
            context.SubmitChanges();


            List<KeyValue> listkey = new List<KeyValue>();
            KeyValue keyv = new KeyValue();


            var getmapplanusertype = (from gm in context.TblMapFieldUserTypes
                                      join
                                          cm in context.TblFields on gm.FieldId equals cm.FieldId
                                      where gm.UserTypeId == Convert.ToInt32(usertype)
                                      select new { gm, cm }).ToList();

            //foreach (var item in getmapplanusertype)
            for (int y = 0; y < getmapplanusertype.Count; y++)
            {
                keyv = new KeyValue();
                keyv.key = Convert.ToString(getmapplanusertype[y].gm.FieldId);
                //var st = "smem_" + usertype + "_" + item.cm.FieldName;
                if (y == 0)
                {
                    keyv.value = Convert.ToString(seniorname);
                }
                else if (y == 1)
                {
                    keyv.value = Convert.ToString(senioraddress);
                }
                else if (y == 2)
                {
                    keyv.value = Convert.ToString(seniorphone);
                }
                else if (y == 3)
                {
                    keyv.value = Convert.ToString(seniordob);
                }
                listkey.Add(keyv);
            }
            var jsono = new JavaScriptSerializer().Serialize(listkey);
            objmember.UserDataobj = jsono;
            context.SubmitChanges();

            //Update the amount of membership
            var getmembership = (from m in context.TempMembershipRegs where m.TempMembershipRegistrationId == Convert.ToInt32(MembershipRegistrationId) select m).FirstOrDefault();

            getmembership.Amount = getmembership.Amount + Convert.ToDecimal(extraamount);
            context.SubmitChanges();

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        //-------------------Block for New Membership Enrolment Ends--------------------------------------

        [HttpPost]
        public ActionResult checkMembershipNo(string membershipnumber)
        {
            //membershipno = Request.Form["membershipnumber"];
            var db = new SuncityDataContext();

            var query = (from m in db.TempMembershipRegs
                         where (m.TempMembershipNo == membershipnumber)
                         select m).FirstOrDefault();
            if (query != null)
                return Content("true");
            else
                return Content("false");
        }
    }
}
