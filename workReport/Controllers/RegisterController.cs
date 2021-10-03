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
            string password = user.userPassword;
            //user.userPassword = Encrypt(password);
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


        private string Encrypt(string clearText)
        {
            string EncryptionKey = "Q3D3I6X1A6G2AY8";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "Q3D3I6X1A6G2AY8";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }





    }
}