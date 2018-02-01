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
    public class UnitsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [AllowAnonymous]
        [Route("api/Units/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var unitList = db.Units.Select(e => new { id = e.UnitId, text = e.UnitName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }

        // GET: api/Units/GetDropDownList/
        [Route("api/Units/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetDropDownList()
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
                var unitList = db.Units.Select(e => new { UnitId = e.UnitId, UnitName = e.UnitName });
                if (unitList == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(unitList);
                }
            }
            else
            {
                var unitList = db.Units.Where(a => a.UnitId == unitId)
                    .Select(e => new { UnitId = e.UnitId, UnitName = e.UnitName });
                if (unitList == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(unitList);
                }
            }
        }

        //Custom Method
        [Route("api/Units/GetUnitList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetUnitList()
        {
            List<XEditGroupView> ImportProductList = new List<XEditGroupView>();
            XEditGroupView importProduct = new XEditGroupView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.Units.UnitId AS id, dbo.Units.UnitName AS name, dbo.Units.ProjectId AS [group], dbo.Projects.ProjectName AS groupName, dbo.Units.Address, dbo.Units.Email, dbo.Units.Phone, dbo.Units.Website
                                    FROM            
                                    dbo.Units INNER JOIN
                                    dbo.Projects ON dbo.Units.ProjectId = dbo.Projects.ProjectId";

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

                        importProduct = new XEditGroupView();
                        importProduct.id = id;
                        importProduct.name = name;
                        importProduct.group = group;
                        importProduct.groupName = groupName;
                        if (reader["Address"] != System.DBNull.Value)
                        {
                            importProduct.Address = reader["Address"].ToString();
                        }
                        if (reader["Email"] != System.DBNull.Value)
                        {
                            importProduct.Email = reader["Email"].ToString();
                        }
                        if (reader["Phone"] != System.DBNull.Value)
                        {
                            importProduct.Phone = reader["Phone"].ToString();
                        }
                        if (reader["Website"] != System.DBNull.Value)
                        {
                            importProduct.Website = reader["Website"].ToString();
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

        //[AllowAnonymous]
        //[Route("api/Units/GetDropDownListXedit")]
        //[HttpGet]
        //[ResponseType(typeof(Project))]
        //public IHttpActionResult GetDropDownListXedit()
        //{
        //    var list = db.Units.Select(e => new { id = e.UnitId, text = e.UnitName });
        //    if (list == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(list);
        //}


        // GET: api/Units
        public IQueryable<Unit> GetUnits()
        {
            return db.Units;
        }

        // GET: api/Units/5
        [ResponseType(typeof(Unit))]
        public async Task<IHttpActionResult> GetUnit(int id)
        {
            Unit unit = await db.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            return Ok(unit);
        }

        // PUT: api/Units/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUnit(int id, Unit unit)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            unit.CreatedBy = userName;
            unit.DateCreated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != unit.UnitId)
            {
                return BadRequest();
            }

            db.Entry(unit).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitExists(id))
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

        // POST: api/Units
        [ResponseType(typeof(Unit))]
        public async Task<IHttpActionResult> PostUnit(Unit unit)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            unit.CreatedBy = userName;
            unit.DateCreated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Units.Add(unit);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = unit.UnitId }, unit);
        }

        // DELETE: api/Units/5
        [ResponseType(typeof(Unit))]
        public async Task<IHttpActionResult> DeleteUnit(int id)
        {
            Unit unit = await db.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            db.Units.Remove(unit);
            await db.SaveChangesAsync();

            return Ok(unit);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UnitExists(int id)
        {
            return db.Units.Count(e => e.UnitId == id) > 0;
        }
    }
}