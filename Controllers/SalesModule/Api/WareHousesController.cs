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
    public class WareHousesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/WareHouses/WareHouseList")]
        [HttpGet]
        public IHttpActionResult GetWareHouseList()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == currentUserId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.WareHouses
                .Select(e => new {
                    id = e.WareHouseId,
                    WareHouseName = e.WareHouseName,
                    WareHouseLocation = e.WareHouseLocation
                })
                .OrderBy(e => e.WareHouseName);
            return Ok(list);
        }

        [Route("api/WareHouses/WareHouseDropDownList")]
        [HttpGet]
        public IHttpActionResult GetWareHouseDropDownList()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var wareHouseId = db.ZoneManagers.Where(a => a.Id == userId).Select(a => a.WareHouseId).FirstOrDefault();
            var list = db.WareHouses
                .Select(e => new {
                    WareHouseId = e.WareHouseId,
                    WareHouseName = e.WareHouseName
                })
                .Where(w =>w.WareHouseId== wareHouseId)
                .OrderBy(e => e.WareHouseName);
            return Ok(list);
        }

        // GET: api/WareHouses
        public IQueryable<WareHouse> GetWareHouses()
        {
            return db.WareHouses;
        }

        // GET: api/WareHouses/5
        [ResponseType(typeof(WareHouse))]
        public async Task<IHttpActionResult> GetWareHouse(int id)
        {
            WareHouse wareHouse = await db.WareHouses.FindAsync(id);
            if (wareHouse == null)
            {
                return NotFound();
            }

            return Ok(wareHouse);
        }

        // PUT: api/WareHouses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWareHouse(int id, WareHouse wareHouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wareHouse.WareHouseId)
            {
                return BadRequest();
            }

            db.Entry(wareHouse).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WareHouseExists(id))
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

        // POST: api/WareHouses
        [ResponseType(typeof(WareHouse))]
        public async Task<IHttpActionResult> PostWareHouse(WareHouse wareHouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WareHouses.Add(wareHouse);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = wareHouse.WareHouseId }, wareHouse);
        }

        // DELETE: api/WareHouses/5
        [ResponseType(typeof(WareHouse))]
        public async Task<IHttpActionResult> DeleteWareHouse(int id)
        {
            WareHouse wareHouse = await db.WareHouses.FindAsync(id);
            if (wareHouse == null)
            {
                return NotFound();
            }

            db.WareHouses.Remove(wareHouse);
            await db.SaveChangesAsync();

            return Ok(wareHouse);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WareHouseExists(int id)
        {
            return db.WareHouses.Count(e => e.WareHouseId == id) > 0;
        }
    }
}