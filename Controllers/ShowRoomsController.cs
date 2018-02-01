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
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.ViewModels;

namespace PCBookWebApp.Controllers.api
{
    [Authorize]
    public class ShowRoomsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/ShowRoom/GetDropDownList/
        [Route("api/ShowRooms/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoom))]
        public IHttpActionResult GetDropDownList()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();

            //var list = db.ShowRooms.Select(e => new { ShowRoomId = e.ShowRoomId, ShowRoomName = e.ShowRoomName });
            //if (list == null)
            //{
            //    return NotFound();
            //}
            //return Ok(list);
            List<ShowRoom> userShowRoomList = new List<ShowRoom>();
            ShowRoom showRoomObj = new ShowRoom();

            string connectionString = ConfigurationManager.ConnectionStrings["PCShowRoomAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.ShowRoomUsers.ShowRoomUserId, dbo.ShowRoomUsers.Id, dbo.AspNetUsers.UserName, dbo.ShowRoomUsers.ShowRoomId, dbo.ShowRooms.ShowRoomName, 
                                    dbo.ShowRoomUsers.UserName AS OfficerName, dbo.ShowRoomUsers.Address, dbo.ShowRoomUsers.Phone, dbo.ShowRoomUsers.Email, dbo.ShowRoomUsers.Image, dbo.ShowRooms.UnitId, 
                                    dbo.Units.UnitName
                                    FROM            
                                    dbo.ShowRoomUsers 
                                    INNER JOIN
                                    dbo.ShowRooms ON dbo.ShowRoomUsers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                    dbo.AspNetUsers ON dbo.ShowRoomUsers.Id = dbo.AspNetUsers.Id INNER JOIN
                                    dbo.Units ON dbo.ShowRooms.UnitId = dbo.Units.UnitId
                                    WHERE        
                                    (dbo.ShowRoomUsers.Id = @userId)";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@userId", userId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["ShowRoomId"];
                        string name = (string)reader["ShowRoomName"];
                        showRoomObj = new ShowRoom();
                        showRoomObj.ShowRoomId = id;
                        showRoomObj.ShowRoomName = name;
                        userShowRoomList.Add(showRoomObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(userShowRoomList);
        }

        [Route("api/ShowRooms/GetImportProductsList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomView))]
        public IHttpActionResult GetImportProductsList()
        {
            List<ShowRoomView> ImportProductList = new List<ShowRoomView>();
            ShowRoomView importProduct = new ShowRoomView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                        dbo.ShowRooms.ShowRoomId AS id, dbo.ShowRooms.ShowRoomName AS name, dbo.ShowRooms.UnitId AS [group], dbo.Units.UnitName AS groupName, dbo.ShowRooms.ShowRoomNameBangla
                                    FROM            
                                        dbo.ShowRooms 
                                    INNER JOIN
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
                        int id = (int)reader["id"];
                        string name = (string)reader["name"];
                        int group = (int)reader["group"];
                        string groupName = (string)reader["groupName"];


                        importProduct = new ShowRoomView();
                        importProduct.id = id;
                        importProduct.name = name;
                        importProduct.group = group;
                        importProduct.groupName = groupName;
                        if (reader["ShowRoomNameBangla"] != DBNull.Value) {
                            importProduct.ShowRoomNameBangla = (string)reader["ShowRoomNameBangla"];
                        }
                         
                        ImportProductList.Add(importProduct);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(ImportProductList);
        }



        [Route("api/ShowRooms/GetShowRoomDropDownList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoom))]
        public IHttpActionResult GetShowRoomDropDownList()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();

            //var list = db.ShowRooms.Select(e => new { ShowRoomId = e.ShowRoomId, ShowRoomName = e.ShowRoomName });
            //if (list == null)
            //{
            //    return NotFound();
            //}
            //return Ok(list);
            List<ShowRoom> userShowRoomList = new List<ShowRoom>();
            ShowRoom showRoomObj = new ShowRoom();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.ShowRoomUsers.ShowRoomUserId, dbo.ShowRoomUsers.Id, dbo.AspNetUsers.UserName, dbo.ShowRoomUsers.ShowRoomId, dbo.ShowRooms.ShowRoomName, 
                                    dbo.ShowRoomUsers.UserName AS OfficerName, dbo.ShowRoomUsers.Address, dbo.ShowRoomUsers.Phone, dbo.ShowRoomUsers.Email, dbo.ShowRoomUsers.Image, dbo.ShowRooms.UnitId, 
                                    dbo.Units.UnitName, dbo.ShowRooms.ShowRoomNameBangla
                                    FROM            
                                    dbo.ShowRoomUsers INNER JOIN
                                    dbo.ShowRooms ON dbo.ShowRoomUsers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                    dbo.AspNetUsers ON dbo.ShowRoomUsers.Id = dbo.AspNetUsers.Id INNER JOIN
                                    dbo.Units ON dbo.ShowRooms.UnitId = dbo.Units.UnitId
                                    WHERE        
                                    (dbo.ShowRoomUsers.Id = @userId)";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@userId", userId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["ShowRoomId"];
                        string name = (string) reader["ShowRoomName"];
                        string showRoomNameBangla = "";
                        if (reader["ShowRoomNameBangla"] != null) {
                            showRoomNameBangla = (string)reader["ShowRoomNameBangla"];
                        }
                        showRoomObj = new ShowRoom();
                        showRoomObj.ShowRoomId = id;
                        showRoomObj.ShowRoomName = name;
                        showRoomObj.ShowRoomNameBangla = showRoomNameBangla;
                        userShowRoomList.Add(showRoomObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(userShowRoomList);
        }



        [Route("api/ShowRooms/GetAllShowRoom")]
        [HttpGet]
        [ResponseType(typeof(ShowRoom))]
        public IHttpActionResult GetAllShowRoom()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers
                            .Where(u => u.Id == currentUserId)
                            .Select(u => u.ShowRoomId)
                            .FirstOrDefault();
            var unitId = db.ShowRooms
                .Where(u => u.ShowRoomId == showRoomId)
                .Select(u => u.UnitId)
                .FirstOrDefault();

            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                var list = db.ShowRooms.Select(e => new { ShowRoomId = e.ShowRoomId, ShowRoomName = e.ShowRoomName });
                if (list == null)
                {
                    return NotFound();
                }
                return Ok(list);
            }
            else
            {
                var list = db.ShowRoomUsers.Where(a => a.Id == currentUserId)
                    .Select(e => new { ShowRoomId = e.ShowRoomId, ShowRoomName = e.ShowRoom.ShowRoomName });
                if (list == null)
                {
                    return NotFound();
                }
                return Ok(list);
            }
        }

        [Route("api/ShowRooms/GetShowRoomByUnitId/{UnitId}")]
        [HttpGet]
        [ResponseType(typeof(ShowRoom))]
        public IHttpActionResult GetShowRoomByUnitId(int UnitId)
        {
            var list = db.ShowRooms
                            .Where(a => a.UnitId == UnitId)
                            .Select(e => new { ShowRoomId = e.ShowRoomId, ShowRoomName = e.ShowRoomName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }



        [AllowAnonymous]
        [Route("api/ShowRooms/GetShowRoomsDropdownList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoom))]
        public IHttpActionResult GetShowRoomsDropdownList()
        {
            var list = db.ShowRooms.Select(e => new { id = e.ShowRoomId, text = e.ShowRoomName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }





        // GET: api/ShowRooms
        public IQueryable<ShowRoom> GetShowRooms()
        {
            return db.ShowRooms;
        }

        // GET: api/ShowRooms/5
        [ResponseType(typeof(ShowRoom))]
        public async Task<IHttpActionResult> GetShowRoom(int id)
        {
            ShowRoom showRoom = await db.ShowRooms.FindAsync(id);
            if (showRoom == null)
            {
                return NotFound();
            }

            return Ok(showRoom);
        }

        // PUT: api/ShowRooms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutShowRoom(int id, ShowRoom showRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != showRoom.ShowRoomId)
            {
                return BadRequest();
            }

            db.Entry(showRoom).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowRoomExists(id))
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

        // POST: api/ShowRooms
        [ResponseType(typeof(ShowRoom))]
        public async Task<IHttpActionResult> PostShowRoom(ShowRoom showRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ShowRooms.Add(showRoom);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = showRoom.ShowRoomId }, showRoom);
        }

        // DELETE: api/ShowRooms/5
        [ResponseType(typeof(ShowRoom))]
        public async Task<IHttpActionResult> DeleteShowRoom(int id)
        {
            ShowRoom showRoom = await db.ShowRooms.FindAsync(id);
            if (showRoom == null)
            {
                return NotFound();
            }

            db.ShowRooms.Remove(showRoom);
            await db.SaveChangesAsync();

            return Ok(showRoom);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShowRoomExists(int id)
        {
            return db.ShowRooms.Count(e => e.ShowRoomId == id) > 0;
        }
    }
}