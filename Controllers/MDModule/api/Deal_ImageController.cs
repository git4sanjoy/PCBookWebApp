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
    public class Deal_ImageController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Deal_Image/DealsImageList")]
        [HttpGet]
        public IHttpActionResult GetDealsImageList()
        {
            var list = db.Deal_Image
                        .Include(a => a.Deal)
                        .ToList()
                        .Select(x => new { x.Id, x.ImageUrl, x.Deal.Name, x.Deal.Description })
                        .ToList();
            return Ok(list);

        }

        // GET: api/Deal_Image
        public IQueryable<Deal_Image> GetDeal_Image()
        {

            return db.Deal_Image;
            
        }

        // GET: api/Deal_Image/5
        [ResponseType(typeof(Deal_Image))]
        public async Task<IHttpActionResult> GetDeal_Image(Guid id)
        {
            Deal_Image deal_Image = await db.Deal_Image.FindAsync(id);
            if (deal_Image == null)
            {
                return NotFound();
            }

            return Ok(deal_Image);
        }

        // PUT: api/Deal_Image/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDeal_Image(Guid id, Deal_Image deal_Image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != deal_Image.Id)
            {
                return BadRequest();
            }

            db.Entry(deal_Image).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Deal_ImageExists(id))
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

        // POST: api/Deal_Image
        [ResponseType(typeof(Deal_Image))]
        public async Task<IHttpActionResult> PostDeal_Image(Deal_Image deal_Image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Deal_Image.Add(deal_Image);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Deal_ImageExists(deal_Image.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = deal_Image.Id }, deal_Image);
        }

        // DELETE: api/Deal_Image/5
        [ResponseType(typeof(Deal_Image))]
        public async Task<IHttpActionResult> DeleteDeal_Image(Guid id)
        {
            Deal_Image deal_Image = await db.Deal_Image.FindAsync(id);
            if (deal_Image == null)
            {
                return NotFound();
            }

            db.Deal_Image.Remove(deal_Image);
            await db.SaveChangesAsync();

            return Ok(deal_Image);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Deal_ImageExists(Guid id)
        {
            return db.Deal_Image.Count(e => e.Id == id) > 0;
        }
    }
}