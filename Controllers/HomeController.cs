using PCBookWebApp.DAL;
using PCBookWebApp.Models.BookModule;
using PCBookWebApp.Models.ProcessModule;
using PCBookWebApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
        private PCBookWebAppContext db = new PCBookWebAppContext();

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

        [HttpPost]
        public virtual string UploadFiles(object obj)
        {
            var length = Request.ContentLength;
            var bytes = new byte[length];
            Request.InputStream.Read(bytes, 0, length);

            var fileName = Request.Headers["X-File-Name"];
            var fileSize = Request.Headers["X-File-Size"];
            var fileType = Request.Headers["X-File-Type"];

            string path = Server.MapPath("../Upload//FinishedGoodImages//");
            //var saveToFileLoc = "\\\\adcyngctg\\HRMS\\Images\\" + fileName;
            var saveToFileLoc = path + fileName;

            // save the file.
            var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite);
            fileStream.Write(bytes, 0, length);
            fileStream.Close();

            return string.Format("{0} bytes uploaded", bytes.Length);
        }

        [HttpPost]
        public ActionResult SaveTutorial(FinishedGoodImage tutorial)
        {
            foreach (string file in Request.Files)
            {
                var fileContent = Request.Files[file];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var inputStream = fileContent.InputStream;
                    var fileName = Path.GetFileName(file);
                    var path = Path.Combine(Server.MapPath("~/App_Data/Images"), fileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        inputStream.CopyTo(fileStream);
                    }
                }
            }
            return Json("Tutorial Saved", JsonRequestBehavior.AllowGet);
        }

        //Recursion method for recursively get all child nodes
        public void GetTreeview(List<Group> list, Group current, ref List<Group> returnList)
        {
            //get child of current item
            var childs = list.Where(a => a.ParentId == current.GroupId).ToList();
            current.Childrens = new List<Group>();
            current.Childrens.AddRange(childs);
            foreach (var i in childs)
            {
                GetTreeview(list, i, ref returnList);
            }
        }

        public List<Group> BuildTree(List<Group> list)
        {
            List<Group> returnList = new List<Group>();
            //find top levels items
            var topLevels = list.Where(a => a.ParentId == list.OrderBy(b => b.GroupId).FirstOrDefault().ParentId);
            returnList.AddRange(topLevels);
            foreach (var i in topLevels)
            {
                GetTreeview(list, i, ref returnList);
            }
            return returnList;
        }
        public JsonResult GetFileStructure()
        {
            List<Group> list = new List<Group>();
            using (PCBookWebAppContext dc = new PCBookWebAppContext())
            {
                list = dc.Groups.OrderBy(a => a.GroupName).ToList();
            }

            List<Group> treelist = new List<Group>();
            if (list.Count > 0)
            {
                treelist = BuildTree(list);
            }

            return new JsonResult { Data = new { treeList = treelist }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }


}
