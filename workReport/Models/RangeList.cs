using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace workReport.Models
{
    public class RangeList
    {
   //     public int workListId { get; set; }
        public string workListType { get; set; }
        public string mun { get; set; }
        public Nullable<int> time { get; set; }
    //    public Nullable<int> users { get; set; }
        public string date { get; set; }
        public string issue { get; set; }
   //     public Nullable<System.DateTime> date_Eng { get; set; }
     //   public string workDet { get; set; }
    }
}