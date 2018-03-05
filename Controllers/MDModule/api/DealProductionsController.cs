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
using PCBookWebApp.Models.MDModule;

namespace PCBookWebApp.Controllers.MDModule.api
{
    public class DealProductionsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/DealProductions
        public IQueryable<DealProduction> GetDealProductions()
        {
            return db.DealProductions;
        }

        // GET: api/DealProductions/5
        [ResponseType(typeof(DealProduction))]
        public async Task<IHttpActionResult> GetDealProduction(int id)
        {
            DealProduction dealProduction = await db.DealProductions.FindAsync(id);
            if (dealProduction == null)
            {
                return NotFound();
            }

            return Ok(dealProduction);
        }

        // PUT: api/DealProductions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDealProduction(int id, DealProduction dealProduction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dealProduction.DealProductionId)
            {
                return BadRequest();
            }

            db.Entry(dealProduction).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DealProductionExists(id))
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

        // POST: api/DealProductions
        [ResponseType(typeof(DealProduction))]
        public async Task<IHttpActionResult> PostDealProduction(DealProduction dealProduction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DealProductions.Add(dealProduction);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = dealProduction.DealProductionId }, dealProduction);
        }

        // DELETE: api/DealProductions/5
        [ResponseType(typeof(DealProduction))]
        public async Task<IHttpActionResult> DeleteDealProduction(int id)
        {
            DealProduction dealProduction = await db.DealProductions.FindAsync(id);
            if (dealProduction == null)
            {
                return NotFound();
            }

            db.DealProductions.Remove(dealProduction);
            await db.SaveChangesAsync();

            return Ok(dealProduction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DealProductionExists(int id)
        {
            return db.DealProductions.Count(e => e.DealProductionId == id) > 0;
        }
    }
}