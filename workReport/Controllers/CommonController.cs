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
    public class CommonController : Controller
    {
        private workReportEntities db = new workReportEntities();
        private static Random random = new Random();

        //add encKey and encPassword to individual
        //public ActionResult SignIn()
        //{
        //    //Session.Abandon();
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult SignIn1(FormCollection fc)
        //{
        //    string email = fc["username"].ToString();
        //    string password = fc["password"].ToString();
        //    string encKey = CommonController.RandomString(15);
        //    string passwordenc= CommonController.Encrypt(password, encKey);
        //    user userYes = db.user.AsNoTracking().Where(x => x.userName == email).FirstOrDefault();
        //    userYes.passkey = encKey;
        //    userYes.passencryp = passwordenc;

        //    db.Entry(userYes).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("SignIn");
        //}



        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string Encrypt(string clearText, string EncKey)
        {

            string EncryptionKey = EncKey;
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

        public static string Decrypt(string cipherText, string decKey)
        {
            string EncryptionKey = decKey;
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