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
using PCBookWebApp.Models.ViewModels;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Controllers.BookModule.api
{
    public class ProvisionsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        [Route("api/Provisions/GetLedgerXeditList")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetLedgerXeditList()
        {
            var unitList = db.Ledgers.Select(e => new { id = e.LedgerId, text = e.LedgerName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }

        //Custom Method
        [Route("api/Provisions/GetProvisionList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetProvisionList()
        {
            List<XEditGroupView> ImportProductList = new List<XEditGroupView>();
            XEditGroupView importProduct = new XEditGroupView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.Provisions.ProvisionId AS id, dbo.Provisions.LedgerId AS [group], dbo.Ledgers.LedgerName AS groupName, dbo.Provisions.OpeningAmount, dbo.Provisions.ProvisionAmount, dbo.Provisions.ActualAmount, 
                                    dbo.Provisions.ShowRoomId, dbo.Provisions.ProvisionDate
                                    FROM            
                                    dbo.Provisions INNER JOIN
                                    dbo.Ledgers ON dbo.Provisions.LedgerId = dbo.Ledgers.LedgerId";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        //string name = (string)reader["name"];
                        int group = (int)reader["group"];
                        string groupName = (string)reader["groupName"];

                        importProduct = new XEditGroupView();
                        importProduct.id = id;
                        //importProduct.name = name;
                        importProduct.group = group;
                        importProduct.groupName = groupName;
                        if (reader["OpeningAmount"] != System.DBNull.Value)
                        {
                            importProduct.OpeningAmount = (double) reader["OpeningAmount"];
                        }
                        if (reader["ProvisionAmount"] != System.DBNull.Value)
                        {
                            importProduct.ProvisionAmount = (double)reader["ProvisionAmount"];
                        }
                        if (reader["ActualAmount"] != System.DBNull.Value)
                        {
                            importProduct.ActualAmount =(double) reader["ActualAmount"];
                        }

                        ImportProductList.Add(importProduct);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(ImportProductList);
        }

        [Route("api/Provisions/GetProvisionListByLastDate")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetProvisionListByLastDate()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            //var provisionDate = (from o in db.Provisions
            //             select new
            //             {
            //                 o.ProvisionDate
            //             }).Distinct().OrderByDescending(x => x.ProvisionDate);

            DateTime provisionDate = DateTime.Parse("2000-01-01");
            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT DISTINCT TOP (1) ProvisionDate FROM dbo.Provisions WHERE ShowRoomId = @showRoomId ORDER BY ProvisionDate DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        provisionDate = (DateTime)reader["ProvisionDate"];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            List<XEditGroupView> ImportProductList = new List<XEditGroupView>();
            XEditGroupView importProduct = new XEditGroupView();

            string queryString1 = @"SELECT        
                                    dbo.Provisions.ProvisionId AS id, dbo.Provisions.LedgerId AS [group], dbo.Ledgers.LedgerName AS groupName, dbo.Provisions.OpeningAmount, dbo.Provisions.ProvisionAmount, dbo.Provisions.ActualAmount, 
                                    dbo.Provisions.ShowRoomId, dbo.Provisions.ProvisionDate
                                    FROM            
                                    dbo.Provisions INNER JOIN
                                    dbo.Ledgers ON dbo.Provisions.LedgerId = dbo.Ledgers.LedgerId
                                    Where ProvisionDate = @provisionDate";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString1, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@provisionDate", provisionDate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        //string name = (string)reader["name"];
                        int group = (int)reader["group"];
                        string groupName = (string)reader["groupName"];
                        importProduct = new XEditGroupView();
                        importProduct.id = id;
                        //importProduct.name = name;
           

                        DateTime fromDate = provisionDate.AddMonths(1);
                        DateTime toDate = provisionDate.AddMonths(2).AddDays(-1);
                        double actualAmount = 0;

                        string queryStringPaymentSum = @"SELECT        
                                                            dbo.VoucherDetails.LedgerId, SUM(dbo.VoucherDetails.DrAmount) AS DrAmount, SUM(dbo.VoucherDetails.CrAmount) AS CrAmount
                                                            FROM            
                                                            dbo.Vouchers INNER JOIN
                                                            dbo.VoucherDetails ON dbo.Vouchers.VoucherId = dbo.VoucherDetails.VoucherId
                                                            WHERE        
                                                            (dbo.Vouchers.VoucherDate >= CONVERT(DATETIME, @fromDate, 102)) AND (dbo.Vouchers.VoucherDate <= CONVERT(DATETIME, @toDate, 102)) AND (dbo.Vouchers.ShowRoomId = @showRoomId)
                                                            GROUP BY dbo.VoucherDetails.LedgerId
                                                            HAVING        
                                                            (dbo.VoucherDetails.LedgerId = @ledgerId)";
                        using (SqlConnection connectionSum = new SqlConnection(connectionString))
                        {
                            SqlCommand commandSum = new SqlCommand(queryStringPaymentSum, connectionSum);
                            connectionSum.Open();
                            commandSum.Parameters.Add(new SqlParameter("@fromDate", fromDate));
                            commandSum.Parameters.Add(new SqlParameter("@toDate", toDate));
                            commandSum.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                            commandSum.Parameters.Add(new SqlParameter("@ledgerId", group));
                            SqlDataReader readerSum = commandSum.ExecuteReader();
                            try
                            {
                                while (readerSum.Read())
                                {
                                    double sumDrAmount = (double)readerSum["DrAmount"];
                                    double sumCrAmount = (double)readerSum["CrAmount"];
                                    actualAmount = sumDrAmount - sumCrAmount;
                                }
                            }
                            finally
                            {
                                readerSum.Close();
                            }
                        }

                        importProduct.group = group;
                        importProduct.groupName = groupName;
                        if (reader["OpeningAmount"] != System.DBNull.Value)
                        {
                            importProduct.OpeningAmount = (double)reader["OpeningAmount"];
                        }
                        if (reader["ProvisionAmount"] != System.DBNull.Value)
                        {
                            importProduct.ProvisionAmount = (double)reader["ProvisionAmount"];
                        }
                        importProduct.ActualAmount = actualAmount;
                        importProduct.ClosingAmount = 0;
                        ImportProductList.Add(importProduct);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(ImportProductList);
        }

        // GET: api/Provisions
        public IQueryable<Provision> GetProvisions()
        {
            return db.Provisions;
        }

        // GET: api/Provisions/5
        [ResponseType(typeof(Provision))]
        public async Task<IHttpActionResult> GetProvision(int id)
        {
            Provision provision = await db.Provisions.FindAsync(id);
            if (provision == null)
            {
                return NotFound();
            }

            return Ok(provision);
        }

        // PUT: api/Provisions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProvision(int id, Provision provision)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            provision.CreatedBy = userName;
            provision.DateCreated = createdAt;
            provision.DateUpdated = createdAt;
            provision.ShowRoomId = showRoomId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != provision.ProvisionId)
            {
                return BadRequest();
            }

            db.Entry(provision).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProvisionExists(id))
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

        // POST: api/Provisions
        [ResponseType(typeof(Provision))]
        public async Task<IHttpActionResult> PostProvision(Provision[] provision)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //db.Provisions.Add(provision);
            //await db.SaveChangesAsync();
            foreach (var item in provision)
            {
                item.CreatedBy = userName;
                item.DateCreated = createdAt;
                item.DateUpdated = createdAt;
                item.ShowRoomId = showRoomId;
                db.Provisions.Add(item);
                await db.SaveChangesAsync();
            }
            //return CreatedAtRoute("DefaultApi", new { id = provision.ProvisionId }, provision);
            return CreatedAtRoute("DefaultApi", new { status = "ok" }, provision);
        }

        // DELETE: api/Provisions/5
        [ResponseType(typeof(Provision))]
        public async Task<IHttpActionResult> DeleteProvision(int id)
        {
            Provision provision = await db.Provisions.FindAsync(id);
            if (provision == null)
            {
                return NotFound();
            }

            db.Provisions.Remove(provision);
            await db.SaveChangesAsync();

            return Ok(provision);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProvisionExists(int id)
        {
            return db.Provisions.Count(e => e.ProvisionId == id) > 0;
        }
    }
}