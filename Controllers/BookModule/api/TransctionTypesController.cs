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
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Controllers.BookModule.api
{
    [Authorize]
    public class TransctionTypesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/TransctionTypes/GetTransctionTypeDdlList")]
        [HttpGet]
        [ResponseType(typeof(TransctionType))]
        public IHttpActionResult GetMatricsList()
        {
            string userName = User.Identity.GetUserName();
            var list = db.TransctionTypes
                .Select(e => new { TransctionTypeId = e.TransctionTypeId, TransctionTypeName = e.TransctionTypeName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/TransctionTypes/GetTransctionTypeList")]
        [HttpGet]
        public IHttpActionResult GetTransctionTypeList()
        {
            string userName = User.Identity.GetUserName();
            var list = db.TransctionTypes
                //.Where(a => a.CreatedBy == userName)
                .Select(e => new { id = e.TransctionTypeId, name = e.TransctionTypeName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // GET: api/TransctionTypes
        public IQueryable<TransctionType> GetTransctionTypes()
        {
            return db.TransctionTypes;
        }

        // GET: api/TransctionTypes/5
        [ResponseType(typeof(TransctionType))]
        public async Task<IHttpActionResult> GetTransctionType(int id)
        {
            TransctionType transctionType = await db.TransctionTypes.FindAsync(id);
            if (transctionType == null)
            {
                return NotFound();
            }

            return Ok(transctionType);
        }

        // PUT: api/TransctionTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTransctionType(int id, TransctionType transctionType)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            transctionType.CreatedBy = userName;
            transctionType.DateCreated = createdAt;
            transctionType.DateUpdated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transctionType.TransctionTypeId)
            {
                return BadRequest();
            }

            db.Entry(transctionType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransctionTypeExists(id))
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

        // POST: api/TransctionTypes
        [ResponseType(typeof(TransctionType))]
        public async Task<IHttpActionResult> PostTransctionType(TransctionType transctionType)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            transctionType.CreatedBy = userName;
            transctionType.DateCreated = createdAt;
            transctionType.DateUpdated = createdAt;

            if (db.TransctionTypes.Any(m => m.TransctionTypeName == transctionType.TransctionTypeName && m.CreatedBy == userName))
            {
                ModelState.AddModelError("TransctionTypeName", "Transction Type Name Already Exists!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TransctionTypes.Add(transctionType);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = transctionType.TransctionTypeId }, transctionType);
        }

        // DELETE: api/TransctionTypes/5
        [ResponseType(typeof(TransctionType))]
        public async Task<IHttpActionResult> DeleteTransctionType(int id)
        {
            TransctionType transctionType = await db.TransctionTypes.FindAsync(id);
            if (transctionType == null)
            {
                return NotFound();
            }

            db.TransctionTypes.Remove(transctionType);
            await db.SaveChangesAsync();

            return Ok(transctionType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransctionTypeExists(int id)
        {
            return db.TransctionTypes.Count(e => e.TransctionTypeId == id) > 0;
        }
    }
}