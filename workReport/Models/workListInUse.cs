﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;



namespace workReport.Models
{
    public partial class workList1
    {
        public int workListId { get; set; }
        public string workListType { get; set; }
        public string mun { get; set; }
        public Nullable<int> time { get; set; }
        public Nullable<int> users { get; set; }
        [DataType(DataType.Date)]
        public string date { get; set; }
        public string issue { get; set; }
        public Nullable<System.DateTime> date_Eng { get; set; }
    }
    public class WorkListModel
        {
        
        public int workListId { get; set; }
    public string workListType { get; set; }
    public string mun { get; set; }
    public Nullable<int> time { get; set; }
    public Nullable<int> users { get; set; }
    public string date { get; set; }
    public string issue { get; set; }
        public string AddedDateString { get; set; }
        public Nullable<System.DateTime> date_Eng { get; set; }

        public Nullable<decimal> LastTradedPrice { get; set; }
        public string workDet { get; set; }
        public int? call { get; set; }
        public int? anydesk { get; set; }
        public int? email { get; set; }
        public int? totalCountCall { get; set; }
        public int? workType { get; set; }
        public int? totalCountAnydesk { get; set; }
        public int? totalCountEmail { get; set; }
        public string DayofMonth { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public int? totalCall { get; set; }
        public int? isDay { get; set; }
        public int? totalAnydesk { get; set; }
        public int? totalEmail { get; set; }
        public int? totalSum { get; set; }
        public int? totalRows { get; set; }
 
    }

    public partial class workListone
    {
        //public int workListId { get; set; }
        public string workListType { get; set; }
        public string mun { get; set; }
        public Nullable<int> time { get; set; }
        public Nullable<int> users { get; set; }
        public string date { get; set; }
        public string issue { get; set; }
        //public string date_Eng { get; set; }
        public string workDet { get; set; }
    }
    public partial class workListnew
    {
        public double workListId { get; set; }
        public string workListType { get; set; }
        public string mun { get; set; }
        public Nullable<double> time { get; set; }
        public Nullable<double> users { get; set; }
        public string date { get; set; }
        public string issue { get; set; }
        public string date_Eng { get; set; }
        public string workDet { get; set; }

        public int TotalCount { get; set; }
    }

    public partial class workListProc
    {
        public int users { get; set; }
        public int TotalCount { get; set; }
    }

    public partial class deleteWorkListData
    {
        public int isUser { get; set; }
        public string isYear { get; set; }

    }

}