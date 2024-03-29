﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

using Windows.ApplicationModel.Email;
using workReport.Models;
namespace workReport.Controllers

{
    public class LoginController : Controller
    {
        private workReportEntities db = new workReportEntities();
        
        public ActionResult addDate()
        {
           
            var yr = 2020;
            for (var j = 1; j <= 50; j++)
            {

                for (var i = 1; i <= 12; i++)
                {
                    engDateList engList = new engDateList();
                    engList.engYear = yr;
                    engList.engMonth = i;
                    engList.daysCount = DateTime.DaysInMonth(yr, i);
                    db.engDateList.Add(engList);
                    db.SaveChanges();

                    // var dataa = yr + " " + i + " " + DateTime.DaysInMonth(yr, i) + "<br />";

                }
                yr = yr + 1;
            }
            return View();
        }

        public ActionResult SignIn()
        {
            //if(Request.Cookies["username"].Value!=null)
            //{
            //    ViewBag.username = Request.Cookies["username"].Value;
            //    return View();
            //}
            //else
            {
                return View();
            }
           
         
        }
        [HttpPost]
        public ActionResult SignIn(FormCollection fc)
        {

            int rememberChecked = (fc["remember"]==null)?0:1 ;
            string email = fc["username"].ToString();
            string password = fc["password"].ToString();

            user userYes = db.user.AsNoTracking().Where(x => x.userName == email).FirstOrDefault() ;

            string encKey = userYes.passkey;

            string encryptedPassword =CommonController.Encrypt(password, encKey);

            bool isok = db.user.Any(x => x.userName == email && x.passencryp == encryptedPassword);

            if (isok)
            {
                user isuser = db.user.AsNoTracking().Where(x => x.userName == email && x.passencryp == encryptedPassword).First();
                MisC_Data visitCount = db.MisC_Data.Find(1);
                visitCount.Visit_Count += 1;
                db.Entry(visitCount).State = EntityState.Modified;
                db.SaveChanges();
                //Response.Cookies["username"].Value = email;
                //Response.Cookies["username"].Expires = DateTime.Now.AddMonths(40);
               
                Session["userName"] = (isuser.userName);
                Session["userId"] = isuser.usrId;
                Session["userPost"] = isuser.post;

                return RedirectToAction("Index", "workLists");


            }
            else
            {
                ViewBag.message = "Username or password incorrect";


                return View();
            }



            // ModelState.AddModelError("", "Username and Password not matched!!");
        }


       

        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult ForgotPassword(string UserName)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (WebSecurity.UserExists(UserName))
        //        {
        //            string To = UserName, UserID, Password, SMTPPort, Host;
        //            string token = WebSecurity.GeneratePasswordResetToken(UserName);
        //            if (token == null)
        //            {
        //                // If user does not exist or is not confirmed.
        //                return View("Index");
        //            }
        //            else
        //            {
        //                //Create URL with above token
        //                var lnkHref = "<a href='" + Url.Action("ResetPassword", "Account", new { email = UserName, code = token }, "http") + "'>Reset Password</a>";
        //                //HTML Template for Send email
        //                string subject = "Your changed password";
        //                string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;
        //                //Get and set the AppSettings using configuration manager.
        //    //     EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);  uncommentthis
        //                //Call send email methods.
        //     //           EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);  uncomment this
        //            }
        //        }
        //    }
        //    return View();
        //}



        public ActionResult SignOut()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return Redirect("/Login/SignIn");
        }

        public string UnAuthorized()
        {
            //Response.Write("Oops!! You are not authrozed to perform this action.");
            string msg = "<h1>Oops!! You are not authrozed to perform this action.</h1>";
            return msg;
        }
    }
}