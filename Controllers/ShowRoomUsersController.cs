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

namespace PCBookWebApp.Controllers.api
{
    public class ShowRoomUsersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/ShowRoomUsers/GetUsersDropDownList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomUser))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var context = new ApplicationDbContext();

            var usersList = context.Users.Select(e => new { Id = e.Id, UserName = e.UserName });
            if (usersList == null)
            {
                return NotFound();
            }
            return Ok(usersList);
        }


        [Route("api/ShowRoomUsers/ShowRoomOfficerList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomUserView))]
        public IHttpActionResult ShowRoomOfficerList()
        {
            List<ShowRoomUserView> showRoomOfficerList = new List<ShowRoomUserView>();
            ShowRoomUserView officerObj = new ShowRoomUserView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.ShowRoomUsers.ShowRoomUserId, dbo.ShowRoomUsers.Id, dbo.AspNetUsers.UserName, dbo.ShowRoomUsers.ShowRoomId, dbo.ShowRooms.ShowRoomName, 
                                    dbo.ShowRoomUsers.UserName AS OfficerName, dbo.ShowRoomUsers.Address, dbo.ShowRoomUsers.Phone, dbo.ShowRoomUsers.Email, dbo.ShowRoomUsers.Image, dbo.ShowRooms.UnitId, 
                                    dbo.Units.UnitName
                                    FROM            
                                    dbo.ShowRoomUsers 
                                    INNER JOIN
                                    dbo.ShowRooms ON dbo.ShowRoomUsers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                    dbo.AspNetUsers ON dbo.ShowRoomUsers.Id = dbo.AspNetUsers.Id INNER JOIN
                                    dbo.Units ON dbo.ShowRooms.UnitId = dbo.Units.UnitId";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int showRoomUserId = (int)reader["ShowRoomUserId"];
                        int showRoomId = (int)reader["ShowRoomId"];
                        string userName = (string)reader["UserName"];
                        string id = (string)reader["Id"];
                        string showroomName = (string)reader["ShowRoomName"];

                        officerObj = new ShowRoomUserView();
                        officerObj.ShowRoomUserId = showRoomUserId;
                        officerObj.UserName = userName;
                        officerObj.Id = id;
                        officerObj.ShowRoomName = showroomName;
                        officerObj.ShowRoomId = showRoomId;

                        officerObj.UnitId = (int)reader["UnitId"];
                        officerObj.UnitName = (string)reader["UnitName"];

                        if (reader["OfficerName"] != DBNull.Value)
                        {
                            officerObj.OfficerName = (string)reader["OfficerName"];
                        }
                        if (reader["Address"] != DBNull.Value)
                        {
                            officerObj.Address = (string)reader["Address"];
                        }
                        if (reader["Phone"] != DBNull.Value)
                        {
                            officerObj.Phone = (string)reader["Phone"];
                        }
                        if (reader["Email"] != DBNull.Value)
                        {
                            officerObj.Email = (string)reader["Email"];
                        }
                        showRoomOfficerList.Add(officerObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(showRoomOfficerList);
        }







        // GET: api/ShowRoomUsers
        public IQueryable<ShowRoomUser> GetShowRoomUsers()
        {
            return db.ShowRoomUsers;
        }

        // GET: api/ShowRoomUsers/5
        [ResponseType(typeof(ShowRoomUser))]
        public async Task<IHttpActionResult> GetShowRoomUser(int id)
        {
            ShowRoomUser showRoomUser = await db.ShowRoomUsers.FindAsync(id);
            if (showRoomUser == null)
            {
                return NotFound();
            }

            return Ok(showRoomUser);
        }

        // PUT: api/ShowRoomUsers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutShowRoomUser(int id, ShowRoomUser showRoomUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != showRoomUser.ShowRoomUserId)
            {
                return BadRequest();
            }

            db.Entry(showRoomUser).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowRoomUserExists(id))
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

        // POST: api/ShowRoomUsers
        [ResponseType(typeof(ShowRoomUser))]
        public async Task<IHttpActionResult> PostShowRoomUser(ShowRoomUser showRoomUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ShowRoomUsers.Add(showRoomUser);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = showRoomUser.ShowRoomUserId }, showRoomUser);
        }

        // DELETE: api/ShowRoomUsers/5
        [ResponseType(typeof(ShowRoomUser))]
        public async Task<IHttpActionResult> DeleteShowRoomUser(int id)
        {
            ShowRoomUser showRoomUser = await db.ShowRoomUsers.FindAsync(id);
            if (showRoomUser == null)
            {
                return NotFound();
            }

            db.ShowRoomUsers.Remove(showRoomUser);
            await db.SaveChangesAsync();

            return Ok(showRoomUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShowRoomUserExists(int id)
        {
            return db.ShowRoomUsers.Count(e => e.ShowRoomUserId == id) > 0;
        }
    }
}