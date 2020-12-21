using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using workReport.Models;

namespace workReport.Controllers
{
    public class ReportsController : SessionCheckController
    {
        private workReportEntities db = new workReportEntities();

        // GET: Reports
        public ActionResult report()
        {
            ViewBag.months = new SelectList(db.Months, "monthId", "monthName");
            return View();
        }


        public ActionResult Index(FormCollection fc)
        {
            string isUser = Session["userName"].ToString();
            int isMonth = Convert.ToInt32(fc["months"]);



            return View();
        }

    }
}