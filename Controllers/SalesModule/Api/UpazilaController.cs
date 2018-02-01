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
using PCBookWebApp.Models.SalesModule;
using PCBookWebApp.Models.SalesModule.ViewModel;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class UpazilaController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        // GET: api/Upazila/GetDistrictList/
        [Route("api/Upazila/GetDistrictList")]
        [HttpGet]
        [ResponseType(typeof(UpazilaView))]
        public IHttpActionResult GetDistrictList()
        {
            List<UpazilaView> ImportProductList = new List<UpazilaView>();
            UpazilaView importProduct = new UpazilaView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                        dbo.Upazilas.UpazilaId AS id, dbo.Upazilas.UpazilaName AS name, dbo.Upazilas.DistrictId AS [group], dbo.Districts.DistrictName AS groupName, dbo.Upazilas.UpazilaNameBangla 
                                    FROM            
                                        dbo.Upazilas 
                                    INNER JOIN
                                        dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId";

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
                        string upazilaNameBangla = "";
                        if (reader["UpazilaNameBangla"] != DBNull.Value) {
                            upazilaNameBangla = (string)reader["UpazilaNameBangla"];
                        }
                        importProduct = new UpazilaView();
                        importProduct.id = id;
                        importProduct.name = name;
                        importProduct.group = group;
                        importProduct.groupName = groupName;
                        importProduct.UpazilaNameBangla = upazilaNameBangla;
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

        [Route("api/Upazila/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Upazila))]
        public IHttpActionResult GetDropDownList()
        {
            var list = db.Upazilas.Select(e => new { UpazilaId = e.UpazilaId, UpazilaName = e.UpazilaName, UpazilaNameBangla = e.UpazilaNameBangla });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/Upazila/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(Upazila))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var list = db.Upazilas.Select(e => new { id = e.UpazilaId, text = e.UpazilaName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

















        // GET: api/Upazila
        public IQueryable<Upazila> GetUpazilas()
        {
            return db.Upazilas;
        }

        // GET: api/Upazila/5
        [ResponseType(typeof(Upazila))]
        public async Task<IHttpActionResult> GetUpazila(int id)
        {
            Upazila upazila = await db.Upazilas.FindAsync(id);
            if (upazila == null)
            {
                return NotFound();
            }

            return Ok(upazila);
        }

        // PUT: api/Upazila/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUpazila(int id, Upazila upazila)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != upazila.UpazilaId)
            {
                return BadRequest();
            }

            db.Entry(upazila).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UpazilaExists(id))
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

        // POST: api/Upazila
        [ResponseType(typeof(Upazila))]
        public async Task<IHttpActionResult> PostUpazila(Upazila upazila)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Upazilas.Add(upazila);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = upazila.UpazilaId }, upazila);
        }

        // DELETE: api/Upazila/5
        [ResponseType(typeof(Upazila))]
        public async Task<IHttpActionResult> DeleteUpazila(int id)
        {
            Upazila upazila = await db.Upazilas.FindAsync(id);
            if (upazila == null)
            {
                return NotFound();
            }

            db.Upazilas.Remove(upazila);
            await db.SaveChangesAsync();

            return Ok(upazila);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UpazilaExists(int id)
        {
            return db.Upazilas.Count(e => e.UpazilaId == id) > 0;
        }
    }
}