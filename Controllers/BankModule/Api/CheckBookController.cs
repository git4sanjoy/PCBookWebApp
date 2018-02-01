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
using PCBookWebApp.Models;
using PCBookWebApp.Models.ViewModels;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.BookViewModel;
using PCBookWebApp.Models.BookViewModel.ViewModels;
using PCBookWebApp.Models.BankModule;

namespace PCBookWebApp.Controllers.BankModule.Api
{
    [Authorize]
    public class CheckBookController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/CheckBook
        public IQueryable<CheckBook> GetCheckBooks()
        {
            return db.CheckBooks;
        }

        [Route("api/CheckBook/GetCheckBookList")]
        [HttpGet]
        [ResponseType(typeof(CheckBook))]
        public IHttpActionResult GetCheckBookList()
        {
            //List<CheckBook> checkBooks = new List<CheckBook>();
            //checkBooks = db.CheckBooks.ToList();
            //if (checkBooks == null)
            //{
            //    return NotFound();
            //}
            //return Ok(checkBooks);
            string userName = User.Identity.GetUserName();
            List<CheckBookView> checkBooks = new List<CheckBookView>();
            CheckBookView checkBook = new CheckBookView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                        dbo.CheckBooks.CheckBookId, dbo.CheckBooks.BankAccountId, dbo.CheckBooks.CheckBookNo, dbo.CheckBooks.StartSuffices, dbo.CheckBooks.StartNo, dbo.CheckBooks.EndNo, 
                                        dbo.BankAccounts.BankAccountNumber, dbo.CheckBooks.CreatedBy
                                    FROM            
                                        dbo.CheckBooks INNER JOIN
                                        dbo.BankAccounts ON dbo.CheckBooks.BankAccountId = dbo.BankAccounts.BankAccountId
                                    WHERE        
                                        (dbo.CheckBooks.CreatedBy = @userName)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add("@userName", SqlDbType.VarChar, 145).Value = userName;
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int checkBookId = (int)reader["CheckBookId"];
                        string bankAccountNumber = (string)reader["BankAccountNumber"];
                        string checkBookNo = (string)reader["CheckBookNo"];
                        string startSuffices = "";
                        if (reader["StartSuffices"] != DBNull.Value)
                        {
                            startSuffices = (string)reader["StartSuffices"];
                        }

                        double startNo = (double)reader["StartNo"];
                        double endNo = (double)reader["EndNo"];

                        checkBook = new CheckBookView();
                        checkBook.CheckBookId = checkBookId;
                        checkBook.BankAccountNumber = bankAccountNumber;
                        checkBook.CheckBookNo = checkBookNo;
                        checkBook.StartSuffices = startSuffices;
                        checkBook.StartNo = startNo;
                        checkBook.EndNo = endNo;
                        checkBooks.Add(checkBook);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(checkBooks);
        }

        [Route("api/CheckBook/CheckBookStatus")]
        [HttpGet]
        public IHttpActionResult CheckBookStatus()
        {
            string userName = User.Identity.GetUserName();
            List<CheckBookStatusView> checkBookStatus = new List<CheckBookStatusView>();
            CheckBookStatusView checkBook = new CheckBookStatusView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;

            string queryString = "";

            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                queryString = @"select * from CheckBooks AS a
	                                OUTER APPLY(
		                                select BankAccountNumber from BankAccounts as x
		                                WHERE a.BankAccountId = x.BankAccountId
	                                ) as x
	                                OUTER APPLY (
		                                select  count(*) AS UnUsed  from CheckBookPages AS b
		                                WHERE a.CheckBookId = b.CheckBookId AND b.Active = 'false'
		                                GROUP BY b.CheckBookId
	                                ) AS b
	                                OUTER APPLY (
		                                select count(*) AS Used  from CheckBookPages AS c
		                                WHERE a.CheckBookId = c.CheckBookId AND c.Active = 'true'
		                                GROUP BY c.CheckBookId
	                                ) AS c WHERE b.UnUsed>0 ";
            }
            else
            {
                queryString = @"select * from CheckBooks AS a
	                                OUTER APPLY(
		                                select BankAccountNumber from BankAccounts as x
		                                WHERE a.BankAccountId = x.BankAccountId
	                                ) as x
	                                OUTER APPLY (
		                                select  count(*) AS UnUsed  from CheckBookPages AS b
		                                WHERE a.CheckBookId = b.CheckBookId AND b.Active = 'false'
		                                GROUP BY b.CheckBookId
	                                ) AS b
	                                OUTER APPLY (
		                                select count(*) AS Used  from CheckBookPages AS c
		                                WHERE a.CheckBookId = c.CheckBookId AND c.Active = 'true'
		                                GROUP BY c.CheckBookId
	                                ) AS c

                                WHERE b.UnUsed>0 AND a.CreatedBy= @createdBy";
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@createdBy", userName));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        
                        string bankAccountNumber = (string)reader["BankAccountNumber"];
                        string checkBookNo = (string)reader["CheckBookNo"];
                        string startSuffices = "";
                        if (reader["StartSuffices"] != DBNull.Value)
                        {
                            startSuffices = (string)reader["StartSuffices"];
                        }
                        double startNo = (double)reader["StartNo"];
                        double endNo = (double)reader["EndNo"];
                        int usedCheck = 0;
                        if (reader["Used"] != DBNull.Value)
                        {
                            usedCheck = (int)reader["Used"];
                        }
                        int unUsedCheck = (int) reader["UnUsed"];

                        checkBook = new CheckBookStatusView();

                        checkBook.BankAccountNumber = bankAccountNumber;
                        checkBook.CheckBookNo = checkBookNo;
                        checkBook.StartSuffices = startSuffices;
                        checkBook.StartNo = startNo;
                        checkBook.EndNo = endNo;
                        checkBook.UsedCheck = usedCheck;
                        checkBook.UnUsedCheck = unUsedCheck;

                        checkBookStatus.Add(checkBook);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(checkBookStatus);
        }
        // GET: api/CheckBook/5
        [ResponseType(typeof(CheckBook))]
        public IHttpActionResult GetCheckBook(int id)
        {
            CheckBook checkBook = db.CheckBooks.Find(id);
            if (checkBook == null)
            {
                return NotFound();
            }

            return Ok(checkBook);
        }

        // PUT: api/CheckBook/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCheckBook(int id, CheckBook checkBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != checkBook.CheckBookId)
            {
                return BadRequest();
            }

            db.Entry(checkBook).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckBookExists(id))
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

        // POST: api/CheckBook
        [ResponseType(typeof(CheckBook))]
        public IHttpActionResult PostCheckBook(CheckBook checkBook)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            checkBook.CreatedBy = userName;
            checkBook.DateCreated = createdAt;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CheckBooks.Add(checkBook);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = checkBook.CheckBookId }, checkBook);
        }

        // DELETE: api/CheckBook/5
        [ResponseType(typeof(CheckBook))]
        public IHttpActionResult DeleteCheckBook(int id)
        {
            CheckBook checkBook = db.CheckBooks.Find(id);
            if (checkBook == null)
            {
                return NotFound();
            }

            db.CheckBooks.Remove(checkBook);
            db.SaveChanges();

            return Ok(checkBook);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CheckBookExists(int id)
        {
            return db.CheckBooks.Count(e => e.CheckBookId == id) > 0;
        }
    }
}