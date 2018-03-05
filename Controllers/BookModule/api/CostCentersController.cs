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
using PCBookWebApp.Models.BookModule;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.ViewModels;
using System.Configuration;
using System.Data.SqlClient;

namespace PCBookWebApp.Controllers.BookModule.api
{
    public class CostCentersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        [Route("api/CostCenters/CostCenterListById/{id}")]
        [HttpGet]
        [ResponseType(typeof(CostCenter))]
        public IHttpActionResult GetCostCenterListById(int id)
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            //var list = db.CostCenters.Where(cs => cs.LedgerId == id).ToList();
            List<CostCenter> list = new List<CostCenter>();
            CostCenter aObj = new CostCenter();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                        CostCenterId, CostCenterName, LedgerId
                                        FROM            
                                        dbo.CostCenters
                                        WHERE        
                                        (LedgerId = @ledgerId)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@ledgerId", id));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        aObj = new CostCenter();
                        aObj.CostCenterId = (int) reader["CostCenterId"];
                        aObj.CostCenterName = (string) reader["CostCenterName"];
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

        [Route("api/CostCenters/GetCostCentersList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetCostCentersList()
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
                                    dbo.CostCenters.CostCenterId AS id, dbo.CostCenters.CostCenterName AS name, dbo.CostCenters.LedgerId AS [group], dbo.Ledgers.LedgerName AS groupName, 
                                    dbo.CostCenters.ShowRoomId
                                    FROM            
                                    dbo.CostCenters INNER JOIN
                                    dbo.Ledgers ON dbo.CostCenters.LedgerId = dbo.Ledgers.LedgerId
                                    WHERE        
                                    (dbo.CostCenters.ShowRoomId =  @showRoomId)
                                    ORDER BY groupName";

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


        // GET: api/CostCenters
        public IQueryable<CostCenter> GetCostCenters()
        {
            return db.CostCenters;
        }

        // GET: api/CostCenters/5
        [ResponseType(typeof(CostCenter))]
        public async Task<IHttpActionResult> GetCostCenter(int id)
        {
            CostCenter costCenter = await db.CostCenters.FindAsync(id);
            if (costCenter == null)
            {
                return NotFound();
            }

            return Ok(costCenter);
        }

        // PUT: api/CostCenters/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCostCenter(int id, CostCenter costCenter)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime updateAt = DateTime.Now;
            DateTime dateCreated = DateTime.Now;
            costCenter.ShowRoomId = showRoomId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != costCenter.CostCenterId)
            {
                return BadRequest();
            }

            db.Entry(costCenter).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CostCenterExists(id))
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

        // POST: api/CostCenters
        [ResponseType(typeof(CostCenter))]
        public async Task<IHttpActionResult> PostCostCenter(CostCenter costCenter)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime updateAt = DateTime.Now;
            DateTime dateCreated = DateTime.Now;
            costCenter.ShowRoomId = showRoomId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CostCenters.Add(costCenter);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = costCenter.CostCenterId }, costCenter);
        }

        // DELETE: api/CostCenters/5
        [ResponseType(typeof(CostCenter))]
        public async Task<IHttpActionResult> DeleteCostCenter(int id)
        {
            CostCenter costCenter = await db.CostCenters.FindAsync(id);
            if (costCenter == null)
            {
                return NotFound();
            }

            db.CostCenters.Remove(costCenter);
            await db.SaveChangesAsync();

            return Ok(costCenter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CostCenterExists(int id)
        {
            return db.CostCenters.Count(e => e.CostCenterId == id) > 0;
        }
    }
}