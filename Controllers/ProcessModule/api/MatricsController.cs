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
using PCBookWebApp.Models.ProcessModule;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using System.Configuration;
using System.Data.SqlClient;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    //[Authorize]
    public class MatricsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        [Route("api/Matrics/GetMatricsList")]
        [HttpGet]
        [ResponseType(typeof(Matric))]
        public IHttpActionResult GetMatricsList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<Matric> list = new List<Matric>();
            Matric aObj = new Matric();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    MatricId, MatricName, ShowRoomId
                                    FROM            
                                    dbo.Matrics
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
                        int id = (int)reader["MatricId"];
                        string name = (string)reader["MatricName"];
                        aObj = new Matric();
                        aObj.MatricId = id;
                        aObj.MatricName = name;
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


        // GET: api/Matrics
        public IQueryable<Matric> GetMatrics()
        {
            return db.Matrics;
        }

        // GET: api/Matrics/5
        [ResponseType(typeof(Matric))]
        public async Task<IHttpActionResult> GetMatric(int id)
        {
            Matric matric = await db.Matrics.FindAsync(id);
            if (matric == null)
            {
                return NotFound();
            }

            return Ok(matric);
        }

        // PUT: api/Matrics/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMatric(int id, Matric matric)
        {
           
            var msg = 0;
            var check = db.Matrics.FirstOrDefault(m => m.MatricName == matric.MatricName);
            //if (!ModelState.IsValid)
            
                //    return BadRequest(ModelState);
                //}

                if (id != matric.MatricId)
                {
                    return BadRequest();
                }

                //db.Entry(matric).State = EntityState.Modified;

                if (check == null)
                {
                    try
                    {
                    var obj = db.Matrics.FirstOrDefault(m => m.MatricId == matric.MatricId);
                    matric.CreatedBy = obj.CreatedBy;
                    matric.DateCreated = obj.DateCreated;
                    matric.DateUpdated = DateTime.Now;
                        matric.ShowRoomId = obj.ShowRoomId;
                        matric.Active = true;
                        db.Matrics.AddOrUpdate(matric);
                        await db.SaveChangesAsync();
                        msg = 1;
                }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MatricExists(id))
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
          
        
        // POST: api/Matrics
        [ResponseType(typeof(Matric))]
        public async Task<IHttpActionResult> PostMatric(Matric matric)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();
          
          
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            bool isTrue = db.Matrics.Any(s=>s.MatricName == matric.MatricName.Trim());
            if(isTrue==false)
            {
                matric.ShowRoomId = showRoomId;
                matric.CreatedBy = userName;
                matric.DateCreated = DateTime.Now;
                matric.DateCreated = matric.DateCreated;
                matric.Active = true;
                db.Matrics.Add(matric);
                await db.SaveChangesAsync();
            }
           

            return CreatedAtRoute("DefaultApi", new { id = matric.MatricId }, matric);
        }

        // DELETE: api/Matrics/5
        [ResponseType(typeof(Matric))]
        public async Task<IHttpActionResult> DeleteMatric(int id)
        {
            Matric matric = await db.Matrics.FindAsync(id);
            if (matric == null)
            {
                return NotFound();
            }

            db.Matrics.Remove(matric);
            await db.SaveChangesAsync();

            return Ok(matric);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MatricExists(int id)
        {
            return db.Matrics.Count(e => e.MatricId == id) > 0;
        }
    }
}