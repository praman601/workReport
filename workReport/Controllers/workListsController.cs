using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using workReport.Models;
using PagedList.Mvc;
using PagedList;


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

            return View();
        }

        public ActionResult Index(int? i)
        {
            if (Session["userId"] != null)
            {
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


        public ActionResult Create(string workTypeId, string issueTypeId, string munId, int timeId, int userId, string workDate,string workDet)
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
