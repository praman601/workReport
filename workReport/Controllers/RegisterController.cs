using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using workReport.Models;

namespace workReport.Controllers
{
    public class RegisterController : Controller
    {
        private workReportEntities db = new workReportEntities();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(user user, FormCollection fc)
        {
            ViewBag.isPost = new SelectList(db.posts, "postId", "postName");
            workReportEntities dbA = new workReportEntities();
            user.post = Convert.ToInt32(fc["isPost"]);
            user.enteredDate = DateTime.Now.ToString("yyyy-MM-dd");
            bool isnotValid = db.user.ToList().Exists(p => p.userName.Equals(user.userName, StringComparison.CurrentCultureIgnoreCase));
            if (isnotValid)
            {
                return View(user);
            }

            else if (ModelState.IsValid)
            {

                dbA.user.Add(user);
                dbA.SaveChanges();
                return RedirectToAction("SignIn", "Login");


            }

            return View(user);
        }

        public ActionResult ForgotPassword()
        {

            return View();

        }

        [HttpPost]
        public ActionResult UpdatePassword(FormCollection fc)
        {
            string username = fc["username"].ToString();
            string password = fc["password"].ToString();
            user isuser = db.user.AsNoTracking().Where(x => x.userName == username).FirstOrDefault();
            isuser.userPassword = password;
            db.Entry(isuser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SignIn", "Login");

        }


    }
}