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
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Controllers.BookModule.api
{
    public class PrimariesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();




        [Route("api/Primaries/GetPrimaryList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetPrimaryList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<XEditGroupView> ImportProductList = new List<XEditGroupView>();
            XEditGroupView importProduct = new XEditGroupView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT PrimaryId AS id, PrimaryName AS name FROM dbo.Primaries";

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
                        importProduct = new XEditGroupView();
                        importProduct.id = id;
                        importProduct.name = name;
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

        [Route("api/Primaries/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(Primary))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var list = db.Primaries.Select(e => new { id = e.PrimaryId, text = e.PrimaryName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/Primaries/GetPrimaryDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Primary))]
        public IHttpActionResult GetPrimaryDropDownList()
        {
            var list = db.Primaries.Select(e => new { PrimaryId = e.PrimaryId, PrimaryName = e.PrimaryName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        // GET: api/Primaries
        public IQueryable<Primary> GetPrimaries()
        {
            return db.Primaries;
        }

        // GET: api/Primaries/5
        [ResponseType(typeof(Primary))]
        public async Task<IHttpActionResult> GetPrimary(int id)
        {
            Primary primary = await db.Primaries.FindAsync(id);
            if (primary == null)
            {
                return NotFound();
            }

            return Ok(primary);
        }

        // PUT: api/Primaries/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPrimary(int id, Primary primary)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != primary.PrimaryId)
            {
                return BadRequest();
            }

            db.Entry(primary).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrimaryExists(id))
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

        // POST: api/Primaries
        [ResponseType(typeof(Primary))]
        public async Task<IHttpActionResult> PostPrimary(Primary primary)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Primaries.Add(primary);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = primary.PrimaryId }, primary);
        }

        // DELETE: api/Primaries/5
        [ResponseType(typeof(Primary))]
        public async Task<IHttpActionResult> DeletePrimary(int id)
        {
            Primary primary = await db.Primaries.FindAsync(id);
            if (primary == null)
            {
                return NotFound();
            }

            db.Primaries.Remove(primary);
            await db.SaveChangesAsync();

            return Ok(primary);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PrimaryExists(int id)
        {
            return db.Primaries.Count(e => e.PrimaryId == id) > 0;
        }
    }
}