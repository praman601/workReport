using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace workReport.Models
{
    public partial class user1
    {
        public int usrId { get; set; }
        public string userName { get; set; }
        public string userPassword { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Nullable<int> post { get; set; }

        
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> contractDate { get; set; }
        public Nullable<int> monthlySalary { get; set; }
        public Nullable<int> totalAmount { get; set; }
        public string bankName { get; set; }
        public string branch { get; set; }
        public string acnumber { get; set; }
    }
}