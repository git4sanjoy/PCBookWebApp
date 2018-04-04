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
    public class DivisionsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Divisions/AllReportList")]
        [HttpGet]
        public IHttpActionResult GetAllReportList()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == currentUserId).Select(a => a.ShowRoomId).FirstOrDefault();

            var listDivisions = db.Divisions
                .Where(d => d.DivisionId != 9)
                .Select(e => new {
                    DivisionId = e.DivisionId,
                    DivisionName = e.DivisionName,
                    DivisionNameBangla = e.DivisionNameBangla
                })
                .OrderBy(e => e.DivisionName);
            var listZoneManagers = db.ZoneManagers
                .Where(zm=>zm.ZoneManagerId <= 7)
                            .OrderBy(d => d.ZoneManagerId)
                            .Select(e => new {
                                ZoneManagerId = e.ZoneManagerId,
                                ZoneManagerName = e.ZoneManagerName
                            });
            var listSaleZones = db.SaleZones
                .Select(e => new {
                    id = e.SaleZoneId,
                    label = e.SaleZoneName,
                    ZoneManagerId = e.ZoneManager.ZoneManagerId,
                    ZoneManagerName = e.ZoneManager.ZoneManagerName,
                    DivisionId = e.DivisionId,
                    e.SaleZoneDescription
                })
                .OrderBy(e => e.label);
            var listDistricts = db.Districts.Select(e => new { id = e.DistrictId, label = e.DistrictName });

            return Ok(new { listDivisions , listZoneManagers , listSaleZones , listDistricts });
        }

        [Route("api/Divisions/DivisionsList")]
        [HttpGet]
        public IHttpActionResult GetDivisionsList()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == currentUserId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.Divisions
                .Select(e => new {
                    id = e.DivisionId,
                    name = e.DivisionName,
                    DivisionNameBangla = e.DivisionNameBangla
                })
                .OrderBy(e => e.name);
            return Ok(list);
        }

        [Route("api/Divisions/DivisionsListXEdit")]
        [HttpGet]
        public IHttpActionResult GetDivisionsListXEdit()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == currentUserId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.Divisions
                            .Select(e => new { value = e.DivisionId, text = e.DivisionName });
            return Ok(list);
        }

        // GET: api/Divisions
        public IQueryable<Division> GetDivisions()
        {
            return db.Divisions;
        }

        // GET: api/Divisions/5
        [ResponseType(typeof(Division))]
        public async Task<IHttpActionResult> GetDivision(int id)
        {
            Division division = await db.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }

            return Ok(division);
        }

        // PUT: api/Divisions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDivision(int id, Division division)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != division.DivisionId)
            {
                return BadRequest();
            }

            db.Entry(division).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DivisionExists(id))
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

        // POST: api/Divisions
        [ResponseType(typeof(Division))]
        public async Task<IHttpActionResult> PostDivision(Division division)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Divisions.Add(division);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = division.DivisionId }, division);
        }

        // DELETE: api/Divisions/5
        [ResponseType(typeof(Division))]
        public async Task<IHttpActionResult> DeleteDivision(int id)
        {
            Division division = await db.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }

            db.Divisions.Remove(division);
            await db.SaveChangesAsync();

            return Ok(division);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DivisionExists(int id)
        {
            return db.Divisions.Count(e => e.DivisionId == id) > 0;
        }
    }
}