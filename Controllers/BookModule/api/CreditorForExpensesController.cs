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
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Controllers.BookModule.api
{
    public class CreditorForExpensesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/CreditorForExpenses
        public IQueryable<CreditorForExpenses> GetCreditorForExpenses()
        {
            return db.CreditorForExpenses;
        }

        // GET: api/CreditorForExpenses/5
        [ResponseType(typeof(CreditorForExpenses))]
        public async Task<IHttpActionResult> GetCreditorForExpenses(int id)
        {
            CreditorForExpenses creditorForExpenses = await db.CreditorForExpenses.FindAsync(id);
            if (creditorForExpenses == null)
            {
                return NotFound();
            }

            return Ok(creditorForExpenses);
        }

        // PUT: api/CreditorForExpenses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCreditorForExpenses(int id, CreditorForExpenses creditorForExpenses)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != creditorForExpenses.CreditorForExpensesId)
            {
                return BadRequest();
            }

            db.Entry(creditorForExpenses).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreditorForExpensesExists(id))
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

        // POST: api/CreditorForExpenses
        [ResponseType(typeof(CreditorForExpenses))]
        public async Task<IHttpActionResult> PostCreditorForExpenses(CreditorForExpenses creditorForExpenses)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CreditorForExpenses.Add(creditorForExpenses);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = creditorForExpenses.CreditorForExpensesId }, creditorForExpenses);
        }

        // DELETE: api/CreditorForExpenses/5
        [ResponseType(typeof(CreditorForExpenses))]
        public async Task<IHttpActionResult> DeleteCreditorForExpenses(int id)
        {
            CreditorForExpenses creditorForExpenses = await db.CreditorForExpenses.FindAsync(id);
            if (creditorForExpenses == null)
            {
                return NotFound();
            }

            db.CreditorForExpenses.Remove(creditorForExpenses);
            await db.SaveChangesAsync();

            return Ok(creditorForExpenses);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CreditorForExpensesExists(int id)
        {
            return db.CreditorForExpenses.Count(e => e.CreditorForExpensesId == id) > 0;
        }
    }
}