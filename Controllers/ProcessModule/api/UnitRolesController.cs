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
using PCBookWebApp.Models.ProcessModule;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.SqlClient;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    public class UnitRolesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        [Route("api/UnitRoles/GetUnitRolesList")]
        [HttpGet]
        [ResponseType(typeof(UnitRole))]
        public IHttpActionResult UnitRolesList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<UnitRole> list = new List<UnitRole>();
            UnitRole aObj = new UnitRole();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    ShowRoomId, UnitRoleId AS id, UnitRoleName AS UnitRoleName
                                    FROM            
                                    dbo.UnitRoles
                                    WHERE (ShowRoomId = @showRoomId)";

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
                            string name = (string)reader["UnitRoleName"];
                        aObj = new UnitRole();
                        aObj.UnitRoleId = id;
                        aObj.UnitRoleName = name;
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


        // GET: api/UnitRoles
        public IQueryable<UnitRole> GetUnitRoles()
        {
            return db.UnitRoles;
        }

        // GET: api/UnitRoles/5
        [ResponseType(typeof(UnitRole))]
        public IHttpActionResult GetUnitRole(int id)
        {
            UnitRole unitRole = db.UnitRoles.Find(id);
            if (unitRole == null)
            {
                return NotFound();
            }

            return Ok(unitRole);
        }

        // PUT: api/UnitRoles/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUnitRole(int id, UnitRole unitRole)
        {
            var msg = 0;
            var check = db.UnitRoles.FirstOrDefault(m => m.UnitRoleName == unitRole.UnitRoleName);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != unitRole.UnitRoleId)
            {
                return BadRequest();
            }

            //db.Entry(unitRole).State = EntityState.Modified;

            if (check == null)
            {
                try
                {
                    var obj = db.UnitRoles.FirstOrDefault(m => m.UnitRoleId == unitRole.UnitRoleId);
                    unitRole.CreatedBy = obj.CreatedBy;
                    unitRole.DateCreated = obj.DateCreated;
                    unitRole.DateUpdated = DateTime.Now;
                    unitRole.ShowRoomId = obj.ShowRoomId;
                    unitRole.Active = true;
                    db.UnitRoles.AddOrUpdate(unitRole);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitRoleExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Ok(msg);
            //return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/UnitRoles
        [ResponseType(typeof(UnitRole))]
       
       public async Task<IHttpActionResult> PostUnitRole(UnitRole unitRole)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            bool isTrue = db.UnitRoles.Any(s => s.UnitRoleName == unitRole.UnitRoleName.Trim() && s.ShowRoomId==showRoomId);
            if (isTrue == false)
            {
                unitRole.ShowRoomId = showRoomId;
                unitRole.CreatedBy = userName;
                unitRole.DateCreated = DateTime.Now;
                unitRole.DateCreated = unitRole.DateCreated;
                unitRole.Active = true;
                db.UnitRoles.Add(unitRole);
                await db.SaveChangesAsync();
            }
            return CreatedAtRoute("DefaultApi", new { id = unitRole.UnitRoleId }, unitRole);
        }

        // DELETE: api/UnitRoles/5
        [ResponseType(typeof(UnitRole))]
        public IHttpActionResult DeleteUnitRole(int id)
        {
            UnitRole unitRole = db.UnitRoles.Find(id);
            if (unitRole == null)
            {
                return NotFound();
            }

            db.UnitRoles.Remove(unitRole);
            db.SaveChanges();

            return Ok(unitRole);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UnitRoleExists(int id)
        {
            return db.UnitRoles.Count(e => e.UnitRoleId == id) > 0;
        }
    }
}