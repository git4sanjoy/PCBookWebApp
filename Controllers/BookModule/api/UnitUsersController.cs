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
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.Identity;

namespace PCBookWebApp.Controllers
{
    [Authorize]
    public class UnitUsersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/UnitUsers/GetUsersXeditDropDownList")]
        [HttpGet]
        [ResponseType(typeof(UserView))]
        public IHttpActionResult GetUsersXeditDropDownList()
        {
            var context = new ApplicationDbContext();
            var list = context.Users.Select(e => new { id = e.Id, text = e.UserName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);

        }


        [Route("api/UnitUsers/GetUnitsXeditDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetDepartmentsXeditDropDownList()
        {
            var list = db.Units.Select(e => new { value = e.UnitId, text = e.UnitName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/UnitUsers/GetUnitUsersList")]
        [HttpGet]
        [ResponseType(typeof(UnitUserView))]
        public IHttpActionResult GetUnitUsersList()
        {
            List<UnitUserView> userUnitList = new List<UnitUserView>();
            UnitUserView userUnit = new UnitUserView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.UnitUsers.Id, dbo.AspNetUsers.UserName, dbo.UnitUsers.UnitId, dbo.Units.UnitName, dbo.AspNetUsers.FullName, dbo.UnitUsers.UnitUserId
                                    FROM            
                                    dbo.Units INNER JOIN
                                    dbo.UnitUsers ON dbo.Units.UnitId = dbo.UnitUsers.UnitId INNER JOIN
                                    dbo.AspNetUsers ON dbo.UnitUsers.Id = dbo.AspNetUsers.Id";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int userUnitId = (int)reader["UnitUserId"];
                        int unitId = (int)reader["UnitId"];
                        string unitName = (string)reader["UnitName"];
                        string id = (string)reader["Id"];
                        string userName = (string)reader["UserName"];
                        string fullName = (string)reader["FullName"];
                        userUnit = new UnitUserView();
                        userUnit.UnitUserId = userUnitId;
                        userUnit.Id = id;
                        userUnit.UserName = userName;
                        userUnit.UnitId = unitId;
                        userUnit.UnitName = unitName;
                        userUnit.FullName = fullName;
                        userUnitList.Add(userUnit);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(userUnitList);
        }


        // GET: api/UnitUsers
        public IQueryable<UnitUser> GetUnitUsers()
        {
            return db.UnitUsers;
        }

        // GET: api/UnitUsers/5
        [ResponseType(typeof(UnitUser))]
        public async Task<IHttpActionResult> GetUnitUser(int id)
        {
            UnitUser unitUser = await db.UnitUsers.FindAsync(id);
            if (unitUser == null)
            {
                return NotFound();
            }

            return Ok(unitUser);
        }

        // PUT: api/UnitUsers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUnitUser(int id, UnitUser unitUser)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            unitUser.CreatedBy = userName;
            unitUser.DateCreated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != unitUser.UnitUserId)
            {
                return BadRequest();
            }

            db.Entry(unitUser).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitUserExists(id))
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

        // POST: api/UnitUsers
        [ResponseType(typeof(UnitUser))]
        public async Task<IHttpActionResult> PostUnitUser(UnitUser unitUser)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            unitUser.CreatedBy = userName;
            unitUser.DateCreated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UnitUsers.Add(unitUser);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = unitUser.UnitUserId }, unitUser);
        }

        // DELETE: api/UnitUsers/5
        [ResponseType(typeof(UnitUser))]
        public async Task<IHttpActionResult> DeleteUnitUser(int id)
        {
            UnitUser unitUser = await db.UnitUsers.FindAsync(id);
            if (unitUser == null)
            {
                return NotFound();
            }

            db.UnitUsers.Remove(unitUser);
            await db.SaveChangesAsync();

            return Ok(unitUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UnitUserExists(int id)
        {
            return db.UnitUsers.Count(e => e.UnitUserId == id) > 0;
        }
    }
}