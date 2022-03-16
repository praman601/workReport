using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using workReport.Models;

using System.Web.Mvc;

using PagedList;
using System.IO;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Reflection;
using ClosedXML.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using workReport.Controllers;

namespace workReport.Providers
{
    public class DashData{
        public string callsCount { get; set; }
        public string anydesksCount { get; set; }
        public string emailsCount { get; set; }
    }
    public class Utility 
    {
        private readonly SessionCheckController _stt;
        public Utility(SessionCheckController stt)
        {
            _stt = stt;
        }
        public  DashData getTodaysDataa(string todaysDate)
        {
         
            var av = HttpContext.Current.Session["userId"].ToString();
            int isuser = Convert.ToInt32(HttpContext.Current.Session["userId"]);
            DashData data = new DashData();
            DateTime isDate = Convert.ToDateTime(todaysDate);
            using (var db =new  workReportEntities()) { 
            var todaysData = db.workList.Where(x => x.date_Eng == isDate).ToList();
                 data.callsCount = todaysData.Where(x => x.workListType == "Call").Sum(x => x.time).ToString();
                data.anydesksCount = todaysData.Where(x => x.workListType == "Anydesk").Sum(x => x.time).ToString();
                data.emailsCount = todaysData.Where(x => x.workListType == "Email").Sum(x => x.time).ToString();
      
                return data;
                }

        }
    }
}