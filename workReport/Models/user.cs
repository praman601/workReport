//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace workReport.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class user
    {
        public int usrId { get; set; }
        public string userName { get; set; }
        public string userPassword { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Nullable<int> post { get; set; }
        public string contractDate { get; set; }
        public Nullable<int> monthlySalary { get; set; }
        public Nullable<int> totalAmount { get; set; }
        public string bankName { get; set; }
        public string branch { get; set; }
        public string acnumber { get; set; }
        public Nullable<int> lastMonthAmount { get; set; }
        public string enteredDate { get; set; }
    }
}
