using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunCity.Models
{
    public class PackageActivityList
    {
        public int PackageId { get; set; }
        public int ActivityId { get; set; }
        public bool MappedStatus { get; set; }
        public string ActivityName { get; set; }
        public string CheckedStatus { get; set; }
    }
}