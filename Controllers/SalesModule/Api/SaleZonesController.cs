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
    public class SaleZonesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/SaleZones/SaleZonesList")]
        [HttpGet]
        public IHttpActionResult GetSaleZonesList()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == currentUserId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.SaleZones
                .Select(e => new {
                    id = e.SaleZoneId,
                    name = e.SaleZoneName,
                    group = e.ZoneManager.ZoneManagerId,
                    groupName = e.ZoneManager.ZoneManagerName,
                    status = e.DivisionId,
                    e.SaleZoneDescription
                })
                .OrderBy(e => e.name);
            return Ok(list);
        }




        [Route("api/SaleZones/SalesZoneDeropDownList")]
        [HttpGet]
        [ResponseType(typeof(ZoneManager))]
        public IHttpActionResult GetSalesZoneDeropDownList()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var list = db.SaleZones
                .Select(e => new { SaleZoneId = e.SaleZoneId, SaleZoneName = e.SaleZoneName })
                .OrderBy(e => e.SaleZoneName);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/SaleZones/ZoneListXEdit")]
        [HttpGet]
        [ResponseType(typeof(ZoneManager))]
        public IHttpActionResult GetZoneListXEdit()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var list = db.SaleZones
                .Select(e => new { id = e.SaleZoneId, text = e.SaleZoneName })
                .OrderBy(e => e.text);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }
        [Route("api/SaleZones/ZoneManagersListXEdit")]
        [HttpGet]
        [ResponseType(typeof(ZoneManager))]
        public IHttpActionResult GetZoneManagersListXEdit()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var list = db.ZoneManagers
                .Select(e => new { id = e.ZoneManagerId, text = e.ZoneManagerName })
                .OrderBy(e => e.text);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }
        // GET: api/SaleZones
        public IQueryable<SaleZone> GetSaleZones()
        {
            return db.SaleZones;
        }

        // GET: api/SaleZones/5
        [ResponseType(typeof(SaleZone))]
        public async Task<IHttpActionResult> GetSaleZone(int id)
        {
            SaleZone saleZone = await db.SaleZones.FindAsync(id);
            if (saleZone == null)
            {
                return NotFound();
            }

            return Ok(saleZone);
        }

        // PUT: api/SaleZones/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSaleZone(int id, SaleZone saleZone)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != saleZone.SaleZoneId)
            {
                return BadRequest();
            }

            db.Entry(saleZone).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleZoneExists(id))
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

        // POST: api/SaleZones
        [ResponseType(typeof(SaleZone))]
        public async Task<IHttpActionResult> PostSaleZone(SaleZone saleZone)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SaleZones.Add(saleZone);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = saleZone.SaleZoneId }, saleZone);
        }

        // DELETE: api/SaleZones/5
        [ResponseType(typeof(SaleZone))]
        public async Task<IHttpActionResult> DeleteSaleZone(int id)
        {
            SaleZone saleZone = await db.SaleZones.FindAsync(id);
            if (saleZone == null)
            {
                return NotFound();
            }

            db.SaleZones.Remove(saleZone);
            await db.SaveChangesAsync();

            return Ok(saleZone);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SaleZoneExists(int id)
        {
            return db.SaleZones.Count(e => e.SaleZoneId == id) > 0;
        }
    }
}