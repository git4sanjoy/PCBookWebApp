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
using PCBookWebApp.Models.ProcessModule;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    public class FinishedGoodStocksController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/FinishedGoodStocks
        public IQueryable<FinishedGoodStock> GetFinishedGoodStocks()
        {
            return db.FinishedGoodStocks;
        }

        // GET: api/FinishedGoodStocks/5
        [ResponseType(typeof(FinishedGoodStock))]
        public async Task<IHttpActionResult> GetFinishedGoodStock(int id)
        {
            FinishedGoodStock finishedGoodStock = await db.FinishedGoodStocks.FindAsync(id);
            if (finishedGoodStock == null)
            {
                return NotFound();
            }

            return Ok(finishedGoodStock);
        }

        // PUT: api/FinishedGoodStocks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFinishedGoodStock(int id, FinishedGoodStock finishedGoodStock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != finishedGoodStock.FinishedGoodStockId)
            {
                return BadRequest();
            }

            db.Entry(finishedGoodStock).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinishedGoodStockExists(id))
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

        // POST: api/FinishedGoodStocks
        [ResponseType(typeof(FinishedGoodStock))]
        public async Task<IHttpActionResult> PostFinishedGoodStock(FinishedGoodStock finishedGoodStock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FinishedGoodStocks.Add(finishedGoodStock);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = finishedGoodStock.FinishedGoodStockId }, finishedGoodStock);
        }

        // DELETE: api/FinishedGoodStocks/5
        [ResponseType(typeof(FinishedGoodStock))]
        public async Task<IHttpActionResult> DeleteFinishedGoodStock(int id)
        {
            FinishedGoodStock finishedGoodStock = await db.FinishedGoodStocks.FindAsync(id);
            if (finishedGoodStock == null)
            {
                return NotFound();
            }

            db.FinishedGoodStocks.Remove(finishedGoodStock);
            await db.SaveChangesAsync();

            return Ok(finishedGoodStock);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FinishedGoodStockExists(int id)
        {
            return db.FinishedGoodStocks.Count(e => e.FinishedGoodStockId == id) > 0;
        }
    }
}