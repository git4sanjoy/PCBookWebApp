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
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.SqlClient;
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Controllers.BookModule.api
{
    [Authorize]
    public class VoucherTypesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        [Route("api/VoucherTypes/GetVoucherTypeList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetMatricList()
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();
            List<XEditGroupView> subDepartmentList = new List<XEditGroupView>();
            XEditGroupView subDepartment = new XEditGroupView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT  VoucherTypeId AS id, VoucherTypeName AS name, CreatedBy
                                    FROM            
                                    dbo.VoucherTypes
                                    ORDER BY  name";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                //command.Parameters.Add(new SqlParameter("@userName", currentUserName));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        string name = (string)reader["name"];


                        subDepartment = new XEditGroupView();
                        subDepartment.id = id;
                        subDepartment.name = name;

                        subDepartmentList.Add(subDepartment);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(subDepartmentList);
        }

        [Route("api/VoucherTypes/GetVoucherTypeListXEdit")]
        [HttpGet]
        [ResponseType(typeof(VoucherType))]
        public IHttpActionResult GetMatricsListXEdit()
        {
            var list = db.VoucherTypes.Select(e => new { value = e.VoucherTypeId, text = e.VoucherTypeName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/VoucherTypes/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Group))]
        public IHttpActionResult GetDropDownList()
        {
            var list = db.VoucherTypes
                .Select(e => new { VoucherTypeId = e.VoucherTypeId, VoucherTypeName = e.VoucherTypeName })
                .OrderBy(e => e.VoucherTypeName );
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }



        // GET: api/VoucherTypes
        public IQueryable<VoucherType> GetVoucherTypes()
        {
            return db.VoucherTypes;
        }

        // GET: api/VoucherTypes/5
        [ResponseType(typeof(VoucherType))]
        public async Task<IHttpActionResult> GetVoucherType(int id)
        {
            VoucherType voucherType = await db.VoucherTypes.FindAsync(id);
            if (voucherType == null)
            {
                return NotFound();
            }

            return Ok(voucherType);
        }

        // PUT: api/VoucherTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVoucherType(int id, VoucherType voucherType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();



            // Get Previous Obj Instance
            string userName = User.Identity.GetUserName();
            DateTime updateAt = DateTime.Now;
            bool Active = false;
            string CreatedBy = null;
            DateTime DateCreated = DateTime.Now;
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                SqlDataReader reader = null;
                string sql = @"SELECT dbo.VoucherTypes.* FROM   dbo.VoucherTypes WHERE VoucherTypeId=@id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Active = (bool)reader["Active"];
                    CreatedBy = (string)reader["CreatedBy"];
                    DateCreated = (DateTime)reader["DateCreated"];
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //voucherType.ShowRoomId = showRoomId;
            voucherType.Active = Active;
            voucherType.DateCreated = DateCreated;
            voucherType.DateUpdated = updateAt;
            voucherType.CreatedBy = CreatedBy;
            // End 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != voucherType.VoucherTypeId)
            {
                return BadRequest();
            }

            db.Entry(voucherType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoucherTypeExists(id))
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

        // POST: api/VoucherTypes
        [ResponseType(typeof(VoucherType))]
        public async Task<IHttpActionResult> PostVoucherType(VoucherType voucherType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            voucherType.CreatedBy = userName;
            voucherType.DateCreated = createdAt;
            voucherType.DateUpdated = createdAt;
            //voucherType.ShowRoomId = showRoomId;
            if (db.VoucherTypes.Any(m => m.VoucherTypeName == voucherType.VoucherTypeName && m.CreatedBy == userName))
            {
                ModelState.AddModelError("VoucherTypeName", "Voucher Type Name Already Exists!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.VoucherTypes.Add(voucherType);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = voucherType.VoucherTypeId }, voucherType);
        }

        // DELETE: api/VoucherTypes/5
        [ResponseType(typeof(VoucherType))]
        public async Task<IHttpActionResult> DeleteVoucherType(int id)
        {
            VoucherType voucherType = await db.VoucherTypes.FindAsync(id);
            if (voucherType == null)
            {
                return NotFound();
            }

            db.VoucherTypes.Remove(voucherType);
            await db.SaveChangesAsync();

            return Ok(voucherType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VoucherTypeExists(int id)
        {
            return db.VoucherTypes.Count(e => e.VoucherTypeId == id) > 0;
        }
    }
}