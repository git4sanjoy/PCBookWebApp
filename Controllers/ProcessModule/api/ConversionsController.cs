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
using PCBookWebApp.Models.ProcessModule.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using System.Configuration;
using System.Data.SqlClient;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    [Authorize]
    public class ConversionsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Conversions/ConversionsList")]
        [HttpGet]
        [ResponseType(typeof(Conversion))]
        public IHttpActionResult GetProcesseLocationsList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<Conversion> list = new List<Conversion>();
            Conversion aObj = new Conversion();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    ConversionId, ConversionName, ShowRoomId
                                    FROM            
                                    dbo.Conversions
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
                        int id = (int)reader["ConversionId"];
                        string name = (string)reader["ConversionName"];
                        aObj = new Conversion();
                        aObj.ConversionId = id;
                        aObj.ConversionName = name;
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

        // GET: api/Conversions
        [HttpGet]
        public IQueryable<Conversion> GetConversions()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            return db.Conversions.Where(c => c.ShowRoomId == showRoomId);
        }


        // GET: api/Conversions/5
        [ResponseType(typeof(Conversion))]
        public IHttpActionResult GetConversion(int id)
        {
            Conversion conversion = db.Conversions.Find(id);
            if (conversion == null)
            {
                return NotFound();
            }

            return Ok(conversion);
        }

        // PUT: api/Conversions/5
        [ResponseType(typeof(Conversion))]
        
        public async Task<IHttpActionResult> PutConversion(int id, Conversion conversion)
        {
            var msg = 0;
            var check = db.Conversions.FirstOrDefault(m => m.ConversionName == conversion.ConversionName);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != conversion.ConversionId)
            {
                return BadRequest();
            }
            //db.Entry(supplier).State = EntityState.Modified;
            if (check == null)
            {
                try
                {
                    var obj = db.Conversions.FirstOrDefault(m => m.ConversionId == conversion.ConversionId);
                    conversion.DateCreated = obj.DateCreated;
                    conversion.CreatedBy= obj.CreatedBy;
                    conversion.DateUpdated = DateTime.Now;
                    conversion.ShowRoomId = obj.ShowRoomId;
                    conversion.Active = true;

                    db.Conversions.AddOrUpdate(conversion);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConversionExists(id))
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

        // POST: api/Conversions
        [ResponseType(typeof(Conversion))]
        public async Task<IHttpActionResult> PostConversion(Conversion conversion)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            conversion.CreatedBy = userName;
            conversion.ShowRoomId = showRoomId;
            conversion.DateCreated = DateTime.Now;
            conversion.Active = true;
            db.Conversions.Add(conversion);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = conversion.ConversionId }, conversion);
            
        }

        // DELETE: api/Conversions/5
        [ResponseType(typeof(Conversion))]
        public IHttpActionResult DeleteConversion(int id)
        {
            Conversion conversion = db.Conversions.Find(id);
            if (conversion == null)
            {
                return NotFound();
            }

            db.Conversions.Remove(conversion);
            db.SaveChanges();

            return Ok(conversion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ConversionExists(int id)
        {
            return db.Conversions.Count(e => e.ConversionId == id) > 0;
        }
    }
}