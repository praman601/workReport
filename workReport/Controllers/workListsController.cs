using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using workReport.Models;
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

namespace workReport.Controllers
{
    public class workListsController : SessionCheckController
    {
        private workReportEntities db = new workReportEntities();
        // GET: workLists

        public ActionResult Main()
        {

            ViewBag.mun = new SelectList(db.mun, "mun_id", "mun_name");
            ViewBag.work_Types = new SelectList(db.workTypes, "workId", "workName");
            ViewBag.issues = new SelectList(db.issues, "issueId", "issueName");
            int isuser = Convert.ToInt32(Session["userId"]);
            int isUserPost = Convert.ToInt32(Session["userPost"]);
            ViewBag.Postuser = Convert.ToInt32(Session["userPost"]);
            ViewBag.work_Types = new SelectList(db.workTypes, "workId", "workName");
            ViewBag.fscYear = new SelectList(db.repYear, "yearId", "yearName");
            return View();
        }

        public JsonResult addelRepTask(int year, int month,string monthName, int day, int taskType)
        {

            string message = monthName;

            if (taskType == 1)
            {
                //here task1 starts
                int nepYear = Convert.ToInt32(year);
                int nepmonth = Convert.ToInt32(month);
                int nepDay = Convert.ToInt32(day);
                string monthh = (nepmonth <= 9) ? "0" + nepmonth : nepmonth.ToString();
                string dayy = (nepDay <= 9) ? "0" + nepDay : nepDay.ToString();
                string isUserName = Session["userName"].ToString();
                int isUserId = Convert.ToInt32(Session["userId"]);
                Random rand = new Random();

                if (nepDay == 0)
                {


                    int daysInMonth = Convert.ToInt32(db.COM_ENGLISH_NEPALI_DATE.AsNoTracking().Where(x => x.NEPALI_YEAR == nepYear && x.MONTH_CD == nepmonth).Select(x => x.NO_OF_DAYS).FirstOrDefault());
                    for (int i = 1; i <= daysInMonth; i++)
                    {

                        List<WorkListModel> worksList = new List<WorkListModel>();
                        string p = (i <= 9) ? "0" + i : i.ToString();
                        string nepDate = nepYear + "-" + monthh + "-" + p;
                        bool hasHoliday = db.workList.AsNoTracking().Where(h => (h.workDet == "Public Holiday" || h.workDet == "Leave" || h.workDet == "TravelOrder") && h.date == nepDate && h.users == isUserId).Any();
                        var ssre = hasHoliday;
                        DateTime engDate = Convert.ToDateTime(db.PR_neptoeng(nepdate: nepDate).FirstOrDefault());
                        string x = engDate.ToString("yyyy-MM-dd");
                        int dailyWorkListCount = rand.Next(7, 15);
                        if (Convert.ToDateTime(x).DayOfWeek.ToString() == "Friday")
                        {
                            dailyWorkListCount = rand.Next(4, 7);
                        }
                        int CallRecord = Convert.ToInt32(dailyWorkListCount * 0.7);
                        int test = Convert.ToInt32(CallRecord * 0.4);
                        int Anydesk = Convert.ToInt32(dailyWorkListCount * 0.1);
                        if (Convert.ToDateTime(x).DayOfWeek.ToString() != "Saturday")
                        {
                            if (!hasHoliday)
                            {
                                for (int j = 1; j <= CallRecord; j++)
                                {


                                    WorkListModel workList = new WorkListModel();
                                    workList.date = nepDate;
                                    workList.date_Eng = Convert.ToDateTime(x);
                                    workList.workListType = "Call";
                                    workList.issue = GetRandomIssue();

                                    workList.mun = db.mun.OrderBy(s => Guid.NewGuid()).Select(s => s.mun_name).FirstOrDefault();
                                    workList.time = rand.Next(3, 12);
                                    workList.workDet = "Null";
                                    workList.users = Convert.ToInt32(Session["userId"]);

                                    worksList.Add(workList);
                                }
                            }
                        }

                        List<WorkListModel> RandomList = new List<WorkListModel>();
                        RandomList = worksList.Take(test).ToList();
                        List<WorkListModel> AddLists = new List<WorkListModel>();

                        foreach (var item in RandomList)
                        {
                            WorkListModel andlist = new WorkListModel();
                            andlist.date = item.date;
                            andlist.date_Eng = item.date_Eng;

                            andlist.issue = item.issue;

                            andlist.mun = item.mun;
                            andlist.time = item.time;
                            andlist.workDet = "Null";
                            andlist.users = Convert.ToInt32(Session["userId"]);
                            andlist.workListType = "Anydesk";

                            worksList.Add(andlist);

                        }
                        worksList = worksList.OrderBy(n => n.mun).ToList();
                        foreach (var item in worksList)
                        {
                            workList wl = new workList();
                            wl.date = item.date;
                            wl.date_Eng = item.date_Eng;

                            wl.issue = item.issue;

                            wl.mun = item.mun;
                            wl.time = item.time;
                            wl.workDet = item.workDet;
                            wl.users = item.users;
                            wl.workListType = item.workListType;
                            db.workList.Add(wl);
                            db.SaveChanges();
                        }
                        WorkListModel test1 = worksList.LastOrDefault();
                        workList w2 = new workList();
                        if (test1 != null)
                        {


                            w2.date = test1.date;
                            w2.date_Eng = test1.date_Eng;

                            w2.issue = "OTHERS";

                            w2.mun = "";
                            Random rrr = new Random();
                            w2.time = rrr.Next(5, 13);
                            w2.workDet = "";
                            w2.users = test1.users;
                            w2.workListType = "Email";
                            db.workList.Add(w2);
                            db.SaveChanges();
                        }
                        //foreach (var item in AddLists.OrderBy(t => t.mun))
                        //{
                        //    CP_WORKLIST wl = new CP_WORKLIST();
                        //    wl = item;
                        //    db.CP_WORKLIST.Add(wl);
                        //    db.SaveChanges();
                        //}
                        //for (int j = 1; j <= Anydesk; j++)
                        //{
                        //    CP_WORKLIST workList = new CP_WORKLIST();
                        //    workList.date = nepDate;
                        //    workList.date_Eng = Convert.ToDateTime(x);
                        //    workList.workListType = "AnyDesk";
                        //    workList.issue = GetRandomIssue();
                        //    workList.mun = db.mun.OrderBy(s => Guid.NewGuid()).Select(s => s.mun_name).FirstOrDefault();
                        //    workList.time = rand.Next(3, 12);
                        //    workList.workDet = "Null";
                        //    workList.users = Convert.ToInt32(Session["userId"]);
                        //    worksList.Add(workList);
                        //}



                    }
                    //var munList = db.mun.Select(x => x.mun_name).Distinct().OrderByDescending(x => x.Length).ToList();
                    //int index = rand.Next(munList.Count);
                    //string randomMun = munList[index];
                    //int time = rand.Next(2, 10);
                    message = message + " Worklist Created";
                    return Json(new
                    {
                        message = message,
                        isRedirect = false
                    });
                }
                else
                {
                    for (int i = nepDay; i <= nepDay; i++)
                    {

                        List<WorkListModel> worksList = new List<WorkListModel>();
                        string p = (i <= 9) ? "0" + i : i.ToString();
                        string nepDate = nepYear + "-" + monthh + "-" + p;
                        bool hasHoliday = db.workList.AsNoTracking().Where(h => (h.workDet == "Public Holiday" || h.workDet == "Leave" || h.workDet == "TravelOrder") && h.date == nepDate && h.users == isUserId).Any();
                        var ssre = hasHoliday;
                        DateTime engDate = Convert.ToDateTime(db.PR_neptoeng(nepdate: nepDate).FirstOrDefault());
                        string x = engDate.ToString("yyyy-MM-dd");
                        int dailyWorkListCount = rand.Next(7, 16);
                        if (Convert.ToDateTime(x).DayOfWeek.ToString() == "Friday")
                        {
                            dailyWorkListCount = rand.Next(4, 7);
                        }
                        int CallRecord = Convert.ToInt32(dailyWorkListCount * 0.7);
                        int test = Convert.ToInt32(CallRecord * 0.4);
                        int Anydesk = Convert.ToInt32(dailyWorkListCount * 0.1);
                        if (Convert.ToDateTime(x).DayOfWeek.ToString() != "Saturday")
                        {
                            if (!hasHoliday)
                            {
                                for (int j = 1; j <= CallRecord; j++)
                                {


                                    WorkListModel workList = new WorkListModel();
                                    workList.date = nepDate;
                                    workList.date_Eng = Convert.ToDateTime(x);
                                    workList.workListType = "Call";
                                    workList.issue = GetRandomIssue();

                                    workList.mun = db.mun.OrderBy(s => Guid.NewGuid()).Select(s => s.mun_name).FirstOrDefault();
                                    workList.time = rand.Next(3, 10);
                                    workList.workDet = "Null";
                                    workList.users = Convert.ToInt32(Session["userId"]);

                                    worksList.Add(workList);
                                }
                            }
                        }

                        List<WorkListModel> RandomList = new List<WorkListModel>();
                        RandomList = worksList.Take(test).ToList();
                        List<WorkListModel> AddLists = new List<WorkListModel>();

                        foreach (var item in RandomList)
                        {
                            WorkListModel andlist = new WorkListModel();
                            andlist.date = item.date;
                            andlist.date_Eng = item.date_Eng;

                            andlist.issue = item.issue;

                            andlist.mun = item.mun;
                            andlist.time = Convert.ToInt32(item.time * 0.7);
                            andlist.workDet = "Null";
                            andlist.users = Convert.ToInt32(Session["userId"]);
                            andlist.workListType = "Anydesk";

                            worksList.Add(andlist);

                        }
                        worksList = worksList.OrderBy(n => n.mun).ToList();

                        foreach (var item in worksList)
                        {
                            workList wl = new workList();
                            wl.date = item.date;
                            wl.date_Eng = item.date_Eng;

                            wl.issue = item.issue;

                            wl.mun = item.mun;
                            wl.time = item.time;
                            wl.workDet = item.workDet;
                            wl.users = item.users;
                            wl.workListType = item.workListType;
                            db.workList.Add(wl);
                            db.SaveChanges();
                        }
                        WorkListModel test1 = worksList.LastOrDefault();
                        workList w2 = new workList();
                        if (test1 != null)
                        {


                            w2.date = test1.date;
                            w2.date_Eng = test1.date_Eng;

                            w2.issue = "OTHERS";

                            w2.mun = "";
                            Random rrr = new Random();
                            w2.time = rrr.Next(5, 13);
                            w2.workDet = "";
                            w2.users = test1.users;
                            w2.workListType = "Email";
                            db.workList.Add(w2);
                            db.SaveChanges();
                        }

                        //foreach (var item in AddLists.OrderBy(t => t.mun))
                        //{
                        //    CP_WORKLIST wl = new CP_WORKLIST();
                        //    wl = item;
                        //    db.CP_WORKLIST.Add(wl);
                        //    db.SaveChanges();
                        //}
                        //for (int j = 1; j <= Anydesk; j++)
                        //{
                        //    CP_WORKLIST workList = new CP_WORKLIST();
                        //    workList.date = nepDate;
                        //    workList.date_Eng = Convert.ToDateTime(x);
                        //    workList.workListType = "AnyDesk";
                        //    workList.issue = GetRandomIssue();
                        //    workList.mun = db.mun.OrderBy(s => Guid.NewGuid()).Select(s => s.mun_name).FirstOrDefault();
                        //    workList.time = rand.Next(3, 12);
                        //    workList.workDet = "Null";
                        //    workList.users = Convert.ToInt32(Session["userId"]);
                        //    worksList.Add(workList);
                        //}



                    }
                    message = message +" "+day +" worklist Created";
                    return Json(new
                    {
                        message = message,
                        isRedirect = false
                    });
                }

                //here task 1 ends





                

               
            }
             else if(taskType==2)
            {
                string nepYear = year.ToString();
                int nepmonth = month;
                int nepDay = day;
                int operation = (nepDay == 0) ? 0 : 1;

                string monthh = (nepmonth <= 9) ? "0" + nepmonth : nepmonth.ToString();
                string dayy = (nepDay <= 9 || nepDay > 0) ? "0" + nepDay : nepDay.ToString();
                string isUserName = Session["userName"].ToString();
                int isUserId = Convert.ToInt32(Session["userId"]);
                var check = db.deleteWorkList(isUserId, operation, nepYear, monthh, dayy);
                if(operation==0)
                {
                    message = message  + " worklist Deleted";
                    return Json(new
                    {
                        message =message,
                        isRedirect = false
                    });

                   
                }
                else
                {
                    message = message + " " + day + " worklist Deleted";
                    return Json(new
                    {
                        message = message,
                        isRedirect = false
                    });
                }
              
            }
            else if(taskType==3)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Reports", new { yearcheck = year, monthcheck = month }),
                    isRedirect = true
                });
            }
            else
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Main", "workLists"),
                    isRedirect = true
                });
            }


        }



        public ActionResult RangeList(string StartDatee, string EndDatee, int? i)
        {
            DateTime todaysDate = DateTime.Now.Date;
            int isuser = Convert.ToInt32(Session["userId"]);

            ViewBag.StartDatee = StartDatee;
            ViewBag.EndDatee = EndDatee;
            if (ViewBag.StartDatee == null || ViewBag.EndDatee == null)
            {
                ViewBag.StartDatee = todaysDate.ToString("yyyy-MM-dd");
                ViewBag.EndDatee = todaysDate.ToString("yyyy-MM-dd");
                var worklistdata = db.workList.AsNoTracking().Where(x => x.users == isuser && x.date_Eng == todaysDate).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10);
                return View(worklistdata);
            }
            ViewBag.PageNume = i;
            DateTime startingDate = Convert.ToDateTime(StartDatee);
            DateTime endingDate = Convert.ToDateTime(EndDatee);
            ViewBag.PageNume = i;
            //return View(db.workList.AsNoTracking().Where(x => x.users == isuser && x.date_Eng == todaysDate).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
            return View(db.workList.AsNoTracking().Where(x => x.users == isuser && x.date_Eng >= startingDate && x.date_Eng <= endingDate).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));

        }



        public ActionResult RangeListnew(string StartDatee, string EndDatee, int? i)
        {
            DateTime todaysDate = DateTime.Now.Date;
            int isuser = Convert.ToInt32(Session["userId"]);

            ViewBag.StartDatee = StartDatee;
            ViewBag.EndDatee = EndDatee;
            if (ViewBag.StartDatee == null || ViewBag.EndDatee == null)
            {
                ViewBag.StartDatee = todaysDate.ToString("yyyy-MM-dd");
                ViewBag.EndDatee = todaysDate.ToString("yyyy-MM-dd");
                var worklistdata = db.workList.AsNoTracking().Where(x => x.users == isuser && x.date_Eng == todaysDate).OrderByDescending(x => x.workListId).ToList();
                return Json(new { data = worklistdata }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.PageNume = i;
            DateTime startingDate = Convert.ToDateTime(StartDatee);
            DateTime endingDate = Convert.ToDateTime(EndDatee);
            ViewBag.PageNume = i;
            //return View(db.workList.AsNoTracking().Where(x => x.users == isuser && x.date_Eng == todaysDate).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
            var worklistdatanew = db.workList.AsNoTracking().Where(x => x.users == isuser && x.date_Eng >= startingDate && x.date_Eng <= endingDate).OrderByDescending(x => x.workListId).ToList();
            return Json(new { data = worklistdatanew }, JsonRequestBehavior.AllowGet);
        }


        //public ViewResult Indexnew(string sortOrder, string searchString)
        //{

        //    var students = from s in db.Students
        //                   select s;
        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        students = students.Where(s => s.LastName.Contains(searchString)
        //                               || s.FirstMidName.Contains(searchString));
        //    }
        //    switch (sortOrder)
        //    {
        //        case "name_desc":
        //            students = students.OrderByDescending(s => s.LastName);
        //            break;
        //        case "Date":
        //            students = students.OrderBy(s => s.EnrollmentDate);
        //            break;
        //        case "date_desc":
        //            students = students.OrderByDescending(s => s.EnrollmentDate);
        //            break;
        //        default:
        //            students = students.OrderBy(s => s.LastName);
        //            break;
        //    }

        //    return View(students.ToList());
        //}

        //public ActionResult RangeList1(string startDate, string endDate,int? i)
        //{
        //    DateTime startingDate = Convert.ToDateTime(startDate);
        //    DateTime endingDate = Convert.ToDateTime(endDate);
        //    int isuser = Convert.ToInt32(Session["userId"]);
        //    var haswork = db.workList.AsNoTracking().Where(x => x.users == isuser && x.date_Eng >= startingDate && x.date_Eng <= endingDate).OrderByDescending(x => x.workListId).ToList();
        //    ViewBag.StartDate = startDate;
        //    ViewBag.EndDate = endDate; ViewBag.PageNume = i;
        //    return View(haswork.ToPagedList(i ?? 1, 10));
        //    //return View(db.workList.AsNoTracking().Where(x => x.users == isuser && x.date_Eng >= startingDate && x.date_Eng <= endingDate).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
        //}

        public ActionResult Index(int? i)
        {
            if (Session["userId"] != null)
            {
                ViewBag.VisitCount = db.MisC_Data.Find(1).Visit_Count;
                ViewBag.mun = new SelectList(db.mun, "mun_id", "mun_name");
                ViewBag.work_Types = new SelectList(db.workTypes, "workId", "workName");
                ViewBag.issues = new SelectList(db.issues, "issueId", "issueName");
                int isuser = Convert.ToInt32(Session["userId"]);
                int isUserPost = Convert.ToInt32(Session["userPost"]);
                ViewBag.Postuser = Convert.ToInt32(Session["userPost"]);

                if (isUserPost == 7 || isUserPost == 8)
                {
                    return View("Index2", db.workList.AsNoTracking().Where(x => x.users == isuser).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
                }
                else
                {

                    return View(db.workList.AsNoTracking().Where(x => x.users == isuser).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
                }
            }
            else
                return RedirectToAction("SignIn", "Login");
        }



        public ActionResult Index1(int? i)
        {
            int isuser = Convert.ToInt32(Session["userId"]);

            return View(db.workList.AsNoTracking().Where(x => x.users == isuser).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
        }

        public ActionResult Index3(int? i)
        {
            int isuser = Convert.ToInt32(Session["userId"]);

            return View(db.workList.AsNoTracking().Where(x => x.users == isuser).OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
        }
        // GET: workLists/Details/5
        public ActionResult Details(int? id)
        {
            workList work1 = db.workList.Find(id);
            ViewBag.username = db.user.AsNoTracking().Where(x => x.usrId == work1.users).Select(x => x.firstName).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            workList workList = db.workList.Find(id);
            if (workList == null)
            {
                return HttpNotFound();
            }
            return View(workList);
        }


        public ActionResult Create(string workTypeId, string issueTypeId, string munId, int timeId, int userId, string workDate, string workDet)
        {
            int isUserPost = Convert.ToInt32(Session["userPost"]);
            var obj = new workReportEntities();

            string x = obj.PR_engtonep(engdate: workDate).FirstOrDefault();

            workList workList = new workList();
            workList.workListType = workTypeId;
            workList.issue = issueTypeId;
            workList.mun = munId;
            workList.time = timeId;
            workList.date_Eng = Convert.ToDateTime(workDate);
            workList.date = x;
            workList.workDet = workDet;
            workList.users = Convert.ToInt32(Session["userId"]);

            db.workList.Add(workList);
            db.SaveChanges();
            if (isUserPost == 7 || isUserPost == 8)
            {
                return RedirectToAction("Index3");
            }
            else
            {
                return RedirectToAction("Index1");
            }


        }


        public ActionResult createRandomWorkList()
        {
            ViewBag.NepYears = new SelectList(db.COM_ENGLISH_NEPALI_DATE.Select(x => x.NEPALI_YEAR).Distinct().OrderByDescending(x => x.Value), "NEPALI_YEAR");

            return View();
        }

        public ActionResult createRandomWorkListPost(FormCollection fc)
        {
            int nepYear = Convert.ToInt32(fc["NepYears"]);
            int nepmonth = Convert.ToInt32(fc["months"]);
            int nepDay = Convert.ToInt32(fc["days"]);
            string monthh = (nepmonth <= 9) ? "0" + nepmonth : nepmonth.ToString();
            string dayy = (nepDay <= 9) ? "0" + nepDay : nepDay.ToString();
            string isUserName = Session["userName"].ToString();
            int isUserId = Convert.ToInt32(Session["userId"]);
            Random rand = new Random();

            if (nepDay == 0)
            {


                int daysInMonth = Convert.ToInt32(db.COM_ENGLISH_NEPALI_DATE.AsNoTracking().Where(x => x.NEPALI_YEAR == nepYear && x.MONTH_CD == nepmonth).Select(x => x.NO_OF_DAYS).FirstOrDefault());
                for (int i = 1; i <= daysInMonth; i++)
                {

                    List<WorkListModel> worksList = new List<WorkListModel>();
                    string p = (i <= 9) ? "0" + i : i.ToString();
                    string nepDate = nepYear + "-" + monthh + "-" + p;
                    bool hasHoliday = db.workList.AsNoTracking().Where(h => (h.workDet == "Public Holiday" || h.workDet == "Leave" || h.workDet== "TravelOrder") && h.date == nepDate && h.users == isUserId).Any();
                    var ssre = hasHoliday;
                    DateTime engDate = Convert.ToDateTime(db.PR_neptoeng(nepdate: nepDate).FirstOrDefault());
                    string x = engDate.ToString("yyyy-MM-dd");
                    int dailyWorkListCount = rand.Next(7, 15);
                    if (Convert.ToDateTime(x).DayOfWeek.ToString() == "Friday")
                    {
                        dailyWorkListCount = rand.Next(4, 7);
                    }
                    int CallRecord = Convert.ToInt32(dailyWorkListCount * 0.7);
                    int test = Convert.ToInt32(CallRecord * 0.4);
                    int Anydesk = Convert.ToInt32(dailyWorkListCount * 0.1);
                    if (Convert.ToDateTime(x).DayOfWeek.ToString() != "Saturday")
                    {
                        if (!hasHoliday)
                        {
                            for (int j = 1; j <= CallRecord; j++)
                            {


                                WorkListModel workList = new WorkListModel();
                                workList.date = nepDate;
                                workList.date_Eng = Convert.ToDateTime(x);
                                workList.workListType = "Call";
                                workList.issue = GetRandomIssue();
                                
                                workList.mun = db.mun.OrderBy(s => Guid.NewGuid()).Select(s => s.mun_name).FirstOrDefault();
                                workList.time = rand.Next(3, 12);
                                workList.workDet = "Null";
                                workList.users = Convert.ToInt32(Session["userId"]);

                                worksList.Add(workList);
                            }
                        }
                    }

                    List<WorkListModel> RandomList = new List<WorkListModel>();
                    RandomList = worksList.Take(test).ToList();
                    List<WorkListModel> AddLists = new List<WorkListModel>();

                    foreach (var item in RandomList)
                    {
                        WorkListModel andlist = new WorkListModel();
                        andlist.date = item.date;
                        andlist.date_Eng = item.date_Eng;

                        andlist.issue = item.issue;
                        
                        andlist.mun = item.mun;
                        andlist.time = item.time;
                        andlist.workDet = "Null";
                        andlist.users = Convert.ToInt32(Session["userId"]);
                        andlist.workListType = "Anydesk";

                        worksList.Add(andlist);

                    }
                    worksList = worksList.OrderBy(n => n.mun).ToList();
                    foreach (var item in worksList)
                    {
                        workList wl = new workList();
                        wl.date = item.date;
                        wl.date_Eng = item.date_Eng;
                        
                        wl.issue = item.issue;

                        wl.mun = item.mun;
                        wl.time = item.time;
                        wl.workDet = item.workDet;
                        wl.users = item.users;
                        wl.workListType = item.workListType;
                        db.workList.Add(wl);
                        db.SaveChanges();
                    }
                    WorkListModel test1 = worksList.LastOrDefault();
                    workList w2 = new workList();
                    if (test1 != null)
                    {

                 
                    w2.date = test1.date;
                    w2.date_Eng = test1.date_Eng;

                    w2.issue = "OTHERS";

                    w2.mun = "";
                        Random rrr = new Random();
                        w2.time = rrr.Next(5, 13);
                    w2.workDet = "";
                    w2.users = test1.users;
                    w2.workListType = "Email";
                    db.workList.Add(w2);
                    db.SaveChanges();
                    }
                    //foreach (var item in AddLists.OrderBy(t => t.mun))
                    //{
                    //    CP_WORKLIST wl = new CP_WORKLIST();
                    //    wl = item;
                    //    db.CP_WORKLIST.Add(wl);
                    //    db.SaveChanges();
                    //}
                    //for (int j = 1; j <= Anydesk; j++)
                    //{
                    //    CP_WORKLIST workList = new CP_WORKLIST();
                    //    workList.date = nepDate;
                    //    workList.date_Eng = Convert.ToDateTime(x);
                    //    workList.workListType = "AnyDesk";
                    //    workList.issue = GetRandomIssue();
                    //    workList.mun = db.mun.OrderBy(s => Guid.NewGuid()).Select(s => s.mun_name).FirstOrDefault();
                    //    workList.time = rand.Next(3, 12);
                    //    workList.workDet = "Null";
                    //    workList.users = Convert.ToInt32(Session["userId"]);
                    //    worksList.Add(workList);
                    //}



                }
                //var munList = db.mun.Select(x => x.mun_name).Distinct().OrderByDescending(x => x.Length).ToList();
                //int index = rand.Next(munList.Count);
                //string randomMun = munList[index];
                //int time = rand.Next(2, 10);
                return RedirectToAction("Index");
            }
            else
            {
                for (int i = nepDay; i <= nepDay; i++)
                {

                    List<WorkListModel> worksList = new List<WorkListModel>();
                    string p = (i <= 9) ? "0" + i : i.ToString();
                    string nepDate = nepYear + "-" + monthh + "-" + p;
                    bool hasHoliday = db.workList.AsNoTracking().Where(h => (h.workDet == "Public Holiday" || h.workDet == "Leave" || h.workDet== "TravelOrder") && h.date == nepDate && h.users == isUserId).Any();
                    var ssre = hasHoliday;
                    DateTime engDate = Convert.ToDateTime(db.PR_neptoeng(nepdate: nepDate).FirstOrDefault());
                    string x = engDate.ToString("yyyy-MM-dd");
                    int dailyWorkListCount = rand.Next(7,16);
                    if (Convert.ToDateTime(x).DayOfWeek.ToString() == "Friday")
                    {
                        dailyWorkListCount = rand.Next(4, 7);
                    }
                    int CallRecord = Convert.ToInt32(dailyWorkListCount * 0.7);
                    int test = Convert.ToInt32(CallRecord * 0.4);
                    int Anydesk = Convert.ToInt32(dailyWorkListCount * 0.1);
                    if (Convert.ToDateTime(x).DayOfWeek.ToString() != "Saturday")
                    {
                        if (!hasHoliday)
                        {
                            for (int j = 1; j <= CallRecord; j++)
                            {


                                WorkListModel workList = new WorkListModel();
                                workList.date = nepDate;
                                workList.date_Eng = Convert.ToDateTime(x);
                                workList.workListType = "Call";
                                workList.issue = GetRandomIssue();

                                workList.mun = db.mun.OrderBy(s => Guid.NewGuid()).Select(s => s.mun_name).FirstOrDefault();
                                workList.time = rand.Next(3, 10);
                                workList.workDet = "Null";
                                workList.users = Convert.ToInt32(Session["userId"]);

                                worksList.Add(workList);
                            }
                        }
                    }

                    List<WorkListModel> RandomList = new List<WorkListModel>();
                    RandomList = worksList.Take(test).ToList();
                    List<WorkListModel> AddLists = new List<WorkListModel>();

                    foreach (var item in RandomList)
                    {
                        WorkListModel andlist = new WorkListModel();
                        andlist.date = item.date;
                        andlist.date_Eng = item.date_Eng;

                        andlist.issue = item.issue;

                        andlist.mun = item.mun;
                        andlist.time =Convert.ToInt32( item.time * 0.7);
                        andlist.workDet = "Null";
                        andlist.users = Convert.ToInt32(Session["userId"]);
                        andlist.workListType = "Anydesk";

                        worksList.Add(andlist);

                    }
                    worksList = worksList.OrderBy(n => n.mun).ToList();
                    
                    foreach (var item in worksList)
                    {
                        workList wl = new workList();
                        wl.date = item.date;
                        wl.date_Eng = item.date_Eng;

                        wl.issue = item.issue;

                        wl.mun = item.mun;
                        wl.time = item.time;
                        wl.workDet = item.workDet;
                        wl.users = item.users;
                        wl.workListType = item.workListType;
                        db.workList.Add(wl);
                        db.SaveChanges();
                    }
                    WorkListModel test1 = worksList.LastOrDefault();
                    workList w2 = new workList();
                    if (test1 != null)
                    {


                        w2.date = test1.date;
                        w2.date_Eng = test1.date_Eng;

                        w2.issue = "OTHERS";

                        w2.mun = "";
                        Random rrr = new Random();
                        w2.time = rrr.Next(5, 13);
                        w2.workDet = "";
                        w2.users = test1.users;
                        w2.workListType = "Email";
                        db.workList.Add(w2);
                        db.SaveChanges();
                    }

                    //foreach (var item in AddLists.OrderBy(t => t.mun))
                    //{
                    //    CP_WORKLIST wl = new CP_WORKLIST();
                    //    wl = item;
                    //    db.CP_WORKLIST.Add(wl);
                    //    db.SaveChanges();
                    //}
                    //for (int j = 1; j <= Anydesk; j++)
                    //{
                    //    CP_WORKLIST workList = new CP_WORKLIST();
                    //    workList.date = nepDate;
                    //    workList.date_Eng = Convert.ToDateTime(x);
                    //    workList.workListType = "AnyDesk";
                    //    workList.issue = GetRandomIssue();
                    //    workList.mun = db.mun.OrderBy(s => Guid.NewGuid()).Select(s => s.mun_name).FirstOrDefault();
                    //    workList.time = rand.Next(3, 12);
                    //    workList.workDet = "Null";
                    //    workList.users = Convert.ToInt32(Session["userId"]);
                    //    worksList.Add(workList);
                    //}



                }
                return RedirectToAction("Index");
            }
        }
        public string GetRandomIssue()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            var abc = 0;
            abc = rand.Next(1, 100);
            if (abc <= 40)
            {
                return "Vital";
            }
            else if (abc > 40 && abc <= 70)
            {
                return "Social Security";
            }
            else if (abc > 70 && abc <= 80)
            {
                return "System";
            }
            else if (abc > 80 && abc <= 90)
            {
                return "Bank";
            }
            else if (abc > 90 && abc <= 93)
            {
                return "GMS";
            }
            else if (abc > 93 && abc <= 96)
            {
                return "PTS";
            }
            else if (abc > 96 && abc <= 99)
            {
                return "Digitization";
            }
            else
            {
                return "OTHERS";
            }
        }

        public ActionResult deleteWorkDataGet()
        {
            ViewBag.NepYears = new SelectList(db.COM_ENGLISH_NEPALI_DATE.Select(x => x.NEPALI_YEAR).Distinct().OrderByDescending(x => x.Value), "NEPALI_YEAR");

            return View();
        }


        public ActionResult deleteWorkDataPost(FormCollection fc)
        {
            string nepYear = fc["NepYears"];
            int nepmonth =Convert.ToInt32( fc["months"]);
            int nepDay = Convert.ToInt32(fc["days"]);
            int operation = (nepDay==0) ?  0: 1;
          
            string monthh = (nepmonth <= 9) ? "0" + nepmonth : nepmonth.ToString();
            string dayy = (nepDay <= 9 || nepDay > 0) ? "0" + nepDay : nepDay.ToString();
            string isUserName = Session["userName"].ToString();
            int isUserId = Convert.ToInt32(Session["userId"]);



           var check = db.deleteWorkList(isUserId, operation, nepYear, monthh, dayy);
         
           
            //if(nepDay==0)
            //{
            //    string insertBefore = "INSERT INTO JN_WORKLIST SELECT * FROM workList where date like'" + nepYear + "-" + monthh + "-%' and users=" + isUserId+ " and workDet <> 'leave'";
            //    string deleteQuery = "delete FROM workList where date like'" + nepYear + "-" + monthh + "-%' and users=" + isUserId + " and workDet <> 'leave'";
            //    var check = db.Database.ExecuteSqlCommand(insertBefore);
            //    if (check > 0)
            //    {
            //        db.Database.ExecuteSqlCommand(deleteQuery);
            //    }

            //}
            //else
            //{
            //    string insertBefore = "INSERT INTO JN_WORKLIST SELECT * FROM workList where date ='" + nepYear + "-" + monthh + "-" + dayy + "' and users=" + isUserId;
            //    string deleteQuery= "delete FROM workList where date ='" + nepYear + "-" + monthh + "-" + dayy + "' and users=" + isUserId;
            //    var check = db.Database.ExecuteSqlCommand(insertBefore);
            //    if(check>0)
            //    {
            //        db.Database.ExecuteSqlCommand(deleteQuery); 
            //    }

            //}


            return RedirectToAction("deleteWorkDataGet");
        }


        public ActionResult addHoliday(int? i)
        {
            int isuser = Convert.ToInt32(Session["userId"]);
            ViewBag.date = db.PR_engtonep(DateTime.Now.ToString("yyyy-MM-dd"));
            ViewBag.date1 = db.PR_engtonep(DateTime.Now.ToString("yyyy-MM-dd"));
            return View(db.workList.AsNoTracking().Where(x => x.users == isuser && x.mun == "" && x.workListType == "").OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
        }
        public ActionResult addHoliday1(int? i)
        {
            int isuser = Convert.ToInt32(Session["userId"]);
            return View(db.workList.AsNoTracking().Where(x => x.users == isuser && x.mun == "" && x.workListType == "").OrderByDescending(x => x.workListId).ToList().ToPagedList(i ?? 1, 10));
        }


        public ActionResult CreateHoliday(string workTypeId, string issueTypeId, string munId, int? timeId, int userId, string workDate, string workDate1,string workDet)
        {
            int isUserPost = Convert.ToInt32(Session["userPost"]);
            var obj = new workReportEntities();
            DateTime startDate = Convert.ToDateTime(obj.PR_neptoeng(nepdate: workDate).FirstOrDefault());
            DateTime endDate = Convert.ToDateTime(obj.PR_neptoeng(nepdate: workDate1).FirstOrDefault());
            
            if(endDate>=startDate)
            {
                for (DateTime date = startDate; startDate <= endDate; startDate.AddDays(1))
                {
                    string x = obj.PR_engtonep(engdate: startDate.ToString("yyyy-MM-dd")).FirstOrDefault().ToString();
                    workList workList = new workList();
                    workList.workListType = workTypeId;
                    workList.issue = issueTypeId;
                    workList.mun = munId;
                    workList.time = timeId;
                    workList.date_Eng = startDate;
                    workList.date = x;
                    workList.workDet = workDet;
                    workList.users = Convert.ToInt32(Session["userId"]);

                    db.workList.Add(workList);
                    db.SaveChanges();
                  startDate=  startDate.AddDays(1);
                }
                return RedirectToAction("addHoliday1");
            }
            else
            {
                
                return RedirectToAction("addHoliday");
            }

           



        }
        public class ListtoDataTable
        {
            public DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties by using reflection   
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names  
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {

                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }

                return dataTable;
            }
        }

        public void ExportToExcel(int? nepMonth, int? nepYear)
        {
            var gv = new GridView();
            List<workList> DATAALISTS = WorkListExportData(nepMonth, nepYear).Select(x => new workList()
            {
                workListType = x.workListType,
                mun = x.mun,
                time = x.time,
                users = x.users,
                date = x.date,
                issue = x.issue,
                date_Eng = x.date_Eng,
                workDet = x.workDet
            }).ToList();

            List<workListone> workslist = new List<workListone>();
            foreach (var x in DATAALISTS)
            {
                workListone workone = new workListone();
                workone.workListType = x.workListType;
                workone.mun = x.mun;
                workone.time = x.time;
                workone.users = x.users;
                workone.date = x.date;
                workone.issue = x.issue;
                //DateTime engdate=Convert.ToDateTime( x.date_Eng);
                //workone.date_Eng = engdate.ToString("yyyy-MM-dd");
                workone.workDet = x.workDet;
                workslist.Add(workone);

            }



            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            workSheet.Cells[1, 1, 1, 7].Style.Font.Bold = true;
            workSheet.Cells[1, 1, 1, 7].Text.ToUpper();

            var modelCells = workSheet.Cells["A1"];
            var modelRows = workslist.Count() + 1;
            string modelRange = "A1:G" + modelRows.ToString();
            var modelTable = workSheet.Cells[modelRange];
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            modelCells.LoadFromCollection(Collection: workslist, PrintHeaders: true);
            modelTable.AutoFitColumns();


            //optional use this to make all columms just a bit wider, text would sometimes still overflow after AutoFitColumns().
            //for (int col = 1; col <= modelTable.End.Column; col++)
            //{
            //    workSheet.Column(col).Width = workSheet.Column(col).Width + 25;
            //}

            //modelTable.Cells[1, 1].LoadFromCollection(DATAALISTS, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //here i have set filname as Students.xlsx
                Response.AddHeader("content-disposition", "attachment;  filename=Worklist.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }


            //gv.DataSource = DATAALISTS;
            //gv.DataBind();

            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            //Response.ContentType = "application/ms-excel";

            //ListtoDataTable lsttodt = new ListtoDataTable();
            //DataTable dt = lsttodt.ToDataTable(DATAALISTS); string filesavename = "";
            //string sheetname = "Sheet1"; string filepath="";
            //string dayss = DateTime.Now.Month.ToString();
            //try
            //{
            //    XLWorkbook workbook = new XLWorkbook();

            //    workbook.Worksheets.Add(dt, sheetname);

            //    workbook.Worksheet(sheetname).Rows(1, 1).Style.Protection.SetLocked(true); for (int ii = 1; ii <= 10; ii++)
            //    {
            //        workbook.Worksheet(sheetname).Columns(ii, ii).Width = 25;

            //    }
            //    var filesss = Path.GetFileNameWithoutExtension(dayss);

            //    string dir = HttpContext.Server.MapPath("~/files/");

            //    if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
            //    try
            //    {
            //        filepath = Server.MapPath("~/files/") ;
            //    }
            //    catch
            //    {

            //    }
            //     filesavename = DateTime.Now.ToString("yyyy-mm-dd HH-MM-ss");
            //    workbook.SaveAs(string.Format("{0}{1}.xlsx", filepath, filesavename));
            //    //workbook.SaveAs(string.Format("{0}{1}.xlsx",  DateTime.Now.ToString("yyyy-mm-dd HH-ss")));


            //}
            //catch(Exception ex)
            //{
            //    var text = ex.Message; 
            //}

            //Response.Charset = "";
            //StringWriter objStringWriter = new StringWriter();
            //HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            //gv.RenderControl(objHtmlTextWriter);

            //Response.Output.Write(objStringWriter.ToString());
            //Response.Flush();
            //Response.End();
            //string savingname = filepath + filesavename;
            //try
            //{

            //    using (var client = new WebClient())
            //    {
            //        client.DownloadFile(filepath, filesavename);
            //    }
            //    return File(savingname, "application/msexcel", filesavename + ".xlsx");
            //}
            //catch (Exception ex)
            //{
            //    var test = ex.Message;

            //    return File(savingname, "application/msexcel", filesavename + ".xlsx");
            //}
        }








        private object ToDataTable<T>(object p)
        {
            throw new NotImplementedException();
        }

        public ActionResult ImportExcel()
        {
            return View();
        }

        [HttpPost]
        // public async Task<List<workList>> Import(HttpPostedFileBase file)
        //{
        //     var list = new List<workList>();
        //     using (Stream stream = file.InputStream)
        //     {
        //         if (file!=null)
        //         {


        //         var filename = file.FileName;


        //             byte[] data;

        //                 MemoryStream memoryStream = stream as MemoryStream;
        //                 if (memoryStream == null)
        //                 {
        //                     memoryStream = new MemoryStream();
        //                     stream.CopyTo(memoryStream);
        //                 }
        //                 data = memoryStream.ToArray();

        //             //IFormFile filet =(IFormFile)file;
        //             //await filet.CopyToAsync(stream);
        //         }
        //         // await file.CopyToAsync(stream);
        //         using (var package= new ExcelPackage(stream))
        //         {
        //             ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //             var rowcount = worksheet.Dimension.Rows;
        //             for(int row=2;row<=rowcount;row++)
        //             {
        //                 list.Add(new workList
        //                 {
        //                     workListType=worksheet.Cells[row,1].Value.ToString().Trim(),
        //                     mun=worksheet.Cells[row,2].Value.ToString().Trim(),
        //                    time=Convert.ToInt32( worksheet.Cells[row, 3].Value.ToString().Trim()),
        //                    users=Convert.ToInt32( worksheet.Cells[row, 4].Value.ToString().Trim()),
        //                    date= worksheet.Cells[row, 5].Value.ToString().Trim(),
        //                    issue= worksheet.Cells[row, 6].Value.ToString().Trim(),
        //                    date_Eng=Convert.ToDateTime( worksheet.Cells[row, 7].Value.ToString().Trim()),
        //                    workDet= worksheet.Cells[row, 8].Value.ToString().Trim(),
        //                 });
        //             }
        //         }
        //     }
        //     return list;
        // }

        public ActionResult Import(HttpPostedFileBase file)
        {
            var obj = new workReportEntities();
            if (file != null)
            {
                string filepath;
                //saving file
                if (file != null && file.ContentLength > 0)
                {
                    string dir = HttpContext.Server.MapPath("~/files/");
                    if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                    try
                    {
                        filepath = Server.MapPath("~/files/upload") + Path.GetFileName((file.FileName).Trim());
                    }
                    catch
                    {
                        filepath = Server.MapPath("~/files/upload") + Path.GetFileName(file.FileName);
                    }
                    file.SaveAs(filepath);

                    string extension = "";
                    string connString = ""; string sheetname = "Sheet1";
                    if (System.IO.File.Exists(filepath))
                    {

                        //  DataTable dtExcel;
                        if (filepath != "")
                        {
                            if (filepath.Contains("."))
                            {
                                extension = Path.GetExtension(filepath);
                            }
                            if (extension == ".xls")
                            {
                                connString = "provider=Microsoft.Jet.OLEDB.4.0;" + @"data source=" + filepath + ";" + "Extended Properties=Excel 8.0;";
                            }
                            else
                            {
                                connString = "provider=Microsoft.ACE.OLEDB.12.0;" + @"data source=" + filepath + ";" + "Extended Properties=Excel 12.0;";
                            }
                            OleDbConnection oledbConn = new OleDbConnection(connString);
                            try
                            {
                                DataTable dtExcelnew = new DataTable();

                                oledbConn.Open();
                                string query = string.Format("SELECT * FROM [{0}$]", sheetname);
                                using (OleDbDataAdapter myCommand = new OleDbDataAdapter(query, oledbConn))
                                {
                                    myCommand.Fill(dtExcelnew);
                                    oledbConn.Close();
                                }
                                List<workListnew> worklist1 = new List<workListnew>();
                                worklist1 = CommonMethod.ConvertToList<workListnew>(dtExcelnew);
                                foreach (var item in worklist1)
                                {
                                    item.date = item.date.Replace("'", "");

                                }
                                List<workList> DATAALISTS = worklist1.Select(x => new workList()
                                {
                                    date = x.date,

                                    date_Eng = Convert.ToDateTime(obj.PR_neptoeng(nepdate: x.date).FirstOrDefault()),

                                    issue = x.issue,
                                    mun = x.mun,
                                    time = Convert.ToInt32(x.time),
                                    users = Convert.ToInt32(Session["userId"]),
                                    workDet = x.workDet,
                                    workListType = x.workListType

                                }).ToList();

                                foreach (var a in DATAALISTS)
                                {
                                    db.workList.Add(a);
                                    db.SaveChanges();
                                }


                            }
                            catch (Exception ex)
                            {
                                var text = ex.Message; oledbConn.Close();
                            }
                        }


                    }
                }

            }

            return RedirectToAction("Main");
        }


        public static class CommonMethod
        {
            public static List<T> ConvertToList<T>(DataTable dt)
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                var properties = typeof(T).GetProperties();
                var check = dt.AsEnumerable().Select(row => {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                        {
                            try
                            {
                                var a = row[pro.Name];
                                var b = objT;
                                pro.SetValue(objT, row[pro.Name]);
                            }
                            catch (Exception ex)
                            {
                                var text = ex.Message;
                            }
                        }
                    }
                    return objT;
                }).ToList();
                return check;
            }
        }

        public List<WorkListModel> WorkListExportData(int? nepMonths, int? nepYears)
        {
            var obj = new workReportEntities();
            string nepMonthstr = "0";

            int? nepYear = nepYears;
            int? nepmonth = nepMonths;

            ViewBag.months = nepmonth;
            ViewBag.fscYear = nepYear;


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

            for (int i = 0; i < daysInMonth; i++)
            {


                DateTime date_Eng = Convert.ToDateTime(engDate.AddDays(i).ToShortDateString());
                var nextday = Convert.ToDateTime(engDate.AddDays(i + 1).ToShortDateString());
                var model1 = db.workList.AsNoTracking().Where(x => x.users == isUserId && x.date_Eng >= date_Eng && x.date_Eng < nextday).ToList();
                foreach (var obj1 in model1)
                {
                    WorkListModel model = new WorkListModel();
                    model.DayofMonth = Convert.ToDateTime(obj1.date_Eng).DayOfWeek.ToString();
                    model.workListType = obj1.workListType;
                    model.mun = obj1.mun;
                    model.time = obj1.time;
                    model.users = obj1.users;
                    model.date = obj1.date;
                    model.issue = obj1.issue;
                    model.date_Eng = obj1.date_Eng;
                    model.workDet = obj1.workDet;
                    WorksList.Add(model);
                }


            }

            //if (Convert.ToInt32(Session["userPost"]) == 7 || Convert.ToInt32(Session["userPost"]) == 8)
            //{
            //    return View("Index1", WorksList);
            //}
            //else
            //{

            return WorksList;

        }

        public ActionResult workListExport()
        {
            ViewBag.months = new SelectList(db.nepMonths, "monthId", "monthName");
            ViewBag.fscYear = new SelectList(db.repYear, "yearId", "yearName");
            return View();
        }



        public ActionResult WorkListData(FormCollection fc)
        {
            var obj = new workReportEntities();
            string nepMonthstr = "0";

            int nepYear = Convert.ToInt32(fc["fscYear"]);
            int nepmonth = Convert.ToInt32(fc["months"]);

            ViewBag.months = nepmonth;
            ViewBag.fscYear = nepYear;


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


            //  string isUserName = Session["userName"].ToString();
            int isUserId = Convert.ToInt32(Session["userId"]);

            //  var userdet = db.user.AsNoTracking().Where(x => x.usrId == isUserId).FirstOrDefault();
            //  var userPost = db.posts.AsNoTracking().Where(x => x.postId == userdet.post).FirstOrDefault();
            //  ViewBag.userPost = userPost.postName;

            //  ViewBag.isReportMonth = nepaliMonth.monthName;
            //  ViewBag.isReportYear =nepYear;
            //  ViewBag.isUserBank = userdet.bankName;
            //  ViewBag.isBankAdd = userdet.branch;
            //  ViewBag.isAccountNo = userdet.acnumber;
            //  string userfullname = userdet.firstName + " " + userdet.lastName;
            //  ViewBag.reporterName = userfullname;
            ////  int isMonth = Convert.ToInt32(fc["months"]);
            //  ViewBag.isReportDate = nepDate;
            //  ViewBag.isContractAmount = userdet.totalAmount;

            //  ViewBag.isUserSalary = userdet.monthlySalary.ToString();
            var lastengdate = engDate.AddDays(daysInMonth);
            //var list=db.workList.Where(x=>x.date_Eng>= engDate && x.date_Eng<=lastengdate).Select
            List<WorkListModel> WorksList = new List<WorkListModel>();

            for (int i = 0; i < daysInMonth; i++)
            {


                DateTime date_Eng = Convert.ToDateTime(engDate.AddDays(i).ToShortDateString());
                var nextday = Convert.ToDateTime(engDate.AddDays(i + 1).ToShortDateString());
                var model1 = db.workList.AsNoTracking().Where(x => x.users == isUserId && x.date_Eng >= date_Eng && x.date_Eng < nextday).ToList();
                foreach (var obj1 in model1)
                {
                    WorkListModel model = new WorkListModel();
                    model.DayofMonth = Convert.ToDateTime(obj1.date_Eng).DayOfWeek.ToString();
                    model.workListType = obj1.workListType;
                    model.mun = obj1.mun;
                    model.time = obj1.time;
                    model.users = obj1.users;
                    model.date = obj1.date;
                    model.issue = obj1.issue;
                    model.date_Eng = obj1.date_Eng;
                    model.workDet = obj1.workDet;
                    WorksList.Add(model);
                }


            }

            //if (Convert.ToInt32(Session["userPost"]) == 7 || Convert.ToInt32(Session["userPost"]) == 8)
            //{
            //    //return View("Index1", WorksList);


            //    return WorkListData1(WorksList);


            //}
            //else { 

            return View(WorksList);

        }

        public ActionResult WorkListData1(List<WorkListModel> worksList)
        {
            return View(worksList);
        }

        [HttpPost]
        public ActionResult ImportFromExcel(HttpPostedFileBase postedFile)
        {
            if (ModelState.IsValid)
            {
                if (postedFile != null && postedFile.ContentLength > (1024 * 1024 * 50))  // 50MB limit
                {
                    ModelState.AddModelError("postedFile", "Your file is to large. Maximum size allowed is 50MB !");
                }

                else
                {
                    string filePath = string.Empty;
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //For Excel 97-03.
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //For Excel 07 and above.
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }

                    try
                    {
                        DataTable dt = new DataTable();
                        conString = string.Format(conString, filePath);

                        using (OleDbConnection connExcel = new OleDbConnection(conString))
                        {
                            using (OleDbCommand cmdExcel = new OleDbCommand())
                            {
                                using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                {
                                    cmdExcel.Connection = connExcel;

                                    //Get the name of First Sheet.
                                    connExcel.Open();
                                    DataTable dtExcelSchema;
                                    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    connExcel.Close();

                                    //Read Data from First Sheet.
                                    connExcel.Open();
                                    cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                    odaExcel.SelectCommand = cmdExcel;
                                    odaExcel.Fill(dt);
                                    connExcel.Close();
                                }
                            }
                        }

                        conString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                        using (SqlConnection con = new SqlConnection(conString))
                        {
                            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                            {
                                //Set the database table name.
                                sqlBulkCopy.DestinationTableName = "InsuranceCertificate";
                                con.Open();
                                sqlBulkCopy.WriteToServer(dt);
                                con.Close();
                                return Json("File uploaded successfully");
                            }
                        }
                    }

                    //catch (Exception ex)
                    //{
                    //    throw ex;
                    //}
                    catch (Exception e)
                    {
                        return Json("error" + e.Message);
                    }
                    //return RedirectToAction("Index");
                }
            }
            //return View(postedFile);
            return Json("no files were selected !");
        }


        public ActionResult Insert()
        {

            var workList4 = db.workList.AsNoTracking().Where(x => x.date == "2077-09-08").ToList();

            foreach (var b in workList4)
            {
                workList work1 = db.workList.Find(b.workListId);
                work1.date_Eng = DateTime.Now;
                db.Entry(work1).State = EntityState.Modified;
                db.SaveChanges();
            }


            return View();
        }

        public ActionResult Edit(int? id)
        {
            workList workList1 = db.workList.Find(id);
            var mun1 = db.mun.AsNoTracking().Where(x => x.mun_name == workList1.mun).FirstOrDefault();
            var worktype1 = db.workTypes.AsNoTracking().Where(x => x.workName == workList1.workListType).FirstOrDefault();
            int workIssueId = db.issues.AsNoTracking().Where(x => x.issueName == workList1.issue).Select(x => x.issueId).FirstOrDefault();

            int munid = mun1.mun_id;
            int worklistid = worktype1.workId;
            ViewBag.mun = new SelectList(db.mun, "mun_id", "mun_name", munid);
            ViewBag.work_Types = new SelectList(db.workTypes, "workId", "workName", worklistid);
            ViewBag.issue_Types = new SelectList(db.issues, "issueId", "issueName", workIssueId);
            ViewBag.username = db.user.AsNoTracking().Where(x => x.usrId == workList1.users).Select(x => x.firstName).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            workList workList = db.workList.Find(id);
            if (workList == null)
            {
                return HttpNotFound();
            }
            return View(workList);
        }
        // POST: workLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(workList workList, FormCollection fc)
        {
            var mun1 = db.mun.Find(Convert.ToInt32(fc["mun"]));
            workList.mun = mun1.mun_name;
            var workIssuetype = db.issues.Find(Convert.ToInt32(fc["issue_Types"]));
            workList.issue = workIssuetype.issueName;
            workList.users = Convert.ToInt32(Session["userId"]);
            var workListType1 = db.workTypes.Find(Convert.ToInt32(fc["work_Types"]));
            workList.workListType = workListType1.workName;
            db.Entry(workList).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");


        }

        // GET: workLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            workList workList = db.workList.Find(id);
            if (workList == null)
            {
                return HttpNotFound();
            }
            return View(workList);
        }

        // POST: workLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            workList workList = db.workList.Find(id);
            db.workList.Remove(workList);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
