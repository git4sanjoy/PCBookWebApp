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
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity.Migrations;
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Controllers.BookModule.api
{
    [Authorize]
    public class LedgersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/Ledgers/GetLedgerTypeAheadList/{SearchTerm}")]
        [HttpGet]
        [ResponseType(typeof(Ledger))]
        public IHttpActionResult GetPartyTypeAheadList(string SearchTerm)
        {
            string userName = User.Identity.GetUserName();

            List<Ledger> typeAheadList = new List<Ledger>();
            SqlConnection checkConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            checkConnection.Open();
            try
            {
                SqlDataReader ledgerReader = null;
                string sql = @"SELECT        
                                LedgerId, LedgerName, CreatedBy
                                FROM            
                                dbo.Ledgers
                                WHERE        
                                (CreatedBy = @createdBy) AND (LedgerName LIKE @searchTerm) ORDER BY LedgerName ASC";

                string searchTerm = string.Format("{0}%", SearchTerm);

                SqlCommand command = new SqlCommand(sql, checkConnection);
                command.Parameters.Add(new SqlParameter("@createdBy", userName));
                command.Parameters.Add(new SqlParameter("@searchTerm", searchTerm));
                ledgerReader = command.ExecuteReader();
                while (ledgerReader.Read())
                {
                    Ledger partyObj = new Ledger();
                    partyObj.LedgerId = (int)ledgerReader["LedgerId"];
                    partyObj.LedgerName = (string)ledgerReader["LedgerName"];
                    typeAheadList.Add(partyObj);
                }
                ledgerReader.Close();
                checkConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(typeAheadList);
        }

        [Route("api/Ledgers/LedgersMultiSelectList")]
        [HttpGet]
        public IHttpActionResult LedgersMultiSelectList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<MultiSelectView> unitList = new List<MultiSelectView>();
            MultiSelectView unitObj = new MultiSelectView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT LedgerId, LedgerName FROM dbo.Ledgers WHERE ShowRoomId=@showRoomId";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["LedgerId"];
                        string name = (string)reader["LedgerName"];
                        //int categoryId = (int)reader["MainMaterialId"];
                        unitObj = new MultiSelectView();
                        unitObj.id = id;
                        unitObj.label = name;
                        //unitObj.gender = categoryId;
                        unitList.Add(unitObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(unitList);
        }


        [Route("api/Ledgers/GetLedgerList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetLedgerList()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<XEditGroupView> subDepartmentList = new List<XEditGroupView>();
            XEditGroupView subDepartment = new XEditGroupView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.Ledgers.LedgerId AS id, dbo.Ledgers.LedgerName AS name, dbo.Ledgers.GroupId AS [group], dbo.Groups.GroupName AS groupName
                                    FROM            
                                    dbo.Ledgers INNER JOIN
                                    dbo.Groups ON dbo.Ledgers.GroupId = dbo.Groups.GroupId
                                    WHERE        
                                    (dbo.Ledgers.ShowRoomId = @showRoomId)
                                    ORDER BY groupName, name";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        string name = (string)reader["name"];
                        int group = (int)reader["group"];
                        string groupName = (string)reader["groupName"];

                        subDepartment = new XEditGroupView();
                        subDepartment.id = id;
                        subDepartment.name = name;
                        subDepartment.group = group;
                        subDepartment.groupName = groupName;
                        subDepartmentList.Add(subDepartment);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(subDepartmentList);
        }


        [Route("api/Ledgers/GetGroupListXEdit")]
        [HttpGet]
        [ResponseType(typeof(Ledger))]
        public IHttpActionResult GetGroupListXEdit()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            //var list = db.Groups
            //                .Where(a => a.ShowRoomId == showRoomId && a.GroupName != "Primary")
            //                .Select(e => new { id = e.GroupId, text = e.GroupName })
            //                .OrderBy(e => e.text);
            var list = db.Groups
                .Where(a => a.GroupName != "Primary" && a.IsParent==false && a.PrimaryId == null)
                .Select(e => new { id = e.GroupId, text = e.GroupName })
                .OrderBy(e => e.text);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/Ledgers/GetLedgerDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Ledger))]
        public IHttpActionResult GetLedgerDropDownList()
        {
            string userName = User.Identity.GetUserName();
            string currentUserId = User.Identity.GetUserId();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.Ledgers
                            .Where(a => a.ShowRoomId == showRoomId)
                            .Select(e => new { LedgerId = e.LedgerId, LedgerName = e.LedgerName,BookId = e.BookId, TrialBalanceId = e.TrialBalanceId })
                            .OrderBy(e => e.LedgerName);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }



        // GET: api/Ledgers
        public IQueryable<Ledger> GetLedgers()
        {
            return db.Ledgers;
        }

        // GET: api/Ledgers/5
        [ResponseType(typeof(Ledger))]
        public async Task<IHttpActionResult> GetLedger(int id)
        {
            Ledger ledger = await db.Ledgers.FindAsync(id);
            if (ledger == null)
            {
                return NotFound();
            }

            return Ok(ledger);
        }

        // PUT: api/Ledgers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLedger(int id, Ledger ledger)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime updateAt = DateTime.Now;
            bool Active = false;
            string CreatedBy = null;
            DateTime DateCreated = DateTime.Now;

            // Get Date Created
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                SqlDataReader reader = null;
                string sql = @"SELECT dbo.Ledgers .* FROM   dbo.Ledgers  WHERE LedgerId=@ledgerId";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@ledgerId", SqlDbType.Int).Value = id;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Active = (bool)reader["Active"];
                    CreatedBy = (string)reader["CreatedBy"];
                    DateCreated = (DateTime)reader["DateCreated"];

                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            // End Get Date Created

            ledger.Active = Active;
            ledger.DateCreated = DateCreated;
            ledger.DateUpdated = updateAt;
            ledger.CreatedBy = CreatedBy;
            ledger.ShowRoomId = showRoomId;

            if (db.Ledgers.Any(m => m.LedgerName == ledger.LedgerName && m.ShowRoomId == showRoomId))
            {
                ModelState.AddModelError("LedgerName", "Ledger Name Already Exists!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ledger.LedgerId)
            {
                return BadRequest();
            }

            db.Entry(ledger).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LedgerExists(id))
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

        // POST: api/Ledgers
        [ResponseType(typeof(Ledger))]
        public async Task<IHttpActionResult> PostLedger(Ledger ledger)
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string bookId = db.Groups
                               .Where(e => e.GroupId == ledger.GroupId)
                               .ToList()
                               .FirstOrDefault().GroupIdStr.Split('-')[1].ToString();

            DateTime createdAt = DateTime.Now;
            ledger.CreatedBy = userName;
            ledger.DateCreated = createdAt;
            ledger.DateUpdated = createdAt;
            ledger.ShowRoomId = showRoomId;
            ledger.TrialBalanceId = 0;
            ledger.BookId = Convert.ToInt32(bookId);

            if (db.Ledgers.Any(m => m.LedgerName == ledger.LedgerName && m.ShowRoomId == showRoomId))
            {
                ModelState.AddModelError("LedgerName", "Ledger Name Already Exists!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ledgers.Add(ledger);
            await db.SaveChangesAsync();

            var newLedgerId = ledger.LedgerId;
            var parentLedger = db.Ledgers.Where(x => x.LedgerId == newLedgerId).FirstOrDefault();
            if (parentLedger.TrialBalanceId == 0)
            {
                parentLedger.TrialBalanceId = newLedgerId;
                db.Ledgers.AddOrUpdate(parentLedger);
                db.SaveChanges();
            }

            return CreatedAtRoute("DefaultApi", new { id = newLedgerId }, ledger);
        }

        // DELETE: api/Ledgers/5
        [ResponseType(typeof(Ledger))]
        public async Task<IHttpActionResult> DeleteLedger(int id)
        {
            Ledger ledger = await db.Ledgers.FindAsync(id);
            if (ledger == null)
            {
                return NotFound();
            }

            db.Ledgers.Remove(ledger);
            await db.SaveChangesAsync();

            return Ok(ledger);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LedgerExists(int id)
        {
            return db.Ledgers.Count(e => e.LedgerId == id) > 0;
        }
    }
}