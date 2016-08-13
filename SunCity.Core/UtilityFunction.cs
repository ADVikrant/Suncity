using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using SunCity.DAL;


namespace SunCity.Core
{
    public static class UtilityFunction
    {
        public static string EncryptData(string Message, string passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToEncrypt = UTF8.GetBytes(Message);
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return Convert.ToBase64String(Results);
        }
        public static string DecryptString(string Message, string passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(Message);
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return UTF8.GetString(Results);
        }

       
        public static string getRollName(int rollid)
        {
            string roleName = "";
            if (rollid == Convert.ToInt32(Enum.Rollname.SuperAdmin))
            {
                roleName = "SuperAdmin";
            }
            else if (rollid == Convert.ToInt32(Enum.Rollname.Admin))
            {
                roleName = "Admin";
            }
            else if (rollid == Convert.ToInt32(Enum.Rollname.Manager))
            {
                roleName = "Manager";
            }
            else if (rollid == Convert.ToInt32(Enum.Rollname.Employee))
            {
                roleName = "Employee";
            }
           
            return roleName;
        }

        public static bool hasPermission(int permissionid)
        {
            SuncityDataContext context = new SuncityDataContext();
            var rolelist = SunCity.Core.Session.Current.RoleId;

            var d = context.PermissionRecord_Role_Mappings.Where(x => x.PermissionRecord_Id == permissionid && x.Role_Id == rolelist).ToList();

            if (d.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool hasPermission(SuncityDataContext context, int permissionid)
        {
            var rolelist = SunCity.Core.Session.Current.RoleId;

            var d = context.PermissionRecord_Role_Mappings.Where(x => x.PermissionRecord_Id == permissionid && x.Role_Id == rolelist).ToList();

            if (d.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool LogActivity(string message,TextWriter sw)
        {
            sw.Write("\r\nLog Entry :\r\n ");
            sw.WriteLine("Date Time :" + DateTime.UtcNow.AddHours(5.50));
            sw.WriteLine("Activity:- " + message);
            sw.WriteLine("----------------------------------------------------------------------------------");
            sw.Close();
            return true;
        
        }

        public static bool SendHtmlMail(string mailTo, string mailToDisplayName, string Subject, string HtmlData, string mailFrom, string mailFromDisplayName = "Suncity Club")
        {
            try
            {
                if (mailFrom == "")
                {
                    mailFrom = "support@suncitybaroda.in";
                }
                //string mailFromDisplayName = "Alpha Dezine OMS";

                MailMessage mailMsg = new MailMessage();
                // To
                mailMsg.To.Add(new MailAddress(mailTo, mailToDisplayName));

                // From
                mailMsg.From = new MailAddress(mailFrom, mailFromDisplayName);

                // Subject and multipart/alternative Body
                mailMsg.Subject = Subject;

                string text = "";
                string html = HtmlData;
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                //SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["SendgridUsername"].ToString(), ConfigurationManager.AppSettings["SendgridPassword"].ToString());
                //smtpClient.Credentials = credentials;
                SmtpClient smtpClient = new SmtpClient("mail.kelp.arvixe.com", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["ArvixeUsername"].ToString(), ConfigurationManager.AppSettings["ArvixePassword"].ToString());
                smtpClient.Credentials = credentials;

                smtpClient.Send(mailMsg);
                return true;
            }
            catch (Exception ex)
            {
                //LogExceptionDetail(ex);
                return false;
            }
        }

    }
}
