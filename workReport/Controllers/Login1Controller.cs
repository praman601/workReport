//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Security;
//using workReport.Models;
//namespace workReport.Controllers
//{
//    public class LoginController : Controller
//    {
//        private workReportEntities db = new workReportEntities();
//        public ActionResult SignIn()
//        {
//            //Session.Abandon();
//            return View();
//        }
//        [HttpPost]
//        public ActionResult SignIn(FormCollection fc)
//        {


//            string email = fc["username"].ToString();
//            string password = fc["password"].ToString();

//            bool isok = db.user.Any(x => x.userName == email && x.userPassword == password);


//            if (isok)
//            {
//                user isuser = db.user.AsNoTracking().Where(x => x.userName == email && x.userPassword == password).First();

//                Session["userName"] = (isuser.userName);
//                Session["userId"] = isuser.usrId;
//                Session["userPost"] = isuser.post;

//                return RedirectToAction("Index", "workLists");


//            }
//            else
//            {
//                ViewBag.message = "Username or password incorrect";


//                return View();
//            }



//            // ModelState.AddModelError("", "Username and Password not matched!!");
//        }
//        public ActionResult SignOut()
//        {
//            Session.Abandon();
//            FormsAuthentication.SignOut();
//            return Redirect("/Login/SignIn");
//        }

//        public string UnAuthorized()
//        {
//            //Response.Write("Oops!! You are not authrozed to perform this action.");
//            string msg = "<h1>Oops!! You are not authrozed to perform this action.</h1>";
//            return msg;
//        }
//    }
//}