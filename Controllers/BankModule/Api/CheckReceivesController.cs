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
using PCBookWebApp.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using PCBookWebApp.Models.BankModule;

namespace PCBookWebApp.Controllers.BankModule.Api
{
    public class CheckReceivesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/CheckReceives
        public IQueryable<CheckReceive> GetCheckReceives()
        {
            return db.CheckReceives;
        }

        [Route("api/CheckReceives/PutVoucherDetailIdToCheckReceives/{CheckReceiveId}/{VoucherDetailId}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVoucherDetailIdToCheckReceives(int CheckReceiveId, int VoucherDetailId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var CheckReceive = db.CheckReceives.Where(x => x.CheckReceiveId == CheckReceiveId).FirstOrDefault();
            if (CheckReceive != null)
            {
                CheckReceive.VoucherDetailId = VoucherDetailId;
                db.CheckReceives.AddOrUpdate(CheckReceive);
                db.SaveChanges();
            }
            return Ok();
        }
        // GET: api/CheckReceives/5
        [ResponseType(typeof(CheckReceive))]
        public async Task<IHttpActionResult> GetCheckReceive(int id)
        {
            CheckReceive checkReceive = await db.CheckReceives.FindAsync(id);
            if (checkReceive == null)
            {
                return NotFound();
            }

            return Ok(checkReceive);
        }

        // PUT: api/CheckReceives/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCheckReceiver(int id, CheckReceive checkReceive)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != checkReceive.CheckReceiveId)
            {
                return BadRequest();
            }

            db.Entry(checkReceive).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckReceiveExists(id))
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

        // POST: api/CheckReceives
        [ResponseType(typeof(CheckReceive))]
        public async Task<IHttpActionResult> PostCheckReceive(CheckReceive[] checkReceive)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //db.CheckReceives.Add(checkReceive);
            //await db.SaveChangesAsync();

            //return CreatedAtRoute("DefaultApi", new { id = checkReceive.CheckReceiveId }, checkReceive);

            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            string currentUserId = User.Identity.GetUserId();

            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            foreach (var item in checkReceive)
            {
                item.CreatedBy = userName;
                item.DateCreated = createdAt;
                item.DateUpdated = createdAt;
                item.ShowRoomId = showRoomId;
                db.CheckReceives.Add(item);
                await db.SaveChangesAsync();
            }
            return CreatedAtRoute("DefaultApi", new { status = "ok" }, checkReceive);
        }

        // DELETE: api/CheckReceives/5
        [ResponseType(typeof(CheckReceive))]
        public async Task<IHttpActionResult> DeleteCheckReceive(int id)
        {
            CheckReceive checkReceive = await db.CheckReceives.FindAsync(id);
            if (checkReceive == null)
            {
                return NotFound();
            }

            db.CheckReceives.Remove(checkReceive);
            await db.SaveChangesAsync();

            return Ok(checkReceive);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CheckReceiveExists(int id)
        {
            return db.CheckReceives.Count(e => e.CheckReceiveId == id) > 0;
        }
    }
}