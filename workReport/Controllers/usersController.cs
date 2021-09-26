using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using workReport.Models;

namespace workReport.Controllers
{
    public class usersController : SessionCheckController
    {
        private workReportEntities db = new workReportEntities();

        // GET: users
        public ActionResult Index()
        {
            return View(db.user.ToList());
        }

        public ActionResult ChangePassword()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(FormCollection fc)
        {
            string oldpassword = fc["oldPassword"].ToString().Trim();
            string newpassword = fc["newOldPassword"].ToString().Trim();
            int isUserId = Convert.ToInt32(Session["userId"]);
            user isUser = db.user.Find(isUserId);
            while(isUser.userPassword==oldpassword)
            {
                isUser.userPassword = newpassword;
                db.Entry(isUser).State = EntityState.Modified;
                db.SaveChanges();
                Session.Abandon();
                FormsAuthentication.SignOut();
                return Redirect("/Login/SignIn");

            }
            ViewBag.message = "Something Went Wrong!!";
            return View();
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
     

        [HttpPost]
        public JsonResult CheckUsername(string username)
        {
          
            bool isValid = db.user.ToList().Exists(p => p.userName.Equals(username, StringComparison.CurrentCultureIgnoreCase));
                return Json(isValid);
        }



        // POST: users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
       

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
