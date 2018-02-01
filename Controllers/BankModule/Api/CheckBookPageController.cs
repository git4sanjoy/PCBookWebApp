using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PCBookWebApp.DAL;
using PCBookWebApp.Models;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.BookViewModel;
using PCBookWebApp.Models.BankModule;

namespace PCBookWebApp.Controllers.BankModule.Api
{
    public class CheckBookPageController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // POST: api/CheckBookPage/PostCheckBookPage
        [HttpPost]
        [Route("api/CheckBookPage/PostCheckBookPage")]
        public IHttpActionResult PostCheckBookPage(CheckBookPageView checkBookPageView)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            int i = 0;
            CheckBookPage checkBookPage = new CheckBookPage();
            for (double checkNo = checkBookPageView.StartNo; checkNo <= checkBookPageView.EndNo; checkNo++)
            {
                checkBookPage.CreatedBy = userName;
                checkBookPage.DateCreated = createdAt;
                checkBookPage.CheckBookId = checkBookPageView.CheckBookId;
                checkBookPage.CheckBookPageNo = checkBookPageView.StartSuffices + checkNo;
                db.CheckBookPages.Add(checkBookPage);
                db.SaveChanges();
                i++;
            }

            return Ok();
        }

        // GET: api/CheckBookPage/GetCheckBookPageList
        [HttpGet]
        [Route("api/CheckBookPage/GetCheckBookPageList/{BankAccountId}")]
        [ResponseType(typeof(CheckBookPage))]
        public IHttpActionResult GetCheckBookPageList(int BankAccountId)
        {
            var checkNoList = db.CheckBookPages
                                .Where(p=> p.CheckBook.BankAccountId == BankAccountId && p.Active == false)
                                .Select(p=> new { CheckBookPageId = p.CheckBookPageId, CheckBookPageNo = p.CheckBookPageNo, CheckBookId = p.CheckBookId });
            if (checkNoList == null)
            {
                return NotFound();
            }
            return Ok(checkNoList);
        }

        // GET: api/CheckBookPage
        public IQueryable<CheckBookPage> GetCheckBookPages()
        {
            return db.CheckBookPages;
        }

        // GET: api/CheckBookPage/5
        [ResponseType(typeof(CheckBookPage))]
        public IHttpActionResult GetCheckBookPage(int id)
        {
            CheckBookPage checkBookPage = db.CheckBookPages.Find(id);
            if (checkBookPage == null)
            {
                return NotFound();
            }

            return Ok(checkBookPage);
        }

        // PUT: api/CheckBookPage/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCheckBookPage(int id, CheckBookPage checkBookPage)
        {

            var aCheckBookPage = db.CheckBookPages.Find(id);
            aCheckBookPage.Active = true;
            db.Entry(aCheckBookPage).State = EntityState.Modified;
            db.SaveChanges();


            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (id != checkBookPage.CheckBookPageId)
            //{
            //    return BadRequest();
            //}

            //db.Entry(checkBookPage).State = EntityState.Modified;

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!CheckBookPageExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/CheckBookPage
        [ResponseType(typeof(CheckBookPage))]
        public IHttpActionResult PostCheckBookPage(CheckBookPage checkBookPage)
        {
            //int i = 0;
            //CheckBookPage checkBookPage = new CheckBookPage();
            //for (double checkNo = checkBookPageView.StartNo; checkNo <= checkBookPageView.EndNo; checkNo++)
            //{
            //    checkBookPage.CheckBookId = checkBookPageView.CheckBookId;
            //    checkBookPage.CheckBookPageNo = checkBookPageView.StartSuffices + checkNo;
            //    db.CheckBookPages.Add(checkBookPage);
            //    db.SaveChanges();
            //    i++;
            //}

            //return Ok();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CheckBookPages.Add(checkBookPage);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = checkBookPage.CheckBookPageId }, checkBookPage);
        }

        // DELETE: api/CheckBookPage/5
        [ResponseType(typeof(CheckBookPage))]
        public IHttpActionResult DeleteCheckBookPage(int id)
        {
            CheckBookPage checkBookPage = db.CheckBookPages.Find(id);
            if (checkBookPage == null)
            {
                return NotFound();
            }

            db.CheckBookPages.Remove(checkBookPage);
            db.SaveChanges();

            return Ok(checkBookPage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CheckBookPageExists(int id)
        {
            return db.CheckBookPages.Count(e => e.CheckBookPageId == id) > 0;
        }
    }
}