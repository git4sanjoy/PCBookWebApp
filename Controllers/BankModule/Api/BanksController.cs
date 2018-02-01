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
using PCBookWebApp.Models.BookViewModel;
using PCBookWebApp.Models.ViewModels;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.BankModule;

namespace PCBookWebApp.Controllers.BankModule.Api
{
    public class BanksController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/Banks
        public IQueryable<Bank> GetBanks()
        {
            return db.Banks;
        }


        // GET: api/Banks/GetBanksList
        [Route("api/Banks/GetBanksList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetProjectsList()
        {
            var list = db.Banks.Select(b => new { id = b.BankId, name = b.BankName, Address = b.Address, Email = b.Email, Phone = b.Phone, Website = b.Website });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        // GET: api/Banks/5
        [ResponseType(typeof(Bank))]
        public async Task<IHttpActionResult> GetBank(int id)
        {
            Bank bank = await db.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }

            return Ok(bank);
        }

        // PUT: api/Banks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBank(int id, Bank bank)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            var dateCreated = db.Banks
                .Where(a => a.BankId == id)
                .Select(a => a.DateCreated)
                .FirstOrDefault();

            bank.CreatedBy = userName;
            bank.DateCreated = dateCreated;
            bank.DateUpdated = createdAt;


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bank.BankId)
            {
                return BadRequest();
            }

            db.Entry(bank).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankExists(id))
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

        // POST: api/Banks
        [ResponseType(typeof(Bank))]
        public async Task<IHttpActionResult> PostBank(Bank bank)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            bank.CreatedBy = userName;
            bank.DateCreated = createdAt;
            bank.DateUpdated = createdAt;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Banks.Add(bank);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = bank.BankId }, bank);
        }

        // DELETE: api/Banks/5
        [ResponseType(typeof(Bank))]
        public async Task<IHttpActionResult> DeleteBank(int id)
        {
            Bank bank = await db.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }

            db.Banks.Remove(bank);
            await db.SaveChangesAsync();

            return Ok(bank);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BankExists(int id)
        {
            return db.Banks.Count(e => e.BankId == id) > 0;
        }
    }
}