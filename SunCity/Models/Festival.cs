using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class Festival
    {
        public int NotificationId { get; set; }
        public string FestivalName { get; set; }
        public string FestivalNote { get; set; }
        public string FestivalDate { get; set; }
    }
}