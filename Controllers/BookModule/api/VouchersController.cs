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
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.BookModule.BookViewModel;
using PCBookWebApp.Models.BookModule;
using System.Data.Entity.Migrations;

namespace PCBookWebApp.Controllers.BookModule.api
{
    public class VouchersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Vouchers/GetNewVoucherId/{date}/{VoucherTypeId}")]
        [HttpGet]
        [ResponseType(typeof(Voucher))]
        public IHttpActionResult GetNewVoucherId(string date, int VoucherTypeId)
        {
            DateTime bdate = DateTime.Parse(date);
            string currentMonth = bdate.Month.ToString();
            string currentYear = bdate.Year.ToString();

            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            Voucher voucherObj = new Voucher();
            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    CAST(ISNULL(MAX(RIGHT(VoucherNo, 4)), 0) + 1 AS INT) AS NewId, YEAR(VoucherDate) AS Year, MONTH(VoucherDate) AS Month, VoucherTypeId, ShowRoomId
                                    FROM            
                                    dbo.Vouchers
                                    GROUP BY YEAR(VoucherDate), MONTH(VoucherDate), VoucherTypeId, ShowRoomId
                                    HAVING        
                                    (YEAR(VoucherDate) = @year) AND (MONTH(VoucherDate) = @month) AND (VoucherTypeId = @voucherTypeId) AND (ShowRoomId = @showRoomId)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@year", currentYear));
                command.Parameters.Add(new SqlParameter("@month", currentMonth));
                command.Parameters.Add(new SqlParameter("@voucherTypeId", VoucherTypeId));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        voucherObj = new Voucher();
                        if (reader["NewId"] != DBNull.Value)
                        {
                            voucherObj.VoucherId = (int)reader["NewId"];
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(voucherObj);
        }


        // GET: api/Vouchers
        public IQueryable<Voucher> GetVouchers()
        {
            return db.Vouchers;
        }

        // GET: api/Vouchers/5
        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> GetVoucher(int id)
        {
            Voucher voucher = await db.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            return Ok(voucher);
        }
        [Route("api/Vouchers/GetVoucherById/{VouchrTypeId}/{VoucherNo}")]
        [HttpGet]
        [ResponseType(typeof(Voucher))]
        public IHttpActionResult GetVoucherById(int VouchrTypeId, string VoucherNo)
        {
            string userId = User.Identity.GetUserId();
            var showRoom = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => new { a.ShowRoomId, ShowRoomName = a.ShowRoom.ShowRoomName })
                .FirstOrDefault();

            var voucher = db.Vouchers
                .Where(v => v.VoucherTypeId== VouchrTypeId && v.VoucherNo == VoucherNo && v.ShowRoomId == showRoom.ShowRoomId)
                .FirstOrDefault();
            if (voucher != null)
            {
                List<VoucherView> list = new List<VoucherView>();
                VoucherView voucherObj = new VoucherView();
                string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
                string queryString = @"SELECT        
                                    dbo.Vouchers.VoucherId, dbo.Vouchers.VoucherDate, dbo.Vouchers.VoucherNo, dbo.Vouchers.Naration, dbo.VoucherTypes.VoucherTypeName, dbo.TransctionTypes.TransctionTypeName, 
                                    dbo.Ledgers.LedgerName, dbo.VoucherDetails.DrAmount, dbo.VoucherDetails.CrAmount, dbo.Vouchers.IsBank, dbo.Vouchers.IsHonored, dbo.Vouchers.HonoredDate, dbo.Vouchers.Authorized, 
                                    dbo.Vouchers.AuthorizedBy, dbo.VoucherDetails.ReceiveOrPayment, dbo.VoucherDetails.CheckId, dbo.VoucherDetails.VoucherDetailId
                                    FROM            
                                    dbo.Vouchers INNER JOIN
                                    dbo.VoucherDetails ON dbo.Vouchers.VoucherId = dbo.VoucherDetails.VoucherId INNER JOIN
                                    dbo.VoucherTypes ON dbo.Vouchers.VoucherTypeId = dbo.VoucherTypes.VoucherTypeId INNER JOIN
                                    dbo.TransctionTypes ON dbo.VoucherDetails.TransctionTypeId = dbo.TransctionTypes.TransctionTypeId INNER JOIN
                                    dbo.Ledgers ON dbo.VoucherDetails.LedgerId = dbo.Ledgers.LedgerId WHERE (dbo.Vouchers.VoucherId = @voucherId)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    command.Parameters.Add(new SqlParameter("@voucherId", voucher.VoucherId));
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            voucherObj = new VoucherView();
                            voucherObj.VoucherId =(int)reader["VoucherId"];
                            voucherObj.ShowRoomName = (string) showRoom.ShowRoomName;
                            voucherObj.TransctionTypeName = (string)reader["TransctionTypeName"];
                            voucherObj.VoucherNo = (string)reader["VoucherNo"];
                            voucherObj.Naration = (string)reader["Naration"];
                            voucherObj.VoucherTypeName = (string)reader["VoucherTypeName"];
                            voucherObj.TransctionTypeName = (string)reader["TransctionTypeName"];
                            voucherObj.LedgerName = (string)reader["LedgerName"];
                            voucherObj.DrAmount = (double)reader["DrAmount"];
                            voucherObj.CrAmount = (double)reader["CrAmount"];
                            voucherObj.IsBank = (bool)reader["IsBank"];
                            voucherObj.ReceiveOrPayment = (bool)reader["ReceiveOrPayment"];
                            voucherObj.VoucherDate = (DateTime)reader["VoucherDate"];
                            if (reader["HonoredDate"] != DBNull.Value)
                            {
                                voucherObj.HonoredDate = (DateTime)reader["HonoredDate"];
                            }
                            list.Add(voucherObj);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                return Ok(list);
            }
            else {
                return Ok("No Record Found");
            }

            // return Ok("No Record Found");
        }

        [Route("api/Vouchers/GetVoucherByDate/{date}")]
        [HttpGet]
        [ResponseType(typeof(Voucher))]
        public IHttpActionResult GetVoucherByDate(string date)
        {
            string userId = User.Identity.GetUserId();
            var showRoom = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => new { a.ShowRoomId, ShowRoomName = a.ShowRoom.ShowRoomName })
                .FirstOrDefault();
            DateTime fdate = DateTime.Parse(date);
            var voucher = db.Vouchers
                .Where(v => v.VoucherDate == fdate && v.ShowRoomId == showRoom.ShowRoomId)
                .FirstOrDefault();
            if (voucher != null)
            {
                List<VoucherView> list = new List<VoucherView>();
                VoucherView voucherObj = new VoucherView();
                string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
                string queryString = @"SELECT        
                                    dbo.Vouchers.VoucherId, dbo.Vouchers.VoucherDate, dbo.Vouchers.VoucherNo, dbo.Vouchers.Naration, dbo.VoucherTypes.VoucherTypeName, dbo.TransctionTypes.TransctionTypeName, 
                                    dbo.Ledgers.LedgerName, dbo.VoucherDetails.DrAmount, dbo.VoucherDetails.CrAmount, dbo.Vouchers.IsBank, dbo.Vouchers.IsHonored, dbo.Vouchers.HonoredDate, dbo.Vouchers.Authorized, 
                                    dbo.Vouchers.AuthorizedBy, dbo.VoucherDetails.ReceiveOrPayment, dbo.VoucherDetails.CheckId, dbo.VoucherDetails.VoucherDetailId
                                    FROM            
                                    dbo.Vouchers INNER JOIN
                                    dbo.VoucherDetails ON dbo.Vouchers.VoucherId = dbo.VoucherDetails.VoucherId INNER JOIN
                                    dbo.VoucherTypes ON dbo.Vouchers.VoucherTypeId = dbo.VoucherTypes.VoucherTypeId INNER JOIN
                                    dbo.TransctionTypes ON dbo.VoucherDetails.TransctionTypeId = dbo.TransctionTypes.TransctionTypeId INNER JOIN
                                    dbo.Ledgers ON dbo.VoucherDetails.LedgerId = dbo.Ledgers.LedgerId WHERE (dbo.Vouchers.VoucherDate = @voucherDate)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    command.Parameters.Add(new SqlParameter("@voucherDate", fdate));
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            voucherObj = new VoucherView();
                            voucherObj.VoucherId = (int)reader["VoucherId"];
                            voucherObj.VoucherDetailId = (int)reader["VoucherDetailId"];
                            voucherObj.ShowRoomName = (string)showRoom.ShowRoomName;
                            voucherObj.TransctionTypeName = (string)reader["TransctionTypeName"];
                            voucherObj.VoucherNo = (string)reader["VoucherNo"];
                            voucherObj.Naration = (string)reader["Naration"];
                            voucherObj.VoucherTypeName = (string)reader["VoucherTypeName"];
                            voucherObj.TransctionTypeName = (string)reader["TransctionTypeName"];
                            voucherObj.LedgerName = (string)reader["LedgerName"];
                            voucherObj.DrAmount = (double)reader["DrAmount"];
                            voucherObj.CrAmount = (double)reader["CrAmount"];
                            voucherObj.IsBank = (bool)reader["IsBank"];
                            voucherObj.ReceiveOrPayment = (bool)reader["ReceiveOrPayment"];
                            voucherObj.VoucherDate = (DateTime)reader["VoucherDate"];
                            if (reader["HonoredDate"] != DBNull.Value)
                            {
                                voucherObj.HonoredDate = (DateTime)reader["HonoredDate"];
                            }
                            list.Add(voucherObj);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                return Ok(list);
            }
            else
            {
                return Ok("No Record Found");
            }

            // return Ok("No Record Found");
        }
        [Route("api/Vouchers/RateMgrUpdateRate/{id}")]
        [HttpPut]
        public IHttpActionResult RateMgrUpdateRate(int id)
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(u => u.Id == currentUserId).Select(u => u.ShowRoomId).FirstOrDefault();
            var unitId = db.ShowRooms .Where(u => u.ShowRoomId == showRoomId).Select(u => u.UnitId).FirstOrDefault();

            var aVoucher = db.Vouchers.Where(v => v.VoucherId == id && v.ShowRoomId == showRoomId).FirstOrDefault();

            if (aVoucher.IsHonored == false)
            {
                aVoucher.IsHonored = true;
                aVoucher.HonoredDate = DateTime.Now.Date;
                aVoucher.AuthorizedBy = currentUserName;
                db.Vouchers.AddOrUpdate(aVoucher);
                db.SaveChanges();
            }
            else {
                aVoucher.IsHonored = false;
                aVoucher.HonoredDate = DateTime.Now.Date;
                aVoucher.AuthorizedBy = currentUserName;
                db.Vouchers.AddOrUpdate(aVoucher);
                db.SaveChanges();
            }
            return Ok();
        }
        // PUT: api/Vouchers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVoucher(int id, Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != voucher.VoucherId)
            {
                return BadRequest();
            }

            db.Entry(voucher).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoucherExists(id))
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

        // POST: api/Vouchers
        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> PostVoucher(Voucher voucher)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            voucher.ShowRoomId = showRoomId;

            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            voucher.CreatedBy = userName;
            voucher.DateCreated = createdAt;
            voucher.DateUpdated = createdAt;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Vouchers.Add(voucher);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = voucher.VoucherId }, voucher);
        }

        // DELETE: api/Vouchers/5
        [ResponseType(typeof(Voucher))]
        public async Task<IHttpActionResult> DeleteVoucher(int id)
        {
            Voucher voucher = await db.Vouchers.FindAsync(id);          

            int voucherId = voucher.VoucherId;
            var checkBookPage = db.Checks
                               .Where(e => e.VoucherId == voucherId)
                               .ToList()
                               .FirstOrDefault();

            
            if (checkBookPage != null)
            {
                var aCheckBookPage = db.CheckBookPages.Where(x => x.CheckBookPageId == checkBookPage.CheckBookPageId).FirstOrDefault();
                aCheckBookPage.Active = false;
                db.Entry(aCheckBookPage).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (voucher == null)
            {
                return NotFound();
            }

            db.Vouchers.Remove(voucher);
            await db.SaveChangesAsync();

            return Ok(voucher);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VoucherExists(int id)
        {
            return db.Vouchers.Count(e => e.VoucherId == id) > 0;
        }
    }
}