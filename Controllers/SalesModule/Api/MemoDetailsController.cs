using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PCBookWebApp.DAL;
using PCBookWebApp.Models.SalesModule;
using Microsoft.AspNet.Identity;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    public class MemoDetailsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/MemoDetails
        public IQueryable<MemoDetail> GetMemoDetails()
        {
            return db.MemoDetails;
        }

        // GET: api/MemoDetails/5
        [ResponseType(typeof(MemoDetail))]
        public async Task<IHttpActionResult> GetMemoDetail(int id)
        {
            MemoDetail memoDetail = await db.MemoDetails.FindAsync(id);
            if (memoDetail == null)
            {
                return NotFound();
            }

            return Ok(memoDetail);
        }

        // PUT: api/MemoDetails/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMemoDetail(int id, MemoDetail memoDetail)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            memoDetail.DateCreated = ceatedAt;
            memoDetail.DateUpdated = ceatedAt;
            memoDetail.CreatedBy = userName;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != memoDetail.MemoDetailId)
            {
                return BadRequest();
            }

            db.Entry(memoDetail).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemoDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/MemoDetails
        [ResponseType(typeof(MemoDetail))]
        public async Task<IHttpActionResult> PostMemoDetail(MemoDetail[] memoDetail)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;

            //
            foreach (var item in memoDetail)
            { 
                item.CreatedBy = userName;
                item.DateCreated = ceatedAt;
                item.DateUpdated = ceatedAt;
                db.MemoDetails.Add(item);
                await db.SaveChangesAsync();
            }

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //db.MemoDetails.Add(memoDetail);
            //await db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new { status = "ok" }, memoDetail);
            //return CreatedAtRoute("DefaultApi", new { id = memoDetail.MemoDetailId }, memoDetail);
        }

        // DELETE: api/MemoDetails/5
        [ResponseType(typeof(MemoDetail))]
        public async Task<IHttpActionResult> DeleteMemoDetail(int id)
        {
            MemoDetail memoDetail = await db.MemoDetails.FindAsync(id);
            if (memoDetail == null)
            {
                return NotFound();
            }

            db.MemoDetails.Remove(memoDetail);
            await db.SaveChangesAsync();

            return Ok(memoDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MemoDetailExists(int id)
        {
            return db.MemoDetails.Count(e => e.MemoDetailId == id) > 0;
        }
    }
}