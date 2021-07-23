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
    public class usersController : Controller
    {
        private workReportEntities db = new workReportEntities();

        // GET: users
        public ActionResult Index()
        {
            return View(db.user.ToList());
        }

        //// GET: users/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    user user = db.user.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        // GET: users/Create
        public ActionResult Create()
        {
            ViewBag.isPost = new SelectList(db.posts, "postId", "postName");
            user model = new user();
            return View(model);
        }


        [HttpPost]
        public JsonResult CheckUsername(string username)
        {
          
            bool isValid = db.user.ToList().Exists(p => p.userName.Equals(username, StringComparison.CurrentCultureIgnoreCase));
                return Json(isValid);
        }



        // POST: users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( user user,FormCollection fc)
        {
           
            workReportEntities dbA = new workReportEntities();
             user.post = Convert.ToInt32(fc["isPost"]);
            user.enteredDate =DateTime.Now.ToString("yyyy-MM-dd");
            bool isValid = db.user.ToList().Exists(p => p.userName.Equals(user.userName, StringComparison.CurrentCultureIgnoreCase));
            if(!isValid)
            {
                return View(user);
            }

          else  if (ModelState.IsValid)
            {

                dbA.user.Add(user);
                    dbA.SaveChanges();
                return RedirectToAction("SignIn", "Login");
                
                
            }

            return View(user);
        }

        // GET: users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.user.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "usrId,userName,userPassword,firstName,lastName,post,contractDate,monthlySalary,totalAmount,bankName,branch,acnumber")] user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: users/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    user user = db.user.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        //// POST: users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    user user = db.user.Find(id);
        //    db.user.Remove(user);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
