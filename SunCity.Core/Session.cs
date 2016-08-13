using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace SunCity.Core
{
    public class Session
    {
        public Session()
        {

        }

        // Gets the current session.

        public static Session Current
        {
            get
            {
                Session session = (Session)HttpContext.Current.Session["__MySession__"];

                if (session == null)
                {
                    session = new Session();
                    HttpContext.Current.Session["__MySession__"] = session;
                    Session.Current.Username = string.Empty;
                    HttpContext.Current.Session.Timeout = 900;
                }
                return session;
            }
        }

        // **** add your session properties here, e.g like this:
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Username { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int flgrt { get; set; }

    }
}
