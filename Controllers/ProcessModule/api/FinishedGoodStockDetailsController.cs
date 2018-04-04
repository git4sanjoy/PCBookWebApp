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
    public class FinishedGoodStockDetailsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/FinishedGoodStockDetails
        public IQueryable<FinishedGoodStockDetails> GetFinishedGoodStockDetails()
        {
            return db.FinishedGoodStockDetails;
        }

        // GET: api/FinishedGoodStockDetails/5
        [ResponseType(typeof(FinishedGoodStockDetails))]
        public async Task<IHttpActionResult> GetFinishedGoodStockDetails(int id)
        {
            FinishedGoodStockDetails finishedGoodStockDetails = await db.FinishedGoodStockDetails.FindAsync(id);
            if (finishedGoodStockDetails == null)
            {
                return NotFound();
            }

            return Ok(finishedGoodStockDetails);
        }

        // PUT: api/FinishedGoodStockDetails/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFinishedGoodStockDetails(int id, FinishedGoodStockDetails finishedGoodStockDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != finishedGoodStockDetails.FinishedGoodStockDetailsId)
            {
                return BadRequest();
            }

            db.Entry(finishedGoodStockDetails).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinishedGoodStockDetailsExists(id))
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

        // POST: api/FinishedGoodStockDetails
        //[ResponseType(typeof(FinishedGoodStockDetails))]
        [Route("api/FinishedGoodStockDetails/PostFinishedGoodStockDetails/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> PostFinishedGoodStockDetails(int id, List<FinishedGoodStockDetails> finishedGoodStockDetails)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (finishedGoodStockDetails != null)
            {
                foreach (var item in finishedGoodStockDetails)
                {
                    item.FinishedGoodStockId = id;  
                    db.FinishedGoodStockDetails.Add(item);
                    await db.SaveChangesAsync();
                }
            }         

            return StatusCode(HttpStatusCode.NoContent);
            //return CreatedAtRoute("DefaultApi", new { id = finishedGoodStockDetails.FinishedGoodStockDetailsId }, finishedGoodStockDetails);
        }

        // DELETE: api/FinishedGoodStockDetails/5
        [ResponseType(typeof(FinishedGoodStockDetails))]
        public async Task<IHttpActionResult> DeleteFinishedGoodStockDetails(int id)
        {
            FinishedGoodStockDetails finishedGoodStockDetails = await db.FinishedGoodStockDetails.FindAsync(id);
            if (finishedGoodStockDetails == null)
            {
                return NotFound();
            }

            db.FinishedGoodStockDetails.Remove(finishedGoodStockDetails);
            await db.SaveChangesAsync();

            return Ok(finishedGoodStockDetails);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FinishedGoodStockDetailsExists(int id)
        {
            return db.FinishedGoodStockDetails.Count(e => e.FinishedGoodStockDetailsId == id) > 0;
        }
    }
}