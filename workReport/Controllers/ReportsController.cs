using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using workReport.Models;
using workReport;
using PagedList.Mvc;
using PagedList;
using System.Globalization;

namespace workReport.Controllers
{





    public class ReportsController : SessionCheckController
    {
        private workReportEntities db = new workReportEntities();

        // GET: Reports

        public ActionResult DateConvert()
        {
            ViewBag.NepYears = new SelectList(db.COM_ENGLISH_NEPALI_DATE.Select(x => x.NEPALI_YEAR).Distinct().OrderByDescending(x => x.Value), "NEPALI_YEAR");
            return View();
        }

        public JsonResult DateConvertor(int year, int month, int day, int bsadchange)
        {
            string monthh = (month <= 9) ? "0" + month : month.ToString();
            string dayy = (day <= 9) ? "0" + day : day.ToString();

            string isdate = year + "-" + monthh + "-" + dayy;


            if (bsadchange == 1)
            {
                string x = db.PR_engtonep(engdate: isdate).FirstOrDefault();

                return Json(x, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DateTime y = Convert.ToDateTime(db.PR_neptoeng(nepdate: isdate).FirstOrDefault());
                string x = y.ToString("yyyy-MM-dd");
                return Json(x, JsonRequestBehavior.AllowGet);
            }


        }


        public ActionResult report()
        {
            ViewBag.months = new SelectList(db.nepMonths, "monthId", "monthName");
            ViewBag.fscYear = new SelectList(db.repYear, "yearId", "yearName");
            return View();
        }

        public ActionResult report1()
        {
            ViewBag.months = new SelectList(db.nepMonths, "monthId", "monthName");
            ViewBag.fscYear = new SelectList(db.repYear, "yearId", "yearName");
            return View();
        }
        //[OutputCache(Duration = 1000, VaryByParam = "fc")]
        public ActionResult dailyReport(FormCollection fc)
        {
            var obj = new workReportEntities();
            string nepMonthstr = "0";
            int nepYear = Convert.ToInt32(fc["fscYear"]);
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

                foreach (var mmm in model1)
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
            public int? WorkCount { get; set; }
        }
        public JsonResult GetData(string mdate)
        {
            DateTime isdate = Convert.ToDateTime(mdate);

            int isUserId = Convert.ToInt32(Session["userId"]);
            WorkListModel worklists = new WorkListModel();
            int? callTotal = db.workList.Where(x => x.workListType == "Call" && x.users == isUserId && x.date_Eng == isdate).Select(x => x.time).Sum();
            int? anydeskTotal = db.workList.Where(x => x.workListType == "Anydesk" && x.users == isUserId && x.date_Eng == isdate).Select(x => x.time).Sum();
            int? emailTotal = db.workList.Where(x => x.workListType == "Email" && x.users == isUserId && x.date_Eng == isdate).Select(x => x.time).Sum();
            List<Ram> Datalists = new List<Ram>();
            Ram model = new Ram();
            model.CallDetails = "Call Total";
            model.WorkCount = callTotal ?? 0;
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



        public ActionResult worklistGraph()
        {
            ViewBag.isMonth = new SelectList(db.nepMonths, "monthId", "monthName");
            ViewBag.isYear = new SelectList(db.repYear, "yearId", "yearName");
            return View();
        }


        public ActionResult GetGraphData(string isYear, string isMonth)
        {
            int isUserId = Convert.ToInt32(Session["userId"]);

            using (var ent = new workReportEntities())
            {
                //if (isMonth == null)
                //{
                //    string startDateNep = isYear + "-" + "01" + "-" + "01";
                //    string endDateNep = isYear + "-" + "12" + "-" + "30";
                //    DateTime startDateEng = Convert.ToDateTime(db.PR_neptoeng(nepdate: startDateNep).FirstOrDefault());
                //    DateTime endDateEng = Convert.ToDateTime(db.PR_neptoeng(nepdate: endDateNep).FirstOrDefault());

                //    //  SumScore = ent.workList.Where(x => x.users == isUserId && x.date_Eng >= startDateEng && x.date_Eng < endDateEng)
                //    //.Select(x => new WorkListModel()
                //    //{
                //    //    SecurityName = x.SecurityName,
                //    //    StockID = x.StockID,
                //    //    AddedDateString = x.AddedDate.ToString(),
                //    //    LastTradedPrice = x.LastTradedPrice,
                //    //    AddedDate = x.AddedDate


                //    //})
                //    //    .OrderBy(a => a.AddedDate)
                //    //    .ToList();
                //}
                //else
                {
                    int daysInMonth = Convert.ToInt32(db.COM_ENGLISH_NEPALI_DATE.Where(x => x.NEPALI_YEAR.ToString() == isYear && x.MONTH_CD.ToString() == isMonth).Select(x => x.NO_OF_DAYS).FirstOrDefault());
                    if (Convert.ToInt32(isMonth) < 10)
                    {
                        isMonth = "0" + isMonth;
                    }

                    string startDateNep = isYear + "-" + isMonth + "-" + "01";
                    string endDateNep = isYear + "-" + isMonth + "-" + daysInMonth;
                    DateTime startDateEng = Convert.ToDateTime(db.PR_neptoeng(nepdate: startDateNep).FirstOrDefault());
                    DateTime endDateEng = Convert.ToDateTime(db.PR_neptoeng(nepdate: endDateNep).FirstOrDefault());

                    List<WorkListModel> WorksList = new List<WorkListModel>();

                    for (int i = 1; i <= daysInMonth; i++)
                    {
                        string nepalidate = isYear + "-" + isMonth + "-" + (i);
                        int? xxx = 0, xxy = 0, xxz = 0, xyx = 0;
                        WorkListModel model = new WorkListModel();
                        model.date_Eng = Convert.ToDateTime(startDateEng.AddDays(i).ToShortDateString());
                        var nextday = Convert.ToDateTime(startDateEng.AddDays(i + 1).ToShortDateString());
                        model.DayofMonth = Convert.ToDateTime(model.date_Eng).DayOfWeek.ToString();
                        // string datess = obj.PR_engtonep(engdate: model.date_Eng.ToString()).FirstOrDefault();
                        model.totalCountAnydesk = db.workList.Where(x => x.workListType == "Anydesk" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.time).Sum();
                        model.totalRows = db.workList.Where(x => x.date_Eng >= model.date_Eng && x.users == isUserId && x.date_Eng < nextday).Select(x => x.workListId).Count();
                        model.totalCountCall = db.workList.Where(x => x.workListType == "Call" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.time).Sum();
                        model.totalCountEmail = db.workList.Where(x => x.workListType == "Email" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.time).Sum();
                        model.mun = db.workList.Where(x => x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.mun).FirstOrDefault();
                        model.workDet = db.workList.Where(x => x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.workDet).FirstOrDefault();
                        model.totalSum = model.totalCountAnydesk + model.totalCountCall + model.totalCountEmail;
                        xxx += model.totalCountEmail ?? 0;
                        xxy += model.totalCountCall ?? 0;
                        xxz += model.totalCountAnydesk ?? 0;
                        xyx += model.totalRows ?? 0;
                        model.totalSum = xxx + xxy + xxz;
                        model.date = nepalidate;
                        model.AddedDateString = Convert.ToDateTime(model.date_Eng).ToString("yyyy-MM-dd");
                        WorksList.Add(model);
                    }
                    var SumScore = WorksList.Select(x => new WorkListModel()
                    {


                        AddedDateString = x.date,
                        totalSum = x.totalSum,
                        AddedDate = Convert.ToDateTime(x.AddedDateString)

                    })
                   .OrderBy(a => a.AddedDate)
                   .ToList();

                    return Json(SumScore, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult Index(FormCollection fc)
        {
            var obj = new workReportEntities();
            string nepMonthstr = "0";
            int nepYear = Convert.ToInt32(fc["fscYear"]);
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
            //DateTime todaysDateNep = DateTime.Parse(obj.PR_engtonep(engdate: todaysDateEng).FirstOrDefault());
            string todaysDateNep = obj.PR_engtonep(engdate: todaysDateEng).FirstOrDefault();
            var datesep = todaysDateNep.Split('-');
            int todaysYear = Convert.ToInt32(datesep[0]);
            int todaysMonth = Convert.ToInt32(datesep[1]);
            int todaysDay = Convert.ToInt32(datesep[2]);
            ViewBag.todaysYear = todaysYear;
            ViewBag.todaysMonth = db.nepMonths.AsNoTracking().Where(x => x.monthId == todaysMonth).Select(x => x.monthName).FirstOrDefault();
            ViewBag.todaysDay = todaysDay;


            nepMonths nepaliMonth = db.nepMonths.AsNoTracking().Where(x => x.monthId == nepmonth).FirstOrDefault();
            ViewBag.nepmonth = nepaliMonth.monthName;

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
            //var list=db.workList.Where(x=>x.date_Eng>= engDate && x.date_Eng<=lastengdate).Select
            List<WorkListModel> WorksList = new List<WorkListModel>();
            int? xxx = 0, xxy = 0, xxz = 0, xyx = 0;
            for (int i = 0; i < daysInMonth; i++)
            {
                WorkListModel model = new WorkListModel();
                model.date_Eng = Convert.ToDateTime(engDate.AddDays(i).ToShortDateString());
                var nextday = Convert.ToDateTime(engDate.AddDays(i + 1).ToShortDateString());
                model.DayofMonth = Convert.ToDateTime(model.date_Eng).DayOfWeek.ToString();
                // string datess = obj.PR_engtonep(engdate: model.date_Eng.ToString()).FirstOrDefault();
                model.totalCountAnydesk = db.workList.Where(x => x.workListType == "Anydesk" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.time).Sum();
                model.totalRows = db.workList.Where(x => x.date_Eng >= model.date_Eng && x.users == isUserId && x.date_Eng < nextday).Select(x => x.workListId).Count();
                model.totalCountCall = db.workList.Where(x => x.workListType == "Call" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.time).Sum();
                model.totalCountEmail = db.workList.Where(x => x.workListType == "Email" && x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.time).Sum();
                model.mun = db.workList.Where(x => x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.mun).FirstOrDefault();
                model.workDet = db.workList.Where(x => x.users == isUserId && x.date_Eng >= model.date_Eng && x.date_Eng < nextday).Select(x => x.workDet).FirstOrDefault();
                xxx += model.totalCountEmail ?? 0;
                xxy += model.totalCountCall ?? 0;
                xxz += model.totalCountAnydesk ?? 0;
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
            DateTime userContractDateNep = Convert.ToDateTime(userdet.contractDate);
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
            DateTime nepDate1 = Convert.ToDateTime((nepYear.ToString() + '-' + nepMonthstr + '-' + 25));

            TimeSpan differ = nepDate1 - enteredDateNep;
            int totalMins = (int)differ.TotalMinutes;
            int totalMonths = Convert.ToInt32(Math.Ceiling((((Convert.ToDouble(totalMins)) / 60) / 24) / 30));
            int prevMonthAmount;
            if (totalMonths <= 1)
            {
                prevMonthAmount = Convert.ToInt32(userdet.lastMonthAmount);
            }
            else
            {
                prevMonthAmount = Convert.ToInt32(userdet.lastMonthAmount) + Convert.ToInt32((totalMonths - 1) * userdet.monthlySalary);
            }
            ViewBag.prevMonthAmount = prevMonthAmount;
            int totalThisMonthAmount = Convert.ToInt32(prevMonthAmount + userdet.monthlySalary);
            ViewBag.totalThisMonthAmount = totalThisMonthAmount;
            int remAmount = Convert.ToInt32(userdet.totalAmount - totalThisMonthAmount);
            ViewBag.remAmount = remAmount;


            if (Convert.ToInt32(Session["userPost"]) == 7 || Convert.ToInt32(Session["userPost"]) == 8)
            {
                return View("Index1", WorksList);
            }
            else
            {

                return View(WorksList);
            }
        }

    }
}