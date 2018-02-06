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
    public class MatricsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/Matrics
        public IQueryable<Matric> GetMatrics()
        {
            return db.Matrics;
        }

        // GET: api/Matrics/5
        [ResponseType(typeof(Matric))]
        public async Task<IHttpActionResult> GetMatric(int id)
        {
            Matric matric = await db.Matrics.FindAsync(id);
            if (matric == null)
            {
                return NotFound();
            }

            return Ok(matric);
        }

        // PUT: api/Matrics/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMatric(int id, Matric matric)
        {
            matric.DateCreated = DateTime.Now;
            matric.DateUpdated = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != matric.MatricId)
            {
                return BadRequest();
            }

            db.Entry(matric).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatricExists(id))
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

        // POST: api/Matrics
        [ResponseType(typeof(Matric))]
        public async Task<IHttpActionResult> PostMatric(Matric matric)
        {
            matric.DateCreated = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool isTrue = db.Matrics.Any(s=>s.MatricName == matric.MatricName.Trim());
            if(isTrue==false)
            {
                db.Matrics.Add(matric);
                await db.SaveChangesAsync();
            }
           

            return CreatedAtRoute("DefaultApi", new { id = matric.MatricId }, matric);
        }

        // DELETE: api/Matrics/5
        [ResponseType(typeof(Matric))]
        public async Task<IHttpActionResult> DeleteMatric(int id)
        {
            Matric matric = await db.Matrics.FindAsync(id);
            if (matric == null)
            {
                return NotFound();
            }

            db.Matrics.Remove(matric);
            await db.SaveChangesAsync();

            return Ok(matric);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MatricExists(int id)
        {
            return db.Matrics.Count(e => e.MatricId == id) > 0;
        }
    }
}