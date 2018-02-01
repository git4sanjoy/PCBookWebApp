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
using System.Data.SqlClient;
using System.Configuration;
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Controllers.BookModule.api
{
    public class VoucherDetailsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/VoucherDetails
        public IQueryable<VoucherDetail> GetVoucherDetails()
        {
            return db.VoucherDetails;
        }

        // GET: api/VoucherDetails/5
        [ResponseType(typeof(VoucherDetail))]
        public async Task<IHttpActionResult> GetVoucherDetail(int id)
        {
            VoucherDetail voucherDetail = await db.VoucherDetails.FindAsync(id);
            if (voucherDetail == null)
            {
                return NotFound();
            }

            return Ok(voucherDetail);
        }

        // PUT: api/VoucherDetails/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVoucherDetail(int id, VoucherDetail voucherDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != voucherDetail.VoucherDetailId)
            {
                return BadRequest();
            }

            db.Entry(voucherDetail).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoucherDetailExists(id))
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

        // POST: api/VoucherDetails
        [ResponseType(typeof(VoucherDetail))]
        public async Task<IHttpActionResult> PostVoucherDetail(VoucherDetail[] voucherDetail)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            //db.VoucherDetails.Add(voucherDetail);
            //await db.SaveChangesAsync();

            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            string currentUserId = User.Identity.GetUserId();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            foreach (var item in voucherDetail)
            {
                //var aLedgerObj = db.Ledgers.Where(x => x.LedgerId == item.LedgerId).FirstOrDefault();

                int trialBalanceId = 0;
                int bookId = 0;
                // Get a Record
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
                connection.Open();
                try
                {
                    SqlDataReader reader = null;
                    string sql = @"SELECT dbo.Ledgers.* FROM   dbo.Ledgers WHERE LedgerId=@ledgerId";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@ledgerId", SqlDbType.Int).Value = item.LedgerId;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        trialBalanceId = (int)reader["TrialBalanceId"];
                        bookId = (int)reader["BookId"];
                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                // End 
                item.TrialBalanceId = trialBalanceId;
                item.BookId = bookId;

                item.CreatedBy = userName;
                item.DateCreated = createdAt;
                item.DateUpdated = createdAt;
                //item.ShowRoomId = showRoomId;
                db.VoucherDetails.Add(item);
                await db.SaveChangesAsync();
            }
            return CreatedAtRoute("DefaultApi", new { status = "ok" }, voucherDetail);
        }

        // DELETE: api/VoucherDetails/5
        [ResponseType(typeof(VoucherDetail))]
        public async Task<IHttpActionResult> DeleteVoucherDetail(int id)
        {
            VoucherDetail voucherDetail = await db.VoucherDetails.FindAsync(id);
            if (voucherDetail == null)
            {
                return NotFound();
            }

            db.VoucherDetails.Remove(voucherDetail);
            await db.SaveChangesAsync();

            return Ok(voucherDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VoucherDetailExists(int id)
        {
            return db.VoucherDetails.Count(e => e.VoucherDetailId == id) > 0;
        }
    }
}