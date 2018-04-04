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
    [Authorize]
    public class PaymentsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Payments/GetPaymentList/{FromDate}/{ToDate}/{CustomerId}")]
        [HttpGet]
        [ResponseType(typeof(PaymentView))]
        public IHttpActionResult GetPaymentList( string FromDate, string ToDate, int? CustomerId)
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            DateTime fdate = DateTime.Parse(FromDate);
            DateTime tdate = DateTime.Parse(ToDate);
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            if (User.IsInRole("Zone Manager"))
            {
                if (CustomerId > 0)
                {
                    var paymentList = db.Payments
                                        .Include(p => p.Customer)
                                        .Where(p => p.PaymentDate >= fdate && p.PaymentDate <= tdate && p.CustomerId == CustomerId && p.CreatedBy == userName && p.PaymentType != "Cash Party")
                                        .Select(p => new {
                                            p.CustomerId,
                                            p.Customer.CustomerName,
                                            p.PaymentDate,
                                            p.PaymentId,
                                            p.SCAmount,
                                            p.SDiscount,
                                            p.CheckNo,
                                            p.BankAccountNo,
                                            p.HonourDate,
                                            p.PaymentType,
                                            p.CreatedBy
                                        }).ToList();
                    return Ok(paymentList);
                }
                else {
                    var paymentList = db.Payments
                                        .Include(p => p.Customer)
                                        .Where(p => p.PaymentDate >= fdate && p.PaymentDate <= tdate && p.CreatedBy == userName && p.PaymentType != "Cash Party")
                                        .Select(p => new {
                                            p.CustomerId,
                                            p.Customer.CustomerName,
                                            p.PaymentDate,
                                            p.PaymentId,
                                            p.SCAmount,
                                            p.SDiscount,
                                            p.CheckNo,
                                            p.BankAccountNo,
                                            p.HonourDate,
                                            p.PaymentType,
                                            p.CreatedBy
                                        }).ToList();
                    return Ok(paymentList);
                }
            } else
            {
                if (CustomerId > 0)
                {
                    var paymentList = db.Payments
                                        .Include(p => p.Customer)
                                        .Where(p => p.PaymentDate >= fdate && p.PaymentDate <= tdate && p.CustomerId == CustomerId && p.ShowRoomId == showRoomId && p.PaymentType != "Cash Party")
                                        .Select(p => new {
                                            p.CustomerId,
                                            p.Customer.CustomerName,
                                            p.PaymentDate,
                                            p.PaymentId,
                                            p.SCAmount,
                                            p.SDiscount,
                                            p.CheckNo,
                                            p.BankAccountNo,
                                            p.HonourDate,
                                            p.PaymentType,
                                            p.CreatedBy,
                                            p.ShowRoomId
                                        }).ToList();
                    return Ok(paymentList);
                }
                else
                {
                    var paymentList = db.Payments
                                        .Include(p => p.Customer)
                                        .Where(p => p.PaymentDate >= fdate && p.PaymentDate <= tdate && p.ShowRoomId == showRoomId && p.PaymentType != "Cash Party")
                                        .Select(p => new {
                                            p.CustomerId,
                                            p.Customer.CustomerName,
                                            p.PaymentDate,
                                            p.PaymentId,
                                            p.SCAmount,
                                            p.SDiscount,
                                            p.CheckNo,
                                            p.BankAccountNo,
                                            p.HonourDate,
                                            p.PaymentType,
                                            p.CreatedBy,
                                            p.ShowRoomId
                                        }).ToList();
                    return Ok(paymentList);
                }

            }


            //List<PaymentView> list = new List<PaymentView>();
            //PaymentView aObj = new PaymentView();

            //string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            //string queryString = "";
            //if (User.IsInRole("Zone Manager")) {
            //    if (CustomerId > 0)
            //    {
            //        queryString = @"SELECT        
            //                        dbo.Payments.PaymentId, dbo.Payments.MemoMasterId, dbo.Payments.CustomerId, dbo.Payments.ShowRoomId, dbo.Payments.PaymentDate, dbo.Payments.SSAmount, dbo.Payments.SCAmount, 
            //                        dbo.Payments.SDiscount, dbo.Payments.Active, dbo.Payments.CreatedBy, dbo.Customers.CustomerName, dbo.Payments.PaymentType, dbo.Payments.HonourDate, dbo.Payments.CheckNo, 
            //                        dbo.Payments.BankAccountNo, dbo.Payments.Remarks, dbo.Payments.DateCreated, dbo.Payments.DateUpdated,dbo.Payments.CreatedBy
            //                        FROM            
            //                        dbo.Payments INNER JOIN dbo.Customers ON dbo.Payments.CustomerId = dbo.Customers.CustomerId
            //                        WHERE        
            //                        (dbo.Payments.CreatedBy = @CreatedBy) AND (dbo.Payments.CustomerId = @customerId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate) AND (dbo.Payments.PaymentType <> 'Cash Party')";
            //    }
            //    else
            //    {
            //        queryString = @"SELECT        
            //                        dbo.Payments.PaymentId, dbo.Payments.MemoMasterId, dbo.Payments.CustomerId, dbo.Payments.ShowRoomId, dbo.Payments.PaymentDate, dbo.Payments.SSAmount, dbo.Payments.SCAmount, 
            //                        dbo.Payments.SDiscount, dbo.Payments.Active, dbo.Payments.CreatedBy, dbo.Customers.CustomerName, dbo.Payments.PaymentType, dbo.Payments.HonourDate, dbo.Payments.CheckNo, 
            //                        dbo.Payments.BankAccountNo, dbo.Payments.Remarks, dbo.Payments.DateCreated, dbo.Payments.DateUpdated,dbo.Payments.CreatedBy
            //                        FROM            
            //                        dbo.Payments INNER JOIN
            //                        dbo.Customers ON dbo.Payments.CustomerId = dbo.Customers.CustomerId
            //                        WHERE        
            //                        (dbo.Payments.CreatedBy = @CreatedBy) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate ) AND (dbo.Payments.PaymentType <> 'Cash Party')";
            //    }
            //} else {
            //    if (CustomerId > 0)
            //    {
            //        queryString = @"SELECT        
            //                        dbo.Payments.PaymentId, dbo.Payments.MemoMasterId, dbo.Payments.CustomerId, dbo.Payments.ShowRoomId, dbo.Payments.PaymentDate, dbo.Payments.SSAmount, dbo.Payments.SCAmount, 
            //                        dbo.Payments.SDiscount, dbo.Payments.Active, dbo.Payments.CreatedBy, dbo.Customers.CustomerName, dbo.Payments.PaymentType, dbo.Payments.HonourDate, dbo.Payments.CheckNo, 
            //                        dbo.Payments.BankAccountNo, dbo.Payments.Remarks, dbo.Payments.DateCreated, dbo.Payments.DateUpdated
            //                        FROM            
            //                        dbo.Payments INNER JOIN dbo.Customers ON dbo.Payments.CustomerId = dbo.Customers.CustomerId
            //                        WHERE        
            //                        (dbo.Payments.CustomerId = @customerId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate) AND (dbo.Payments.PaymentType <> 'Cash Party')";
            //    }
            //    else
            //    {
            //        queryString = @"SELECT        
            //                        dbo.Payments.PaymentId, dbo.Payments.MemoMasterId, dbo.Payments.CustomerId, dbo.Payments.ShowRoomId, dbo.Payments.PaymentDate, dbo.Payments.SSAmount, dbo.Payments.SCAmount, 
            //                        dbo.Payments.SDiscount, dbo.Payments.Active, dbo.Payments.CreatedBy, dbo.Customers.CustomerName, dbo.Payments.PaymentType, dbo.Payments.HonourDate, dbo.Payments.CheckNo, 
            //                        dbo.Payments.BankAccountNo, dbo.Payments.Remarks, dbo.Payments.DateCreated, dbo.Payments.DateUpdated, dbo.Payments.ShowRoomId
            //                        FROM            
            //                        dbo.Payments INNER JOIN
            //                        dbo.Customers ON dbo.Payments.CustomerId = dbo.Customers.CustomerId
            //                        WHERE        
            //                        (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate ) AND (dbo.Payments.PaymentType <> 'Cash Party')";
            //    }
            //}



            //using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    SqlCommand command = new SqlCommand(queryString, connection);
            //    connection.Open();
            //    command.Parameters.Add(new SqlParameter("@fromDate", fdate));
            //    command.Parameters.Add(new SqlParameter("@toDate", tdate));
            //    command.Parameters.Add(new SqlParameter("@CreatedBy", userName));
            //    if (CustomerId > 0) {
            //        command.Parameters.Add(new SqlParameter("@customerId", CustomerId));
            //    }                    
            //    command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
            //    SqlDataReader reader = command.ExecuteReader();
            //    try
            //    {
            //        while (reader.Read())
            //        {
            //            aObj = new PaymentView();
            //            aObj.PaymentId = (int) reader["PaymentId"];
            //            aObj.CustomerName = (string)reader["CustomerName"]; 
            //            aObj.PaymentDate = (DateTime)reader["PaymentDate"]; 
            //            aObj.SCAmount = (double) reader["SCAmount"];
            //            aObj.SDiscount = (double) reader["SDiscount"];
            //            aObj.PaymentType = (string)reader["PaymentType"];
            //            if (reader["CheckNo"] != DBNull.Value) {
            //                aObj.CheckNo = (string)reader["CheckNo"];
            //            }
            //            if (reader["BankAccountNo"] != DBNull.Value)
            //            {
            //                aObj.BankAccountNo = (string)reader["BankAccountNo"];
            //            }
            //            if (reader["HonourDate"] != DBNull.Value)
            //            {
            //                aObj.HonourDate = (DateTime) reader["HonourDate"];
            //            }
            //            if (reader["Remarks"] != DBNull.Value)
            //            {
            //                aObj.Remarks = (string)reader["Remarks"];
            //            }
            //            list.Add(aObj);
            //        }
            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}
            //ViewBag.AccountUserList = BankAccounts;
            
        }



        
        [Route("api/Payments/GetDateBetweenPaymentsSum/{FromDate}/{ToDate}/{CustomerId}")]
        [HttpGet]
        [ResponseType(typeof(PaymentView))]
        public IHttpActionResult GetDateBetweenPaymentsSum(string FromDate, string ToDate, int? CustomerId)
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            DateTime fdate = DateTime.Parse(FromDate);
            DateTime tdate = DateTime.Parse(ToDate);

            List<PaymentView> list = new List<PaymentView>();
            PaymentView aObj = new PaymentView();

            //string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            //string queryString = "";

            if (User.IsInRole("Zone Manager"))
            {
                if (CustomerId > 0)
                {
                    //queryString = @"SELECT        
                    //                SUM(SSAmount) AS SSAmount, SUM(SCAmount) AS SCAmount,  SUM(SDiscount) AS SDiscount
                    //                FROM            
                    //                dbo.Payments
                    //                WHERE        
                    //                (PaymentType = 'Cash') AND (dbo.Payments.CustomerId = @customerId) AND (dbo.Payments.CreatedBy = @CreatedBy) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate)";
                    var cashPaymentList = db.Customers
                                            .Include(c => c.Payments)
                                            .Select(c => new {
                                                CustomerId = c.CustomerId,
                                                CustomerName = c.CustomerName,
                                                CreatedBy = c.Payments.Select(a => a.CreatedBy),
                                                PaymentType = c.Payments.Select(a => a.PaymentType),
                                                PaymentDate = c.Payments.Select(a => a.PaymentDate),
                                                TotalPayments = c.Payments.Where(s => s.PaymentDate >= fdate && s.PaymentDate <= tdate && s.PaymentType == "Cash Party" && s.CreatedBy==userName).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fdate && s.PaymentDate <= tdate && s.PaymentType == "Cash Party" && s.CreatedBy == userName).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                                            })
                                            .Where(c => c.CustomerId == CustomerId)
                                            .ToList();
                    return Ok(cashPaymentList);
                }
                else
                {
                    //queryString = @"SELECT        
                    //                SUM(SSAmount) AS SSAmount, SUM(SCAmount) AS SCAmount, SUM(SDiscount) AS SDiscount
                    //                FROM            
                    //                dbo.Payments
                    //                WHERE        
                    //                (PaymentType = 'Cash') AND (dbo.Payments.CreatedBy = @CreatedBy) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate)";
                    var cashPaymentList = db.Customers
                        .Include(c => c.Payments)
                        .Select(c => new {
                            CustomerId = c.CustomerId,
                            CustomerName = c.CustomerName,
                            CreatedBy = c.Payments.Select(a => a.CreatedBy),
                            PaymentType = c.Payments.Select(a => a.PaymentType),
                            PaymentDate = c.Payments.Select(a => a.PaymentDate),
                            TotalPayments = c.Payments.Where(s => s.PaymentDate >= fdate && s.PaymentDate <= tdate && s.PaymentType == "Cash Party" && s.CreatedBy==userName).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                            TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fdate && s.PaymentDate <= tdate && s.PaymentType == "Cash Party" && s.CreatedBy == userName).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                        })
                        .ToList();
                    return Ok(cashPaymentList);
                }
            }
            else {
                if (CustomerId > 0)
                {
                    //queryString = @"SELECT        
                    //                SUM(SSAmount) AS SSAmount, SUM(SCAmount) AS SCAmount,  SUM(SDiscount) AS SDiscount
                    //                FROM            
                    //                dbo.Payments
                    //                WHERE        
                    //                (PaymentType = 'Cash') AND (dbo.Payments.CustomerId = @customerId) AND (dbo.Payments.ShowRoomId = @showRoomId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate)";
                    var cashPaymentList = db.Customers
                        .Include(c => c.Payments)
                        .Select(c => new {
                            CustomerId = c.CustomerId,
                            CustomerName = c.CustomerName,
                            CreatedBy = c.Payments.Select(a => a.CreatedBy),
                            PaymentType = c.Payments.Select(a => a.PaymentType),
                            PaymentDate = c.Payments.Select(a => a.PaymentDate),
                            TotalPayments = c.Payments.Where(s => s.PaymentDate >= fdate && s.PaymentDate <= tdate && s.PaymentType == "Cash Party" && s.ShowRoomId == showRoomId).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                            TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fdate && s.PaymentDate <= tdate && s.PaymentType == "Cash Party" && s.ShowRoomId == showRoomId).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                        })
                        .Where(c => c.CustomerId == CustomerId)
                        .ToList();
                    return Ok(cashPaymentList);
                }
                else
                {
                    //queryString = @"SELECT        
                    //                SUM(SSAmount) AS SSAmount, SUM(SCAmount) AS SCAmount, SUM(SDiscount) AS SDiscount
                    //                FROM            
                    //                dbo.Payments
                    //                WHERE        
                    //                (PaymentType = 'Cash') AND (dbo.Payments.ShowRoomId = @showRoomId) AND (dbo.Payments.PaymentDate >= @fromDate) AND (dbo.Payments.PaymentDate <= @toDate)";

                    //var cashPaymentList = db.Customers
                    //                        .Include(c => c.Payments)
                    //                        .Select(c => new
                    //                        {
                    //                            CustomerId = c.CustomerId,
                    //                            CustomerName = c.CustomerName,
                    //                            CreatedBy = c.Payments.Select(a => a.CreatedBy),
                    //                            PaymentType = c.Payments.Select(a => a.PaymentType),
                    //                            PaymentDate = c.Payments.Select(a => a.PaymentDate),
                    //                            TotalPayments = c.Payments.Where(s => s.PaymentDate >= fdate && s.PaymentDate <= tdate && s.PaymentType == "Cash Party" && s.ShowRoomId == showRoomId).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                    //                            TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fdate && s.PaymentDate <= tdate && s.PaymentType == "Cash Party" && s.ShowRoomId == showRoomId).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                    //                        }).ToList();

                    var cashSale = db.Payments
                        .Include(p => p.Customer)
                        .Select(p => new
                        {
                            p.CustomerId,
                            p.Customer.CustomerName,
                            p.PaymentId,
                            p.PaymentDate,
                            p.PaymentType,
                            p.SCAmount,
                            p.SDiscount,
                            p.ShowRoomId
                        })
                    .Where(p => p.PaymentDate >= fdate && p.PaymentDate <= tdate && p.PaymentType == "Cash" && p.ShowRoomId == showRoomId)
                    .ToList()
                    .Sum(x => x.SCAmount);

                    var cashDiscount = db.Payments
                        .Include(p => p.Customer)
                        .Select(p => new
                        {
                            p.CustomerId,
                            p.Customer.CustomerName,
                            p.PaymentId,
                            p.PaymentDate,
                            p.PaymentType,
                            p.SCAmount,
                            p.SDiscount,
                            p.ShowRoomId
                        })
                    .Where(p => p.PaymentDate >= fdate && p.PaymentDate <= tdate && p.PaymentType == "Cash" && p.ShowRoomId == showRoomId)
                    .ToList()
                    .Sum(x => x.SDiscount);

                    var cashPaymentList = new { SCAmount=cashSale, SDiscount=cashDiscount };
                    aObj = new PaymentView();
                    aObj.SCAmount = cashDiscount;
                    aObj.SDiscount = cashSale;
                    list.Add(aObj);
                    return Ok(list);
                }
            }


            


            //using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    SqlCommand command = new SqlCommand(queryString, connection);
            //    connection.Open();
            //    command.Parameters.Add(new SqlParameter("@fromDate", fdate));
            //    command.Parameters.Add(new SqlParameter("@toDate", tdate));
            //    command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
            //    command.Parameters.Add(new SqlParameter("@CreatedBy", userName));
            //    if (CustomerId > 0) {
            //        command.Parameters.Add(new SqlParameter("@customerId", CustomerId));
            //    }
            //    SqlDataReader reader = command.ExecuteReader();
            //    try
            //    {
            //        while (reader.Read())
            //        {
            //            aObj = new PaymentView();
            //            if (reader["SCAmount"] != DBNull.Value) {
            //                aObj.SCAmount = (double)reader["SCAmount"];
            //            }
            //            if (reader["SDiscount"] != DBNull.Value)
            //            {
            //                aObj.SDiscount = (double)reader["SDiscount"];
            //            }
                        
            //            list.Add(aObj);
            //        }
            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}
            //return Ok(list);
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