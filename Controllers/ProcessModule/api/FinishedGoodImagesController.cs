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
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    public class FinishedGoodImagesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        // GET: api/FinishedGoodImages
        public IQueryable<FinishedGoodImage> GetFinishedGoodImages()
        {
            return db.FinishedGoodImages;
        }

        // GET: api/FinishedGoodImages/5
        [ResponseType(typeof(FinishedGoodImage))]
        public async Task<IHttpActionResult> GetFinishedGoodImage(int id)
        {
            FinishedGoodImage finishedGoodImage = await db.FinishedGoodImages.FindAsync(id);
            if (finishedGoodImage == null)
            {
                return NotFound();
            }

            return Ok(finishedGoodImage);
        }

        // PUT: api/FinishedGoodImages/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFinishedGoodImage(int id, FinishedGoodImage finishedGoodImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != finishedGoodImage.FinishedGoodImageId)
            {
                return BadRequest();
            }

            db.Entry(finishedGoodImage).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinishedGoodImageExists(id))
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

        // POST: api/FinishedGoodImages
        [ResponseType(typeof(FinishedGoodImage))]
        public async Task<IHttpActionResult> PostFinishedGoodImage(FinishedGoodImage finishedGoodImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FinishedGoodImages.Add(finishedGoodImage);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = finishedGoodImage.FinishedGoodImageId }, finishedGoodImage);

        }

        // DELETE: api/FinishedGoodImages/5
        [ResponseType(typeof(FinishedGoodImage))]
        public async Task<IHttpActionResult> DeleteFinishedGoodImage(int id)
        {
            FinishedGoodImage finishedGoodImage = await db.FinishedGoodImages.FindAsync(id);
            if (finishedGoodImage == null)
            {
                return NotFound();
            }

            db.FinishedGoodImages.Remove(finishedGoodImage);
            await db.SaveChangesAsync();

            return Ok(finishedGoodImage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FinishedGoodImageExists(int id)
        {
            return db.FinishedGoodImages.Count(e => e.FinishedGoodImageId == id) > 0;
        }
    }
}