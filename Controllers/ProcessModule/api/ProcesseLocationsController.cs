using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PCBookWebApp.DAL;
using PCBookWebApp.Models.ProcessModule;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    //[Authorize]
    public class ProcesseLocationsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/ProcesseLocations
        public IQueryable<ProcesseLocation> GetProcesseLocations()
        {
            return db.ProcesseLocations;
        }

        // GET: api/ProcesseLocations/5
        [ResponseType(typeof(ProcesseLocation))]
        public IHttpActionResult GetProcesseLocation(int id)
        {
            ProcesseLocation processeLocation = db.ProcesseLocations.Find(id);
            if (processeLocation == null)
            {
                return NotFound();
            }

            return Ok(processeLocation);
        }

        // PUT: api/ProcesseLocations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProcesseLocation(int id, ProcesseLocation processeLocation)
        {
            processeLocation.DateCreated = db.ProcesseLocations.Where(x => x.ProcesseLocationId == id).Select(x => x.DateCreated).FirstOrDefault();
            processeLocation.DateUpdated = DateTime.Now;
            processeLocation.Active = true;
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != processeLocation.ProcesseLocationId)
            {
                return BadRequest();
            }

            db.Entry(processeLocation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcesseLocationExists(id))
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

        // POST: api/ProcesseLocations
        [ResponseType(typeof(ProcesseLocation))]
        public IHttpActionResult PostProcesseLocation(ProcesseLocation processeLocation)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            processeLocation.DateCreated = DateTime.Now;
            db.ProcesseLocations.Add(processeLocation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = processeLocation.ProcesseLocationId }, processeLocation);
        }

        // DELETE: api/ProcesseLocations/5
        [ResponseType(typeof(ProcesseLocation))]
        public IHttpActionResult DeleteProcesseLocation(int id)
        {
            ProcesseLocation processeLocation = db.ProcesseLocations.Find(id);
            if (processeLocation == null)
            {
                return NotFound();
            }

            db.ProcesseLocations.Remove(processeLocation);
            db.SaveChanges();

            return Ok(processeLocation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProcesseLocationExists(int id)
        {
            return db.ProcesseLocations.Count(e => e.ProcesseLocationId == id) > 0;
        }
    }
}