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
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.SalesModule;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class SalesMenController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/SalesMen/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(SalesMan))]
        public IHttpActionResult GetDropDownList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
            var list = db.SalesMen
                .Where(e=> e.ShowRoom.UnitId== unitId)
                .Select(e => new { SalesManId = e.SalesManId, SalesManName = e.SalesManName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/SalesMen/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(SalesMan))]
        public IHttpActionResult GetDropDownListXedit()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
            var list = db.SalesMen.Where(e => e.ShowRoom.UnitId == unitId).Select(e => new { id = e.SalesManId, text = e.SalesManName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        [Route("api/SalesMen/SalesMenList")]
        [HttpGet]
        [ResponseType(typeof(SalesMan))]
        public IHttpActionResult SalesMenList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
            var list = db.SalesMen.Where(e => e.ShowRoom.UnitId == unitId).Select(e => new { value = e.SalesManId, text = e.SalesManName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }





        [Route("api/SalesMen/ShowRoomSalesMenList")]
        [HttpGet]
        public IHttpActionResult ShowRoomSalesMenList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
            List<SalesMan> list = new List<SalesMan>();
            SalesMan aObj = new SalesMan();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.SalesMen.SalesManId, dbo.SalesMen.SalesManName, dbo.SalesMen.Address, dbo.SalesMen.Phone, dbo.SalesMen.Email, dbo.SalesMen.Active, dbo.SalesMen.CreatedBy, dbo.SalesMen.DateCreated, 
                                    dbo.SalesMen.DateUpdated, dbo.SalesMen.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.ShowRooms.UnitId, dbo.Units.UnitName
                                    FROM            
                                    dbo.SalesMen INNER JOIN
                                    dbo.ShowRooms ON dbo.SalesMen.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                    dbo.Units ON dbo.ShowRooms.UnitId = dbo.Units.UnitId
                                    WHERE        
                                    (dbo.ShowRooms.UnitId = @UnitId)";
            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@UnitId", unitId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["SalesManId"];
                        string name = (string)reader["SalesManName"];
                        aObj = new SalesMan();
                        aObj.SalesManId = id;
                        aObj.SalesManName = name;
                        aObj.ShowRoomId = (int)reader["ShowRoomId"];
                        //aObj.ShowRoomName = (string)reader["ShowRoomName"];
                        if (reader["Address"] != System.DBNull.Value) { aObj.Address = (string)reader["Address"]; }
                        if (reader["Phone"] != System.DBNull.Value) { aObj.Phone = (string)reader["Phone"]; }
                        if (reader["Email"] != System.DBNull.Value) { aObj.Email = (string)reader["Email"]; }
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


        // GET: api/SalesMen
        public IQueryable<SalesMan> GetSalesMen()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            return db.SalesMen.Where(s => s.ShowRoomId == showRoomId);
        }

        // GET: api/SalesMen/5
        [ResponseType(typeof(SalesMan))]
        public async Task<IHttpActionResult> GetSalesMan(int id)
        {
            SalesMan salesMan = await db.SalesMen.FindAsync(id);
            if (salesMan == null)
            {
                return NotFound();
            }

            return Ok(salesMan);
        }

        // PUT: api/SalesMen/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSalesMan(int id, SalesMan salesMan)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            salesMan.ShowRoomId = showRoomId;
            salesMan.DateCreated = ceatedAt;
            salesMan.DateUpdated = ceatedAt;
            salesMan.CreatedBy = userName;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesMan.SalesManId)
            {
                return BadRequest();
            }

            db.Entry(salesMan).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesManExists(id))
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

        // POST: api/SalesMen
        [ResponseType(typeof(SalesMan))]
        public async Task<IHttpActionResult> PostSalesMan(SalesMan salesMan)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            salesMan.ShowRoomId = showRoomId;
            salesMan.DateCreated = ceatedAt;
            salesMan.DateUpdated = ceatedAt;
            salesMan.CreatedBy = userName;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SalesMen.Add(salesMan);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = salesMan.SalesManId }, salesMan);

            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString))
            //{
            //    con.Open();
            //    string sql = @"INSERT INTO dbo.SalesMen (SalesManName, Address, Phone, Email, ShowRoomId, Active, CreatedBy, DateCreated, DateUpdated) 
            //                                      VALUES(@SalesManName, @Address, @Phone, @Email, @ShowRoomId, @active, @CreatedBy, @DateCreated,@DateUpdated)";
            //    SqlCommand cmd = new SqlCommand(sql, con);

            //    cmd.Parameters.Add("@SalesManName", SqlDbType.VarChar, 145).Value = salesMan.SalesManName;
            //    cmd.Parameters.Add("@Address", SqlDbType.VarChar, 145).Value = salesMan.Address;
            //    cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 145).Value = salesMan.Phone;
            //    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 145).Value = salesMan.Email;
            //    cmd.Parameters.Add("@ShowRoomId", SqlDbType.Int).Value = showRoomId;
            //    cmd.Parameters.Add("@active", SqlDbType.Bit).Value = false;
            //    cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, 145).Value =userName;
            //    cmd.Parameters.Add("@DateCreated", SqlDbType.DateTime).Value = ceatedAt;
            //    cmd.Parameters.Add("@DateUpdated", SqlDbType.DateTime).Value =ceatedAt;

            //    cmd.CommandType = CommandType.Text;
            //    cmd.ExecuteNonQuery();
            //    con.Close();
            //}

            //return Ok();

        }

        // DELETE: api/SalesMen/5
        [ResponseType(typeof(SalesMan))]
        public async Task<IHttpActionResult> DeleteSalesMan(int id)
        {
            SalesMan salesMan = await db.SalesMen.FindAsync(id);
            if (salesMan == null)
            {
                return NotFound();
            }

            db.SalesMen.Remove(salesMan);
            await db.SaveChangesAsync();

            return Ok(salesMan);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalesManExists(int id)
        {
            return db.SalesMen.Count(e => e.SalesManId == id) > 0;
        }
    }
}