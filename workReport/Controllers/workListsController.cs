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
        public ActionResult Index(int? i)
        {
            if (Session["userName"] != null)
            {
                ViewBag.mun = new SelectList(db.mun, "mun_id", "mun_name");
                ViewBag.work_Types = new SelectList(db.workTypes, "workId", "workName");
                ViewBag.issues = new SelectList(db.issues, "issueId", "issueName");
                string isuser = Session["userName"].ToString();

                return View(db.workList.AsNoTracking().Where(x=>x.users==isuser).OrderByDescending(x=>x.date).ToList().ToPagedList(i??1,10));
            }
            else
                return RedirectToAction("SignIn", "Login");
        }

        public ActionResult Index1(int? i)
        {
            string isuser = Session["userName"].ToString();

            return View(db.workList.AsNoTracking().Where(x => x.users == isuser).OrderByDescending(x => x.date).ToList().ToPagedList(i ?? 1, 10));
        }
        // GET: workLists/Details/5
        public ActionResult Details(int? id)
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

      
        public ActionResult Create(string workTypeId, string issueTypeId,string munId, int timeId,string userId)
        {

            workList workList = new workList();
            workList.workListType = workTypeId;
            workList.issue = issueTypeId;
            workList.mun = munId;
            workList.time = timeId;
            workList.users = userId;
            workList.date = DateTime.Now;
           workList.users = Session["userName"].ToString();
            
            db.workList.Add(workList);
            db.SaveChanges();
            return RedirectToAction("Index1");



        }


        public ActionResult Edit(int? id)
        {
            workList workList1 = db.workList.Find(id);
            var mun1 = db.mun.AsNoTracking().Where(x => x.mun_name == workList1.mun).FirstOrDefault();
            var worktype1 = db.workTypes.AsNoTracking().Where(x => x.workName == workList1.workListType).FirstOrDefault();
            int munid = mun1.mun_id;
            int worklistid = worktype1.workId;
            ViewBag.mun = new SelectList(db.mun, "mun_id", "mun_name",munid);
            ViewBag.work_Types = new SelectList(db.workTypes, "workId", "workName",worklistid);
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
        public ActionResult Edit([Bind(Include = "workListId,workListType,mun,time,users,date")] workList workList,FormCollection fc)
        {
            var mun1 = db.mun.Find(Convert.ToInt32( fc["mun"]));
            workList.mun = mun1.mun_name;
            var workListType1 = db.workTypes.Find(Convert.ToInt32( fc["work_Types"]));
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
