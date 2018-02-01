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
using PCBookWebApp.Models.SalesModule;
using PCBookWebApp.Models.ViewModels;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.SalesModule.ViewModel;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    public class PaymentsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Payments/GetPaymentList/{FromDate}/{ToDate}/{CustomerId}")]
        [HttpGet]
        [ResponseType(typeof(PaymentView))]
        public IHttpActionResult GetPaymentList( string FromDate, string ToDate, int? CustomerId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            DateTime fdate = DateTime.Parse(FromDate);
            DateTime tdate = DateTime.Parse(ToDate);
            List<PaymentView> list = new List<PaymentView>();
            PaymentView aObj = new PaymentView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = "";
            if (CustomerId > 0)
            {
                queryString = @"SELECT        
                                    dbo.Payments.PaymentId, dbo.Payments.MemoMasterId, dbo.Payments.CustomerId, dbo.Payments.ShowRoomId, dbo.Payments.PaymentDate, dbo.Payments.SSAmount, dbo.Payments.TSAmount, 
                                    dbo.Payments.SCAmount, dbo.Payments.TCAmount, dbo.Payments.SDiscount, dbo.Payments.TDiscount, dbo.Payments.Active, dbo.Payments.CreatedBy, dbo.Customers.CustomerName, 
                                    dbo.Payments.PaymentType, dbo.Payments.HonourDate, dbo.Payments.CheckNo, dbo.Payments.BankAccountNo, dbo.Payments.Remarks, dbo.Payments.DateCreated, dbo.Payments.DateUpdated
                                    FROM            
                                    dbo.Payments INNER JOIN
                                    dbo.Customers ON dbo.Payments.CustomerId = dbo.Customers.CustomerId
                                    WHERE        
                                    (dbo.Payments.ShowRoomId = @showRoomId) AND (dbo.Payments.CustomerId = @customerId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate)";
            }
            else {
                queryString = @"SELECT        
                                    dbo.Payments.PaymentId, dbo.Payments.MemoMasterId, dbo.Payments.CustomerId, dbo.Payments.ShowRoomId, dbo.Payments.PaymentDate, dbo.Payments.SSAmount, dbo.Payments.TSAmount, 
                                    dbo.Payments.SCAmount, dbo.Payments.TCAmount, dbo.Payments.SDiscount, dbo.Payments.TDiscount, dbo.Payments.Active, dbo.Payments.CreatedBy, dbo.Customers.CustomerName, 
                                    dbo.Payments.PaymentType, dbo.Payments.HonourDate, dbo.Payments.CheckNo, dbo.Payments.BankAccountNo, dbo.Payments.Remarks, dbo.Payments.DateCreated, dbo.Payments.DateUpdated
                                    FROM            
                                    dbo.Payments INNER JOIN
                                    dbo.Customers ON dbo.Payments.CustomerId = dbo.Customers.CustomerId
                                    WHERE        
                                    (dbo.Payments.ShowRoomId = @showRoomId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate)";
            }


            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                if (CustomerId > 0) {
                    command.Parameters.Add(new SqlParameter("@customerId", CustomerId));
                }                    
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        aObj = new PaymentView();
                        aObj.CustomerName = (string)reader["CustomerName"]; 
                        aObj.PaymentDate = (DateTime)reader["PaymentDate"]; 
                        aObj.SCAmount = (double) reader["SCAmount"];
                        aObj.SDiscount = (double) reader["SDiscount"];
                        aObj.PaymentType = (string)reader["PaymentType"];
                        if (reader["CheckNo"] != DBNull.Value) {
                            aObj.CheckNo = (string)reader["CheckNo"];
                        }
                        if (reader["BankAccountNo"] != DBNull.Value)
                        {
                            aObj.BankAccountNo = (string)reader["BankAccountNo"];
                        }
                        if (reader["HonourDate"] != DBNull.Value)
                        {
                            aObj.HonourDate = (DateTime) reader["HonourDate"];
                        }
                        if (reader["Remarks"] != DBNull.Value)
                        {
                            aObj.Remarks = (string)reader["Remarks"];
                        }
                        list.Add(aObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(list);
        }



        
        [Route("api/Payments/GetDateBetweenPaymentsSum/{FromDate}/{ToDate}/{CustomerId}")]
        [HttpGet]
        [ResponseType(typeof(PaymentView))]
        public IHttpActionResult GetDateBetweenPaymentsSum(string FromDate, string ToDate, int? CustomerId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            DateTime fdate = DateTime.Parse(FromDate);
            DateTime tdate = DateTime.Parse(ToDate);
            List<PaymentView> list = new List<PaymentView>();
            PaymentView aObj = new PaymentView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = "";
            if (CustomerId > 0)
            {
                queryString = @"SELECT        
                                    SUM(SSAmount) AS SSAmount, SUM(TSAmount) AS TSAmount, SUM(SCAmount) AS SCAmount, SUM(TCAmount) AS TCAmount, SUM(SDiscount) AS SDiscount, SUM(TDiscount) AS TDiscount
                                    FROM            
                                    dbo.Payments
                                    WHERE        
                                    (dbo.Payments.CustomerId = @customerId) AND (dbo.Payments.ShowRoomId = @showRoomId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate)";
            }
            else {
                queryString = @"SELECT        
                                    SUM(SSAmount) AS SSAmount, SUM(TSAmount) AS TSAmount, SUM(SCAmount) AS SCAmount, SUM(TCAmount) AS TCAmount, SUM(SDiscount) AS SDiscount, SUM(TDiscount) AS TDiscount
                                    FROM            
                                    dbo.Payments
                                    WHERE        
                                    (dbo.Payments.ShowRoomId = @showRoomId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate)";
            }

            


            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                if (CustomerId > 0) {
                    command.Parameters.Add(new SqlParameter("@customerId", CustomerId));
                }
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        aObj = new PaymentView();
                        if (reader["SCAmount"] != DBNull.Value) {
                            aObj.SCAmount = (double)reader["SCAmount"];
                        }
                        if (reader["SCAmount"] != DBNull.Value)
                        {
                            aObj.SDiscount = (double)reader["SDiscount"];
                        }
                        
                        list.Add(aObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(list);
        }



        // GET: api/Payments/GetDropDownListXedit/
        [Route("api/Payments/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(MainCategory))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var unitList = db.Customers.Select(e => new { id = e.CustomerId, text = e.CustomerName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }
        // GET: api/Payments
        public IQueryable<Payment> GetPayments()
        {
            return db.Payments;
        }

        // GET: api/Payments/5
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> GetPayment(int id)
        {
            Payment payment = await db.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        // PUT: api/Payments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPayment(int id, Payment payment)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            payment.ShowRoomId = showRoomId;
            payment.DateCreated = ceatedAt;
            payment.DateUpdated = ceatedAt;
            payment.CreatedBy = userName;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != payment.PaymentId)
            {
                return BadRequest();
            }

            db.Entry(payment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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

        // POST: api/Payments
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> PostPayment(Payment payment)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            payment.ShowRoomId = showRoomId;
            payment.DateCreated = ceatedAt;
            payment.DateUpdated = ceatedAt;
            payment.CreatedBy = userName;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Payments.Add(payment);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = payment.PaymentId }, payment);
        }

        // DELETE: api/Payments/5
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> DeletePayment(int id)
        {
            Payment payment = await db.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            db.Payments.Remove(payment);
            await db.SaveChangesAsync();

            return Ok(payment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentExists(int id)
        {
            return db.Payments.Count(e => e.PaymentId == id) > 0;
        }
    }
}