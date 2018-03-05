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
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Configuration;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    [Authorize]
    public class ProcesseLocationsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/ProcesseLocations/ProcesseLocationsMultiSelectList")]
        [HttpGet]
        public IHttpActionResult GetProcesseLocationsMultiSelectList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.ProcesseLocations
                            .Where(d => d.ShowRoomId == showRoomId)
                            .OrderBy(d => d.ProcesseLocationName)
                            .Select(e => new {
                                id = e.ProcesseLocationId,
                                label = e.ProcesseLocationName
                            });

            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }
        [Route("api/ProcesseLocations/GetProcesseLocationsList")]
        [HttpGet]
        [ResponseType(typeof(ProcesseLocation))]
        public IHttpActionResult GetProcesseLocationsList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<ProcesseLocation> list = new List<ProcesseLocation>();
            ProcesseLocation aObj = new ProcesseLocation();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    ProcesseLocationId, ProcesseLocationName, ShowRoomId
                                    FROM            
                                    dbo.ProcesseLocations
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
                        int id = (int)reader["ProcesseLocationId"];
                        string name = (string)reader["ProcesseLocationName"];
                        aObj = new ProcesseLocation();
                        aObj.ProcesseLocationId = id;
                        aObj.ProcesseLocationName = name;
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

        // GET: api/ProcesseLocations
        public IQueryable<ProcesseLocation> GetProcesseLocations()
        {
            return db.ProcesseLocations;
        }

        // GET: api/ProcesseLocations/5
        [ResponseType(typeof(ProcesseLocation))]
        public IHttpActionResult GetProcesseLocation(int id)
        {
            ProcesseLocation processeLocation = db.ProcesseLocations.Find(id);
            if (processeLocation == null)
            {
                return NotFound();
            }

            return Ok(processeLocation);
        }

        // PUT: api/ProcesseLocations/5
        [ResponseType(typeof(ProcesseLocation))]
        
        public async Task<IHttpActionResult> PutProcesseLocation(int id, ProcesseLocation processeLocation)
        {
            var msg = 0;
            var check = db.ProcesseLocations.FirstOrDefault(m => m.ProcesseLocationName == processeLocation.ProcesseLocationName);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (id != processeLocation.ProcesseLocationId)
            {
                return BadRequest();
            }
            if (check == null)
            {
                try
                {
                    var obj = db.ProcesseLocations.Where(x => x.ProcesseLocationId == id).FirstOrDefault();
                    processeLocation.CreatedBy = obj.CreatedBy;
                    processeLocation.DateCreated = obj.DateCreated;
                    processeLocation.DateUpdated = DateTime.Now;
                    processeLocation.Active = true;
                    processeLocation.ShowRoomId = obj.ShowRoomId;
                    db.ProcesseLocations.AddOrUpdate(processeLocation);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcesseLocationExists(id))
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
        }

        // POST: api/ProcesseLocations
        [ResponseType(typeof(ProcesseLocation))]
        public IHttpActionResult PostProcesseLocation(ProcesseLocation processeLocation)
        {
            try
            {
                string userId = User.Identity.GetUserId();
                var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
                string userName = User.Identity.GetUserName();
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                processeLocation.CreatedBy = userName;
                processeLocation.ShowRoomId = showRoomId;
                processeLocation.DateCreated = DateTime.Now;
                processeLocation.Active = true;

                db.ProcesseLocations.Add(processeLocation);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            

            return CreatedAtRoute("DefaultApi", new { id = processeLocation.ProcesseLocationId }, processeLocation);
        }

        // DELETE: api/ProcesseLocations/5
        [ResponseType(typeof(ProcesseLocation))]
        public IHttpActionResult DeleteProcesseLocation(int id)
        {
            ProcesseLocation processeLocation = db.ProcesseLocations.Find(id);
            if (processeLocation == null)
            {
                return NotFound();
            }

            db.ProcesseLocations.Remove(processeLocation);
            db.SaveChanges();

            return Ok(processeLocation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProcesseLocationExists(int id)
        {
            return db.ProcesseLocations.Count(e => e.ProcesseLocationId == id) > 0;
        }
    }
}