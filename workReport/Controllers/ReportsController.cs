using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
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
            ViewBag.months = new SelectList(db.nepMonths, "monthId", "monthName");
            return View();
        }

        public ActionResult report1()
        {
            ViewBag.months = new SelectList(db.nepMonths, "monthId", "monthName");
            return View();
        }

        public ActionResult dailyReport(FormCollection fc)
        {
            var obj = new workReportEntities();
            string nepMonthstr = "0";
            int nepmonth = Convert.ToInt32(fc["months"]);
            if (nepmonth < 10)
            {
                nepMonthstr = '0' + nepmonth.ToString();
            }
            else
            {
                nepMonthstr = nepmonth.ToString();
            }
            string todaysDateEng = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime todaysDateNep = Convert.ToDateTime(obj.PR_engtonep(engdate: todaysDateEng).FirstOrDefault());
            ViewBag.todaysYear = todaysDateNep.Year;
            int todaysMonth = todaysDateNep.Month;
            ViewBag.todaysMonth = db.nepMonths.AsNoTracking().Where(x => x.monthId == todaysMonth).Select(x => x.monthName).FirstOrDefault();
            ViewBag.todaysDay = todaysDateNep.Day;


            nepMonths nepaliMonth = db.nepMonths.AsNoTracking().Where(x => x.monthId == nepmonth).FirstOrDefault();
            ViewBag.nepmonth = nepaliMonth.monthName;
            int nepYear = 2077;
            int nepDay = 1;
            string nepDaystr = '0' + nepDay.ToString();
            string nepDate = nepYear.ToString() + '-' + nepMonthstr + '-' + nepDaystr;
            DateTime engDate = Convert.ToDateTime(obj.PR_neptoeng(nepdate: nepDate).FirstOrDefault());
            int engYear = engDate.Year;
            int engMonth = engDate.Month;

            int daysInMonth = Convert.ToInt32(db.COM_ENGLISH_NEPALI_DATE.AsNoTracking().Where(x => x.NEPALI_YEAR == nepYear && x.MONTH_CD == nepmonth).Select(x => x.NO_OF_DAYS).FirstOrDefault());


            string isUserName = Session["userName"].ToString();
            int isUserId = Convert.ToInt32(Session["userId"]);

            var userdet = db.user.AsNoTracking().Where(x => x.usrId == isUserId).FirstOrDefault();
            var userPost = db.posts.AsNoTracking().Where(x => x.postId == userdet.post).FirstOrDefault();
            ViewBag.userPost = userPost.postName;

            ViewBag.isReportMonth = nepaliMonth.monthName;
            ViewBag.isReportYear = nepYear;
            ViewBag.isUserBank = userdet.bankName;
            ViewBag.isBankAdd = userdet.branch;
            ViewBag.isAccountNo = userdet.acnumber;
            string userfullname = userdet.firstName + " " + userdet.lastName;
            ViewBag.reporterName = userfullname;
            //  int isMonth = Convert.ToInt32(fc["months"]);
            ViewBag.isReportDate = nepDate;
            ViewBag.isContractAmount = userdet.totalAmount;

            ViewBag.isUserSalary = userdet.monthlySalary.ToString();
            var lastengdate = engDate.AddDays(daysInMonth);

            List<WorkListModel> WorksList = new List<WorkListModel>();
            //int? xxx = 0, xxy = 0, xxz = 0, xyx = 0;
            for (int i = 0; i < daysInMonth; i++)
            {
                
                WorkListModel model = new WorkListModel();
                model.isDay = i + 1;
                model.date_Eng = Convert.ToDateTime(engDate.AddDays(i).ToShortDateString());
                var nextday = Convert.ToDateTime(engDate.AddDays(i + 1).ToShortDateString());
                var model1 = db.workList.Where(x => x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).ToList();
               
                foreach(var mmm in model1)
                {
                    WorkListModel nmodel = new WorkListModel();
                    nmodel.DayofMonth = Convert.ToDateTime(mmm.date_Eng).DayOfWeek.ToString();
                    nmodel.workListType = mmm.workListType;
                    nmodel.mun = mmm.mun;
                    nmodel.time = mmm.time;
                    nmodel.isDay = model.isDay;
                    nmodel.workDet = mmm.workDet;
                    nmodel.issue = mmm.issue;
                    WorksList.Add(nmodel);
                }



                //model.DayofMonth = Convert.ToDateTime(model.date_Eng).DayOfWeek.ToString();
                //// string datess = obj.PR_engtonep(engdate: model.date_Eng.ToString()).FirstOrDefault();
                //model.totalCountAnydesk = db.workList.Where(x => x.workListType == "Anydesk" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng <= nextday).Select(x => x.time).Sum();
                //model.totalRows = db.workList.Where(x => x.date_Eng >= model.date_Eng && x.users == isUserId && x.date_Eng <= nextday).Select(x => x.workListId).Count();
                //model.totalCountCall = db.workList.Where(x => x.workListType == "Call" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng <= nextday).Select(x => x.time).Sum();
                //model.totalCountEmail = db.workList.Where(x => x.workListType == "Email" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng <= nextday).Select(x => x.time).Sum();
                //xxx += model.totalCountEmail ?? 0;
                //xxy += model.totalCountCall ?? 0;
                //xxz += model.totalCountAnydesk ?? 0;
                //xyx += model.totalRows ?? 0;
                //WorksList.Add(model);
            }
            if (Convert.ToInt32(Session["userPost"]) == 7 || Convert.ToInt32(Session["userPost"]) == 8)
            {
                return View("dailyReport1", WorksList);
            }
            else
            {

                return View(WorksList);
            }
        }
        public class Ram
        {
            public string CallDetails { get; set; }
            public int ? WorkCount { get; set; }
        }
        public JsonResult GetData(string mdate)
        {
            DateTime isdate = Convert.ToDateTime(mdate);

            int isUserId = Convert.ToInt32(Session["userId"]);
            WorkListModel worklists = new WorkListModel();
            int? callTotal =  db.workList.Where(x => x.workListType == "Call" && x.users == isUserId && x.date_Eng ==isdate).Select(x => x.time).Sum();
            int? anydeskTotal = db.workList.Where(x => x.workListType == "Anydesk" && x.users == isUserId && x.date_Eng == isdate).Select(x => x.time).Sum();
            int? emailTotal = db.workList.Where(x => x.workListType == "Email" && x.users == isUserId && x.date_Eng == isdate).Select(x => x.time).Sum();
            List<Ram> Datalists = new List<Ram>();
            Ram model = new Ram();
            model.CallDetails = "Call Total";
            model.WorkCount = callTotal??0;
            Datalists.Add(model);
             model = new Ram();
            model.CallDetails = "Any Desk Total";
            model.WorkCount = anydeskTotal ?? 0;
            Datalists.Add(model);
            model = new Ram();
            model.CallDetails = "Email Total";
            model.WorkCount = emailTotal ?? 0;
            Datalists.Add(model);

            List<SelectListItem> OptionName = new List<SelectListItem>();
            OptionName.Add(new SelectListItem { Text = "callTotal", Value = callTotal.ToString() });
            OptionName.Add(new SelectListItem { Text = "anydeskTotal", Value = anydeskTotal.ToString() });
            OptionName.Add(new SelectListItem { Text = "emailTotal", Value = emailTotal.ToString() });
           
            //for (int i = 1; i <= count; i++)
            //{
            //    string a = i.ToString();
            //    Wards.Add(new SelectListItem { Text = a, Value = i.ToString() });
            //}
            //var viewmodel = Wards.Select(x => new
            //{
            //    ward_id = x.Value,
            //    ward_name = x.Text
            //});
            return Json(Datalists, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(FormCollection fc)
        {
            var obj = new workReportEntities();
            string nepMonthstr="0";
            int nepmonth = Convert.ToInt32(fc["months"]);
            if(nepmonth<10)
            {
                nepMonthstr = '0' + nepmonth.ToString();
            }
            else
            {
                nepMonthstr = nepmonth.ToString();
            }
            string todaysDateEng = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime todaysDateNep = Convert.ToDateTime(obj.PR_engtonep(engdate: todaysDateEng).FirstOrDefault());
            ViewBag.todaysYear = todaysDateNep.Year;
            int todaysMonth = todaysDateNep.Month;
            ViewBag.todaysMonth = db.nepMonths.AsNoTracking().Where(x => x.monthId == todaysMonth).Select(x=>x.monthName).FirstOrDefault();
            ViewBag.todaysDay = todaysDateNep.Day;


            nepMonths nepaliMonth = db.nepMonths.AsNoTracking().Where(x => x.monthId == nepmonth).FirstOrDefault();
            ViewBag.nepmonth = nepaliMonth.monthName;
            int nepYear = 2077;
            int nepDay = 1;
            string nepDaystr = '0' + nepDay.ToString();
            string nepDate = nepYear.ToString() + '-' + nepMonthstr + '-' + nepDaystr ;
            DateTime engDate =Convert.ToDateTime( obj.PR_neptoeng(nepdate: nepDate).FirstOrDefault());
            int engYear = engDate.Year;
            int engMonth = engDate.Month;

            int daysInMonth = Convert.ToInt32( db.COM_ENGLISH_NEPALI_DATE.AsNoTracking().Where(x => x.NEPALI_YEAR == nepYear && x.MONTH_CD == nepmonth).Select(x => x.NO_OF_DAYS).FirstOrDefault());


            string isUserName = Session["userName"].ToString();
            int isUserId = Convert.ToInt32(Session["userId"]);

            var userdet = db.user.AsNoTracking().Where(x => x.usrId == isUserId).FirstOrDefault();
            var userPost = db.posts.AsNoTracking().Where(x => x.postId == userdet.post).FirstOrDefault();
            ViewBag.userPost = userPost.postName;
           
            ViewBag.isReportMonth = nepaliMonth.monthName;
            ViewBag.isReportYear =nepYear;
            ViewBag.isUserBank = userdet.bankName;
            ViewBag.isBankAdd = userdet.branch;
            ViewBag.isAccountNo = userdet.acnumber;
            string userfullname = userdet.firstName + " " + userdet.lastName;
            ViewBag.reporterName = userfullname;
          //  int isMonth = Convert.ToInt32(fc["months"]);
            ViewBag.isReportDate = nepDate;
            ViewBag.isContractAmount = userdet.totalAmount;

            ViewBag.isUserSalary = userdet.monthlySalary.ToString();
            var lastengdate = engDate.AddDays(daysInMonth);
            //var list=db.workList.Where(x=>x.date_Eng>= engDate && x.date_Eng<=lastengdate).Select
            List<WorkListModel> WorksList = new List<WorkListModel>();
            int? xxx = 0,xxy = 0,xxz = 0,xyx=0;
            for (int i = 0; i < daysInMonth; i++)
            {
                WorkListModel model = new WorkListModel();
                model.date_Eng = Convert.ToDateTime(engDate.AddDays(i).ToShortDateString());
                var nextday= Convert.ToDateTime(engDate.AddDays(i+1).ToShortDateString());
                model.DayofMonth = Convert.ToDateTime(model.date_Eng).DayOfWeek.ToString();
               // string datess = obj.PR_engtonep(engdate: model.date_Eng.ToString()).FirstOrDefault();
                model.totalCountAnydesk = db.workList.Where(x => x.workListType == "Anydesk" && x.users==isUserId && x.date_Eng >= model.date_Eng && x.date_Eng<nextday).Select(x => x.time).Sum();
                model.totalRows = db.workList.Where(x => x.date_Eng >= model.date_Eng && x.users == isUserId && x.date_Eng < nextday).Select(x => x.workListId).Count();
                model.totalCountCall = db.workList.Where(x => x.workListType == "Call" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.time).Sum();
                model.totalCountEmail = db.workList.Where(x => x.workListType == "Email" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.time).Sum();
                xxx += model.totalCountEmail ??0;
                xxy += model.totalCountCall ??0;
                xxz += model.totalCountAnydesk??0;
                xyx += model.totalRows ?? 0;
                WorksList.Add(model);
            }
            ////  https://stackoverflow.com/questions/34881975/asp-net-mvc-sales-each-day-in-a-month
            WorkListModel nmodel = new WorkListModel();
            ViewBag.TotalEmail = xxx;
            ViewBag.TotalCall = xxy;
            ViewBag.TotalAnydesk = xxz;
            ViewBag.TotalRows = xyx;

            //-----------------------------salary table----------------------------
            DateTime userContractDateNep =Convert.ToDateTime(userdet.contractDate);
            ViewBag.contractYear = userContractDateNep.Year;
            int contractMonth = userContractDateNep.Month;
            ViewBag.contractMonth = db.nepMonths.AsNoTracking().Where(x => x.monthId == contractMonth).Select(x => x.monthName).FirstOrDefault();
            ViewBag.contractDay = userContractDateNep.Day;
            ViewBag.isContractDate = userContractDateNep;
            DateTime userContractDateEng = Convert.ToDateTime(obj.PR_neptoeng(nepdate: userContractDateNep.ToString("yyyy-MM-dd")).FirstOrDefault());
            //TimeSpan diff = engDate - userContractDateEng;
            //int totalMins = (int)diff.TotalMinutes;
            //int totalMonths = Convert.ToInt32(Math.Ceiling((((Convert.ToDouble(totalMins)) / 60) / 24) / 30));
            //    int prevMonthAmount =Convert.ToInt32( (totalMonths - 1) * userdet.monthlySalary);
            //      ViewBag.prevMonthAmount = prevMonthAmount;
            //      int totalThisMonthAmount =Convert.ToInt32( prevMonthAmount + userdet.monthlySalary);
            //    ViewBag.totalThisMonthAmount = totalThisMonthAmount;

            string enterDate = userdet.enteredDate;
            DateTime enteredDateNep = Convert.ToDateTime(obj.PR_engtonep(engdate: enterDate).FirstOrDefault());
            DateTime nepDate1 = Convert.ToDateTime( (nepYear.ToString() + '-' + nepMonthstr + '-' + 25));

            TimeSpan differ = nepDate1 - enteredDateNep;
            int totalMins = (int)differ.TotalMinutes;
            int totalMonths = Convert.ToInt32(Math.Ceiling((((Convert.ToDouble(totalMins)) / 60) / 24) / 30));
            int prevMonthAmount = Convert.ToInt32( userdet.lastMonthAmount ) +  Convert.ToInt32((totalMonths ) * userdet.monthlySalary);
            ViewBag.prevMonthAmount = prevMonthAmount;
            int totalThisMonthAmount =Convert.ToInt32( prevMonthAmount + userdet.monthlySalary);
            ViewBag.totalThisMonthAmount = totalThisMonthAmount;
            int remAmount = Convert.ToInt32(userdet.totalAmount - totalThisMonthAmount);
            ViewBag.remAmount = remAmount;











            //--------------------------report daily table----------------------

            //     var days = Enumerable.Range(1, daysInMonth);
            //     var query = db.workList.Where(x => x.date_Eng.Value.Year == engYear && x.date_Eng.Value.Month == engMonth).Select(g => new
            //     {

            //         day=g.date_Eng.Value.Day,
            //         email=20,

            //     });




            if (Convert.ToInt32(Session["userPost"]) == 7 || Convert.ToInt32(Session["userPost"]) == 8)
            {
                return View("Index1", WorksList);
            }
            else { 

            return View(WorksList);
            }
        }

    }
}