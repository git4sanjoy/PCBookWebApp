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
    [Authorize]
    public class ZoneManagersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/ZoneManagers/ZoneManagerList")]
        [HttpGet]
        public IHttpActionResult GetZoneManagerList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.ZoneManagers
                            .OrderBy(d => d.ZoneManagerName)
                            .Select(e => new {
                                id = e.ZoneManagerId,
                                ZoneManagerName = e.ZoneManagerName,
                                Phone = e.Phone,
                                Address = e.Address,
                                Email = e.Email
                            });

            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        // GET: api/ZoneManagers
        public IQueryable<ZoneManager> GetZoneManagers()
        {
            return db.ZoneManagers;
        }

        // GET: api/ZoneManagers/5
        [ResponseType(typeof(ZoneManager))]
        public async Task<IHttpActionResult> GetZoneManager(int id)
        {
            ZoneManager zoneManager = await db.ZoneManagers.FindAsync(id);
            if (zoneManager == null)
            {
                return NotFound();
            }

            return Ok(zoneManager);
        }

        // PUT: api/ZoneManagers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutZoneManager(int id, ZoneManager zoneManager)
        {
            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            zoneManager.DateCreated = ceatedAt;
            zoneManager.DateUpdated = ceatedAt;
            zoneManager.CreatedBy = userName;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != zoneManager.ZoneManagerId)
            {
                return BadRequest();
            }

            db.Entry(zoneManager).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZoneManagerExists(id))
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

        // POST: api/ZoneManagers
        [ResponseType(typeof(ZoneManager))]
        public async Task<IHttpActionResult> PostZoneManager(ZoneManager zoneManager)
        {
            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            zoneManager.DateCreated = ceatedAt;
            zoneManager.DateUpdated = ceatedAt;
            zoneManager.CreatedBy = userName;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ZoneManagers.Add(zoneManager);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = zoneManager.ZoneManagerId }, zoneManager);
        }

        // DELETE: api/ZoneManagers/5
        [ResponseType(typeof(ZoneManager))]
        public async Task<IHttpActionResult> DeleteZoneManager(int id)
        {
            ZoneManager zoneManager = await db.ZoneManagers.FindAsync(id);
            if (zoneManager == null)
            {
                return NotFound();
            }

            db.ZoneManagers.Remove(zoneManager);
            await db.SaveChangesAsync();

            return Ok(zoneManager);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ZoneManagerExists(int id)
        {
            return db.ZoneManagers.Count(e => e.ZoneManagerId == id) > 0;
        }
    }
}