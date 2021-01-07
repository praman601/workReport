using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace workReport.Models
{
    public class report
    {
        public int Day { get; set; }
        public string DayName { get; set; }
        public int Email { get; set; }
        public int Calls { get; set; }
        public int Anydesk { get; set; }

    }
}