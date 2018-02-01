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
using Microsoft.AspNet.Identity;
using System.Web.Http.Results;
using PCBookWebApp.Models.ViewModels;
using System.Data.SqlClient;
using System.Configuration;
using PCBookWebApp.Models.BookViewModel;
using PCBookWebApp.Models.BookViewModel.ViewModels;
using System.Data.Entity.Migrations;
using PCBookWebApp.Models.BankModule;

namespace PCBookWebApp.Controllers.BankModule.Api
{
    [Authorize]
    public class CheckController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        //private static string scriptDataPicker = @"<script type='text/javascript' src='{0}'></script>";





        [Route("api/Check/ApplicationDetail")]
        [HttpGet]
        public IHttpActionResult ApplicationDetail()
        {
            string userName = User.Identity.GetUserName();
            var unitCount = 0;
            var bankCount = 0;
            var bankAccountsCount = 0;
            var usedCheckCount = 0;
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                unitCount = db.Units.Count();
                bankCount = db.Banks.Count();
                bankAccountsCount = db.BankAccounts.Count();
                usedCheckCount = db.Checks.Count();
            }
            else
            {
                string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
                string queryStringBankAccountCount = @"SELECT        
                                        dbo.UserBankAccounts.UserBankAccountID, 
                                        dbo.UserBankAccounts.BankAccountID, 
                                        dbo.UserBankAccounts.UserName, 
                                        dbo.BankAccounts.BankAccountNumber
                                    FROM            
                                        dbo.UserBankAccounts 
                                    INNER JOIN                         
                                        dbo.BankAccounts ON dbo.UserBankAccounts.BankAccountID = dbo.BankAccounts.BankAccountID
                                    WHERE        
                                        (dbo.UserBankAccounts.UserName = @userName)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryStringBankAccountCount, connection);
                    connection.Open();
                    command.Parameters.Add(new SqlParameter("@userName", userName));
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            bankAccountsCount = bankAccountsCount + 1;
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                //Bank Count
                string queryStringBankCount = @"SELECT DISTINCT BankId FROM ViewUserAccountBankAndUnit WHERE(ViewUserAccountBankAndUnit.UserName =  @userName)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryStringBankCount, connection);
                    connection.Open();
                    command.Parameters.Add(new SqlParameter("@userName", userName));
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            bankCount = bankCount + 1;
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                //Unit Count
                string queryStringUnitCount = @"SELECT DISTINCT UnitId FROM ViewUserAccountBankAndUnit WHERE(ViewUserAccountBankAndUnit.UserName =  @userName)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryStringUnitCount, connection);
                    connection.Open();
                    command.Parameters.Add(new SqlParameter("@userName", userName));
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            unitCount = unitCount + 1;
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                usedCheckCount = db.Checks.Where(a => a.CreatedBy == userName).Count();
            }



            return Ok(new { bankCount = bankCount, unitCount = unitCount, bankAccountsCount = bankAccountsCount, usedCheckCount= usedCheckCount });
        }

        [Route("api/Check/GetUserBankAccounts")]
        [HttpGet]
        [ResponseType(typeof(BankAccount))]
        public IHttpActionResult GetUserBankAccounts()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            //List<BankAccount> list = new List<BankAccount>();
            var list = db.BankAccounts
                .Where(a => a.ShowRoomId == showRoomId)
                .Select(a=> new { BankAccountId=a.BankAccountId, BankAccountNumber = a.BankAccountNumber, LedgerId = a.LedgerId });

            return Ok(list);
        }

        // GET: api/Check
        public  IQueryable<Check> GetChecks()
        {
            var checks =  db.Checks.Include(c => c.BankAccount).Include(c => c.Ledger).Include(c=> c.CheckBookPage);                         
            return db.Checks;
        }
        [Route("api/Check/GetCheckList")]
        [HttpGet]
        [ResponseType(typeof(Check))]
        public IHttpActionResult GetCheckList()
        {
            List<Check> checks = new List<Check>();
            checks = db.Checks.ToList();
            if (checks == null)
            {
                return NotFound();
            }
            return Ok(checks);
        }

        // GET: api/Check/5
        [ResponseType(typeof(Check))]
        public IHttpActionResult GetCheck(int id)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            //Check check = db.Checks.Find(id);
            //if (check == null)
            //{
            //    return NotFound();
            //}
            //return Ok(check);
            CheckView checkObj = new CheckView();
            SqlConnection checkConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            checkConnection.Open();
            try
            {
                SqlDataReader checkReader = null;
                string sql = "";
                sql = "SELECT BankAccountNumber, CheckNumber, Amount, IssueDate, CheckDate, HonourDate, Remarks, PartyName, BankName, UnitName, CheckId, PartyId, BankAccountId, UnitId, BankId FROM dbo.CheckView WHERE CheckId=" + id;

                SqlCommand pbCommand = new SqlCommand(sql, checkConnection);
                checkReader = pbCommand.ExecuteReader();
                while (checkReader.Read())
                {
                    
                    checkObj.CheckId = (int)checkReader["CheckId"];
                    checkObj.CheckNo = (string)checkReader["CheckNumber"];
                    checkObj.IssueDate = (DateTime)checkReader["IssueDate"];
                    checkObj.CheckDate = (DateTime)checkReader["CheckDate"];
                    checkObj.HonourDate = (DateTime)checkReader["HonourDate"];
                    checkObj.Amount = (double)checkReader["Amount"];
                    checkObj.BankAccountNumber = (string)checkReader["BankAccountNumber"];
                    checkObj.BankName = (string)checkReader["BankName"];
                    checkObj.PartyName = (string)checkReader["PartyName"];
                    checkObj.PartyId = (int)checkReader["PartyId"];
                    checkObj.BankAccountId = (int)checkReader["BankAccountId"];
                    if (checkReader["Remarks"] != System.DBNull.Value)
                    {
                        checkObj.Remarks = checkReader["Remarks"].ToString();
                    }
                    
                }
                checkReader.Close();
                checkConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(checkObj);
        }

        // PUT: api/Check/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCheck(int id, Check check)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != check.CheckId)
            {
                return BadRequest();
            }

            db.Entry(check).State = EntityState.Modified;

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!CheckExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return StatusCode(HttpStatusCode.NoContent);
            // Proceed to Update
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString))
            {
                con.Open();
                string updateSQL;
                updateSQL = "UPDATE dbo.Checks SET ";
                updateSQL += "PartyId=@partyId, Amount=@amount, ";
                updateSQL += "IssueDate=@idate, CheckDate=@cdate, HonourDate=@hdate, Remarks=@remarks, ";
                updateSQL += "CheckNumber=@checkNo ";
                updateSQL += "WHERE CheckId=@checkId";

                SqlCommand cmdInsert = new SqlCommand(updateSQL, con);
                cmdInsert.Parameters.Add("@partyId", SqlDbType.Int).Value = check.LedgerId;                
                cmdInsert.Parameters.Add("@amount", SqlDbType.Float).Value = check.Amount;
                cmdInsert.Parameters.Add("@idate", SqlDbType.DateTime).Value = check.IssueDate;
                cmdInsert.Parameters.Add("@cdate", SqlDbType.DateTime).Value = check.CheckDate;
                cmdInsert.Parameters.Add("@hdate", SqlDbType.DateTime).Value = check.HonourDate;
                cmdInsert.Parameters.Add("@checkNo", SqlDbType.VarChar, 145).Value = check.CheckNumber;
                cmdInsert.Parameters.Add("@checkId", SqlDbType.Int).Value = check.CheckId;
                if (check.Remarks != null)
                {
                    cmdInsert.Parameters.Add("@remarks", SqlDbType.VarChar, 145).Value = check.Remarks;
                }
                else
                {
                    cmdInsert.Parameters.Add("@remarks", SqlDbType.VarChar, 145).Value = DBNull.Value;
                }


                cmdInsert.CommandType = CommandType.Text;
                cmdInsert.ExecuteNonQuery();
                con.Close();
            }
            // Return Object
            CheckView checkObj = new CheckView();
            SqlConnection checkConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            checkConnection.Open();
            try
            {
                SqlDataReader checkReader = null;
                string sql = "";
                sql = "SELECT BankAccountNumber, CheckNumber, Amount, IssueDate, CheckDate, HonourDate, Remarks, PartyName, BankName, UnitName, CheckId, PartyId, BankAccountId, UnitId, BankId FROM dbo.CheckView WHERE CheckId=" + id;

                SqlCommand pbCommand = new SqlCommand(sql, checkConnection);
                checkReader = pbCommand.ExecuteReader();
                while (checkReader.Read())
                {

                    checkObj.CheckId = (int)checkReader["CheckId"];
                    checkObj.CheckNo = (string)checkReader["CheckNumber"];
                    checkObj.IssueDate = (DateTime)checkReader["IssueDate"];
                    checkObj.CheckDate = (DateTime)checkReader["CheckDate"];
                    checkObj.HonourDate = (DateTime)checkReader["HonourDate"];
                    checkObj.Amount = (double)checkReader["Amount"];
                    checkObj.BankAccountNumber = (string)checkReader["BankAccountNumber"];
                    checkObj.BankName = (string)checkReader["BankName"];
                    checkObj.PartyName = (string)checkReader["PartyName"];
                    checkObj.PartyId = (int)checkReader["PartyId"];
                    checkObj.BankAccountId = (int)checkReader["BankAccountId"];
                    if (checkReader["Remarks"] != System.DBNull.Value)
                    {
                        checkObj.Remarks = checkReader["Remarks"].ToString();
                    }

                }
                checkReader.Close();
                checkConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(checkObj);
        }


        [Route("api/Check/UpdateVoucherIdToChecks/{CheckId}/{VoucherDetailId}/{VoucherId}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateVoucherIdToChecks(int CheckId, int VoucherDetailId, int VoucherId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var parentCheck = db.Checks.Where(x => x.CheckId == CheckId).FirstOrDefault();
            if (parentCheck != null)
            {
                parentCheck.VoucherId = VoucherId;
                parentCheck.VoucherDetailId = VoucherDetailId;
                db.Checks.AddOrUpdate(parentCheck);
                db.SaveChanges();
            }
            return Ok();
        }

        // POST: api/Check
        [ResponseType(typeof(Check))]
        public IHttpActionResult PostCheck(Check check)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            check.CreatedBy = userName;
            check.DateCreated = createdAt;
            check.DateUpdated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Checks.Add(check);
            db.SaveChanges();

            //var accNo = db.BankAccounts
            //                    .Where(ba => ba.BankAccountId == check.BankAccountId)
            //                    .Select(ba => ba.BankAccountNumber)
            //                    .FirstOrDefault();
            //var partyName = db.Parties
            //                    .Where(p => p.PartyId == check.PartyId)
            //                    .Select(p => p.PartyName)
            //                    .FirstOrDefault();
            //check.BankAccount.BankAccountNumber = accNo;
            //check.Party.PartyName = partyName;

            return CreatedAtRoute("DefaultApi", new { id = check.CheckId }, check);
        }

        // DELETE: api/Check/5
        [ResponseType(typeof(Check))]
        public IHttpActionResult DeleteCheck(int id)
        {
            Check check = db.Checks.Find(id);
            if (check == null)
            {
                return NotFound();
            }

            var aCheckBookPage = db.CheckBookPages.Find(check.CheckBookPageId);
            if (aCheckBookPage != null)
            {
                aCheckBookPage.Active = false;
                db.Entry(aCheckBookPage).State = EntityState.Modified;
                db.SaveChanges();
            }

            db.Checks.Remove(check);
            db.SaveChanges();

            return Ok(check);
        }
        // GET: api/Party/GetPartyByDate/2016-05-01/2016-12-31
        [Route("api/Check/CheckListByDate/{FromDate}/{ToDate}/{SearchBy}")]
        [HttpGet]
        [ResponseType(typeof(Check))]
        public IHttpActionResult CheckListByDate(string FromDate, string ToDate, string SearchBy)
        {
            string userName = User.Identity.GetUserName();
            DateTime fdate = DateTime.Parse(FromDate);
            DateTime tdate = DateTime.Parse(ToDate);
            List<Check> checks = new List<Check>();
            //if (SearchBy== "CheckDate") {
            //     checks = db.Checks.Where(c => c.CheckDate >= fdate && c.CheckDate <= tdate).ToList();
            //}
            //else if(SearchBy == "IssueDate") {
            //    checks = db.Checks.Where(c => c.IssueDate >= fdate && c.IssueDate <= tdate).ToList();
            //}
            //else {
            //    checks = db.Checks.Where(c => c.HonourDate >= fdate && c.HonourDate <= tdate).ToList();
            //}            
            //if (checks == null)
            //{
            //    return NotFound();
            //}
            //return Ok(checks);

            List<CheckView> checkList = new List<CheckView>();
            SqlConnection checkConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            checkConnection.Open();
            try
            {
                SqlDataReader checkReader = null;
                string sql = "";
                if (SearchBy == "CheckDate")
                {
                    sql = "SELECT BankAccountNumber, CheckNumber, Amount, IssueDate, CheckDate, HonourDate, Remarks, PartyName, BankName, UnitName, CheckId, PartyId, BankAccountId, UnitId, BankId,CheckBookNo FROM dbo.CheckView WHERE (CheckDate BETWEEN CONVERT(DATETIME, '" + FromDate + "', 102) AND CONVERT(DATETIME, '" + ToDate + "', 102)) AND (CreatedBy='"+ userName + "')";
                }
                if (SearchBy == "IssueDate")
                {
                    sql = "SELECT BankAccountNumber, CheckNumber, Amount, IssueDate, CheckDate, HonourDate, Remarks, PartyName, BankName, UnitName, CheckId, PartyId, BankAccountId, UnitId, BankId,CheckBookNo FROM dbo.CheckView WHERE (IssueDate BETWEEN CONVERT(DATETIME, '" + FromDate + "', 102) AND CONVERT(DATETIME, '" + ToDate + "', 102)) AND (CreatedBy='" + userName + "')";
                }
                else
                {
                    sql = "SELECT BankAccountNumber, CheckNumber, Amount, IssueDate, CheckDate, HonourDate, Remarks, PartyName, BankName, UnitName, CheckId, PartyId, BankAccountId, UnitId, BankId,CheckBookNo FROM dbo.CheckView WHERE (HonourDate BETWEEN CONVERT(DATETIME, '" + FromDate + "', 102) AND CONVERT(DATETIME, '" + ToDate + "', 102)) AND (CreatedBy='" + userName + "')";
                }


                SqlCommand pbCommand = new SqlCommand(sql, checkConnection);
                checkReader = pbCommand.ExecuteReader();
                while (checkReader.Read())
                {
                    CheckView checkObj = new CheckView();
                    checkObj.CheckId= (int) checkReader["CheckId"];
                    checkObj.CheckNo = (string)checkReader["CheckNumber"];
                    checkObj.IssueDate = (DateTime)checkReader["IssueDate"];
                    checkObj.CheckDate = (DateTime)checkReader["CheckDate"];
                    checkObj.HonourDate = (DateTime)checkReader["HonourDate"];
                    checkObj.Amount = (double)checkReader["Amount"];
                    checkObj.BankAccountNumber = (string)checkReader["BankAccountNumber"];
                    checkObj.BankName = (string)checkReader["BankName"];
                    checkObj.PartyName = (string)checkReader["PartyName"];
                    if (checkReader["CheckBookNo"] != System.DBNull.Value)
                    {
                        checkObj.CheckBookNo = checkReader["CheckBookNo"].ToString();
                    }
                    if (checkReader["Remarks"] != System.DBNull.Value)
                    {
                        checkObj.Remarks = checkReader["Remarks"].ToString();
                    }
                    checkList.Add(checkObj);
                }
                checkReader.Close();
                checkConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(checkList);            
        }


        [Route("api/Check/CheckListByCheckNo/{CheckNo}")]
        [HttpGet]
        [ResponseType(typeof(Check))]
        public IHttpActionResult CheckListByCheckNo(string CheckNo)
        {
            string userName = User.Identity.GetUserName();
            List<Check> checks = new List<Check>();
            List<CheckView> checkList = new List<CheckView>();
            SqlConnection checkConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            checkConnection.Open();
            try
            {
                SqlDataReader checkReader = null;
                string sql = "";
                sql = "SELECT BankAccountNumber, CheckNumber, Amount, IssueDate, CheckDate, HonourDate, Remarks, PartyName, BankName, UnitName, CheckId, PartyId, BankAccountId, UnitId, BankId,CheckBookNo FROM dbo.CheckView WHERE (CheckNumber='" + CheckNo + "') AND (CreatedBy='" + userName + "')";
                SqlCommand pbCommand = new SqlCommand(sql, checkConnection);
                checkReader = pbCommand.ExecuteReader();
                while (checkReader.Read())
                {
                    CheckView checkObj = new CheckView();
                    checkObj.CheckId = (int)checkReader["CheckId"];
                    checkObj.CheckNo = (string)checkReader["CheckNumber"];
                    checkObj.IssueDate = (DateTime)checkReader["IssueDate"];
                    checkObj.CheckDate = (DateTime)checkReader["CheckDate"];
                    checkObj.HonourDate = (DateTime)checkReader["HonourDate"];
                    checkObj.Amount = (double)checkReader["Amount"];
                    checkObj.BankAccountNumber = (string)checkReader["BankAccountNumber"];
                    checkObj.BankName = (string)checkReader["BankName"];
                    checkObj.PartyName = (string)checkReader["PartyName"];
                    if (checkReader["CheckBookNo"] != System.DBNull.Value)
                    {
                        checkObj.CheckBookNo = checkReader["CheckBookNo"].ToString();
                    }
                    if (checkReader["Remarks"] != System.DBNull.Value)
                    {
                        checkObj.Remarks = checkReader["Remarks"].ToString();
                    }
                    checkList.Add(checkObj);
                }
                checkReader.Close();
                checkConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(checkList);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CheckExists(int id)
        {
            return db.Checks.Count(e => e.CheckId == id) > 0;
        }
    }
}