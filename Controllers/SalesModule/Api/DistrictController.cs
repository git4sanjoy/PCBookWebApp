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
using PCBookWebApp.Models.SalesModule;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class DistrictController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/District/GetDropDownListXedit/
        [Route("api/District/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var unitList = db.Districts.Select(e => new { id = e.DistrictId, text = e.DistrictName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }

        // GET: api/District/GetDropDownList/
        [Route("api/District/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetDropDownList()
        {
            var listObj = db.Districts.Select(e => new { DistrictId = e.DistrictId, DistrictName = e.DistrictName, DistrictNameBangla = e.DistrictNameBangla });
            if (listObj == null)
            {
                return NotFound();
            }
            return Ok(listObj);
        }














        // GET: api/District
        public IQueryable<District> GetDistricts()
        {
            return db.Districts;
        }

        // GET: api/District/5
        [ResponseType(typeof(District))]
        public async Task<IHttpActionResult> GetDistrict(int id)
        {
            District district = await db.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            return Ok(district);
        }

        // PUT: api/District/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDistrict(int id, District district)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != district.DistrictId)
            {
                return BadRequest();
            }

            db.Entry(district).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistrictExists(id))
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

        // POST: api/District
        [ResponseType(typeof(District))]
        public async Task<IHttpActionResult> PostDistrict(District district)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Districts.Add(district);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = district.DistrictId }, district);
        }

        // DELETE: api/District/5
        [ResponseType(typeof(District))]
        public async Task<IHttpActionResult> DeleteDistrict(int id)
        {
            District district = await db.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            db.Districts.Remove(district);
            await db.SaveChangesAsync();

            return Ok(district);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DistrictExists(int id)
        {
            return db.Districts.Count(e => e.DistrictId == id) > 0;
        }
    }
}