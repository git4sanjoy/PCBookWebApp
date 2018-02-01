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
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using PCBookWebApp.Models.BankModule;
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Controllers.BankModule.Api
{
    public class BankAccountsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/BankAccounts/GetBankAccountsList")]
        [HttpGet]
        public IHttpActionResult GetBankAccountsList()
        {
            string userName = User.Identity.GetUserName();
            var list = db.BankAccounts
                            .Include(a => a.Group)
                            .Include(a => a.Bank)
                            .Include(a => a.Ledger)

                            .Where(d => d.CreatedBy == userName)
                            .OrderBy(d => d.BankAccountNumber)
                            .Select(e => new {
                                id = e.BankAccountId,
                                name = e.BankAccountNumber,
                                group = e.GroupId,
                                groupName = e.Group.GroupName,
                                status = e.Bank.BankId,
                                updateDate = e.DateCreated,
                            });

            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/BankAccounts/GetBanksXEditList")]
        [HttpGet]
        [ResponseType(typeof(Bank))]
        public IHttpActionResult GetBanksXEditList()
        {
            var list = db.Banks.Select(e => new { value = e.BankId, text = e.BankName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/BankAccounts/GetGroupXEditList")]
        [HttpGet]
        [ResponseType(typeof(Group))]
        public IHttpActionResult GetGroupXEditList()
        {
            string userName = User.Identity.GetUserName();
            string currentUserId = User.Identity.GetUserId();

            var list = db.Groups
                            .Where(a => a.IsParent == false && a.PrimaryId==null && a.GroupName != "Primary")
                            .Select(e => new { id = e.GroupId, text = e.GroupName })
                            .OrderBy(e => e.text);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }





        // GET: api/BankAccounts
        public IQueryable<BankAccount> GetBankAccounts()
        {
            return db.BankAccounts;
        }

        // GET: api/BankAccounts/5
        [ResponseType(typeof(BankAccount))]
        public async Task<IHttpActionResult> GetBankAccount(int id)
        {
            BankAccount bankAccount = await db.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return Ok(bankAccount);
        }

        // PUT: api/BankAccounts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBankAccount(int id, BankAccount bankAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bankAccount.BankAccountId)
            {
                return BadRequest();
            }

            db.Entry(bankAccount).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
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

        // POST: api/BankAccounts
        [ResponseType(typeof(BankAccount))]
        public async Task<IHttpActionResult> PostBankAccount(BankAccount bankAccount)
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            DateTime createdAt = DateTime.Now;

            bankAccount.CreatedBy = userName;
            bankAccount.DateCreated = createdAt;
            bankAccount.DateUpdated = createdAt;
            bankAccount.AccountOpenDate = createdAt;

            var showRoomId = db.ShowRoomUsers
                                .Where(a => a.Id == userId)
                                .Select(a => a.ShowRoomId)
                                .FirstOrDefault();
            bankAccount.ShowRoomId = showRoomId;

            int ledgerId = (int) bankAccount.LedgerId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BankAccounts.Add(bankAccount);
            await db.SaveChangesAsync();

            //Update BankAccountId to Ledgers table
            int bankAccountId = bankAccount.BankAccountId;
            var parentLedger = db.Ledgers.Where(x => x.LedgerId == ledgerId).FirstOrDefault();
            if (parentLedger.BankAccountId == null) {
                parentLedger.BankAccountId = bankAccountId;
                db.Ledgers.AddOrUpdate(parentLedger);
                await db.SaveChangesAsync();
            }
            // End

            //return CreatedAtRoute("DefaultApi", new { id = bankAccountId }, bankAccount);
            return Ok();
        }

        // DELETE: api/BankAccounts/5
        [ResponseType(typeof(BankAccount))]
        public async Task<IHttpActionResult> DeleteBankAccount(int id)
        {
            BankAccount bankAccount = await db.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            db.BankAccounts.Remove(bankAccount);
            await db.SaveChangesAsync();

            return Ok(bankAccount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BankAccountExists(int id)
        {
            return db.BankAccounts.Count(e => e.BankAccountId == id) > 0;
        }
    }
}