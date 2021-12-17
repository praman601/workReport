using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using workReport.Models;
using workReport.Controllers;
using System.Net.Mail;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace workReport.Controllers
{
    public class RegisterController : Controller
    {
        private workReportEntities db = new workReportEntities();
        private static Random random = new Random();
        public ActionResult Create()
        {
            ViewBag.isPost = new SelectList(db.posts, "postId", "postName");
            user model = new user();
            return View(model);
        }

        public ActionResult SildeGallery()
        {
            var idParam = new SqlParameter
            {
                ParameterName = "userId",
                Value = 6
            };

            var data = db.Database.SqlQuery<workListProc>("exec allUsersDataCount @userId", idParam).ToList();
          /*  var dataaatest = data.AsEnumerable().ToList()*/;
            DataTable dt = new DataTable();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
           dt = JsonConvert.DeserializeObject<DataTable>(json);
            return View();
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
            string password = user.userPassword;
            string encKey = CommonController.RandomString(15);
            user.passkey = encKey;
            user.passencryp = CommonController.Encrypt(password, encKey);
            //string passwordis = Decrypt(user.userPassword);
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

        public ActionResult ForgotPassword1(FormCollection fc)
        {
            string username = fc["username"].ToString();
            string email = fc["email"].ToString();
           
            user userYes = db.user.AsNoTracking().Where(x => x.userName == username).FirstOrDefault();
       
            if (userYes!=null)
            {
                string newPassword = GeneratePassword();
                string encKey = CommonController.RandomString(15);
                string encPassword = CommonController.Encrypt(newPassword, encKey);
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("ghimire.kailashkailu@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "Reset Password";
                    mail.Body = "<h1>Please find the new password.</h1> </br>"+"<h3>Password : "+newPassword+"</h3>" ;
                    mail.IsBodyHtml = true;
                  //   mail.Attachments.Add(new Attachment("E:\\PASSPORT SIZE PHOTO.jpeg"));

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("ghimire.kailashkailu@gmail.com", "kailu1234");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        userYes.userPassword = newPassword;
                        userYes.passkey = encKey;
                        userYes.passencryp = encPassword;
                        db.Entry(userYes).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                TempData["msg"] = "Email Sent Successfully";
                
                return RedirectToAction("ForgotPassword");
            }
            else
            {
                ViewBag.msg = "User doesnot Exist!!";
                return RedirectToAction("ForgotPassword");
            }
        }

        public string GeneratePassword()
        {
            string PasswordLength = "8";
            string NewPassword = "";

            string allowedChars = "";
            allowedChars = "1,2,3,4,5,6,7,8,9,0";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";


            char[] sep = {
            ','
        };
            string[] arr = allowedChars.Split(sep);


            string IDString = "";
            string temp = "";

            Random rand = new Random();

            for (int i = 0; i < Convert.ToInt32(PasswordLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                IDString += temp;
                NewPassword = IDString;

            }
            return NewPassword;
        }



        [HttpPost]
        public ActionResult UpdatePassword(FormCollection fc)
        {
            string username = fc["username"].ToString();
            string password = fc["password"].ToString();
            user isuser = db.user.AsNoTracking().Where(x => x.userName == username).FirstOrDefault();
            string encKey =CommonController.RandomString(15);
            isuser.passkey = encKey;
            isuser.passencryp =CommonController.Encrypt(password, encKey);
            isuser.userPassword = password;
            db.Entry(isuser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SignIn", "Login");

        }

      

       




    }
}