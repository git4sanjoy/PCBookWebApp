using PCBookWebApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace PCBookWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Contact() {
            return View();
        }

        /// <summary>
        /// Action Method to Handle the Upload Functionalty
        /// </summary>
        /// <param name="aFile"></param>
        [HttpPost]
        public void Upload(System.Web.HttpPostedFileBase aFile)
        {
            string file = aFile.FileName;
            string path = Server.MapPath("../Upload//");
            //aFile.SaveAs(path + Guid.NewGuid() + "." + file.Split('.')[1]);
            aFile.SaveAs(path + file);
        }
        [HttpPost]
        public void UploadCustomerImage(System.Web.HttpPostedFileBase aFile,int CustomerId)
        {
            string file = aFile.FileName;
            string path = Server.MapPath("../Upload//Customers//");
            //aFile.SaveAs(path + Guid.NewGuid() + "." + file.Split('.')[1]);
            aFile.SaveAs(path+ CustomerId + "." + file.Split('.')[1]);
        }
        [HttpPost]
        public void UploadProductImage(System.Web.HttpPostedFileBase aFile, int ProductId)
        {
            string file = aFile.FileName;
            string path = Server.MapPath("../Upload//Products//");
            //aFile.SaveAs(path + Guid.NewGuid() + "." + file.Split('.')[1]);
            aFile.SaveAs(path + ProductId + "." + file.Split('.')[1]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("it@pakizagroup.com"));      // who get the mail with valid address 
                message.From = new MailAddress("sanjoy@pakizagroup.com");   // replace with valid value smtp info
                message.Subject = model.Message;
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "sanjoy@pakizagroup.com",    // replace with valid value
                        Password = "P@kiza123"                  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "email.pakizagroup.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;

                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate,
                             X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };

                    smtp.Send(message);
                    //return RedirectToAction("Sent");
                    return this.Json("send");
                }
            }
            return View(model);
        }

        public ActionResult Sent()
        {
            return View();
        }

    }
}
