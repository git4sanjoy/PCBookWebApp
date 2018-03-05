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
using PCBookWebApp.Models;
using PCBookWebApp.Models.SalesModule.ViewModel;
using System.Configuration;
using System.Data.SqlClient;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class UnitManagersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        [Route("api/UnitManagers/UnitManagerList")]
        [HttpGet]
        [ResponseType(typeof(UnitManagerView))]
        public IHttpActionResult GetUnitManagerList()
        {
            List<UnitManagerView> list = new List<UnitManagerView>();
            UnitManagerView aObj = new UnitManagerView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.UnitManagers.UnitManagerId, dbo.UnitManagers.Id, dbo.UnitManagers.UnitId, dbo.UnitManagers.UnitManagerName, dbo.UnitManagers.Address, dbo.UnitManagers.Phone, dbo.UnitManagers.Image, 
                                    dbo.AspNetUsers.FullName, dbo.AspNetUsers.UserImage, dbo.Units.UnitName, dbo.AspNetUsers.Email, dbo.AspNetUsers.UserName
                                    FROM            
                                    dbo.UnitManagers INNER JOIN
                                    dbo.AspNetUsers ON dbo.UnitManagers.Id = dbo.AspNetUsers.Id INNER JOIN
                                    dbo.Units ON dbo.UnitManagers.UnitId = dbo.Units.UnitId";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        string userName = (string)reader["UserName"];
                        string id = (string)reader["Id"];
                        aObj = new UnitManagerView();
                        aObj.UserName = userName;
                        aObj.Id = id;
                        aObj.UnitManagerId = (int)reader["UnitManagerId"];
                        aObj.UnitId = (int)reader["UnitId"];
                        aObj.UnitName = (string)reader["UnitName"]; 
                        if (reader["UnitManagerName"] != DBNull.Value)
                        {
                            aObj.ManagerName = (string)reader["UnitManagerName"];
                        }
                        if (reader["Address"] != DBNull.Value)
                        {
                            aObj.Address = (string)reader["Address"];
                        }
                        if (reader["Phone"] != DBNull.Value)
                        {
                            aObj.Phone = (string)reader["Phone"];
                        }
                        if (reader["Email"] != DBNull.Value)
                        {
                            aObj.Email = (string)reader["Email"];
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


        [Route("api/UnitManagers/UnitsDropDownList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomUser))]
        public IHttpActionResult GetUsersDropDownListt()
        {
            var list = db.Units.Select(e => new { UnitId = e.UnitId, UnitName = e.UnitName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        //[Route("api/UnitManagers/UnitManagerDropDownList")]
        //[HttpGet]
        //[ResponseType(typeof(ShowRoomUser))]
        //public IHttpActionResult GetUnitManagerDropDownList()
        //{
        //    var list = db.UnitManagers.Select(e => new { UnitManagerId = e.UnitManagerId, UnitManagerName = e.UnitManagerName });
        //    if (list == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(list);
        //}


        // GET: api/UnitManagers
        public IQueryable<UnitManager> GetUnitManagers()
        {
            return db.UnitManagers;
        }

        // GET: api/UnitManagers/5
        [ResponseType(typeof(UnitManager))]
        public async Task<IHttpActionResult> GetUnitManager(int id)
        {
            UnitManager unitManager = await db.UnitManagers.FindAsync(id);
            if (unitManager == null)
            {
                return NotFound();
            }

            return Ok(unitManager);
        }

        // PUT: api/UnitManagers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUnitManager(int id, UnitManager unitManager)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != unitManager.UnitManagerId)
            {
                return BadRequest();
            }

            db.Entry(unitManager).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitManagerExists(id))
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

        // POST: api/UnitManagers
        [ResponseType(typeof(UnitManager))]
        public async Task<IHttpActionResult> PostUnitManager(UnitManager unitManager)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UnitManagers.Add(unitManager);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = unitManager.UnitManagerId }, unitManager);
        }

        // DELETE: api/UnitManagers/5
        [ResponseType(typeof(UnitManager))]
        public async Task<IHttpActionResult> DeleteUnitManager(int id)
        {
            UnitManager unitManager = await db.UnitManagers.FindAsync(id);
            if (unitManager == null)
            {
                return NotFound();
            }

            db.UnitManagers.Remove(unitManager);
            await db.SaveChangesAsync();

            return Ok(unitManager);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UnitManagerExists(int id)
        {
            return db.UnitManagers.Count(e => e.UnitManagerId == id) > 0;
        }
    }
}