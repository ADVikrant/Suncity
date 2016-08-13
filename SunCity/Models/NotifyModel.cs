using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class NotifyModel
    {
        public List<DisplayFields> listmarriage { get; set; }
        public List<DisplayFields> listbirthday { get; set; }
    }

    public class DisplayFields {
        public int MemberId { get; set; }
        public string Membername { get; set; }
    
    }
}