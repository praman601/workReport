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
    
    public partial class JN_WORKLIST
    {
        public int workListId { get; set; }
        public string workListType { get; set; }
        public string mun { get; set; }
        public Nullable<int> time { get; set; }
        public Nullable<int> users { get; set; }
        public string date { get; set; }
        public string issue { get; set; }
        public Nullable<System.DateTime> date_Eng { get; set; }
        public string workDet { get; set; }
    }
}
