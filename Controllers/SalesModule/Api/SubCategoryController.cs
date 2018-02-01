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
using PCBookWebApp.Models.SalesModule;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class SubCategoryController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        // GET: api/SubCategory/GetSubCategoryList/
        [Route("api/SubCategory/GetSubCategoryList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomView))]
        public IHttpActionResult GetSubCategoryList()
        {
            List<ShowRoomView> ImportProductList = new List<ShowRoomView>();
            ShowRoomView importProduct = new ShowRoomView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                        dbo.SubCategories.SubCategoryId AS id, dbo.SubCategories.SubCategoryName AS name, dbo.SubCategories.MainCategoryId AS [group], dbo.MainCategories.MainCategoryName AS groupName
                                    FROM            
                                        dbo.SubCategories 
                                    INNER JOIN
                                        dbo.MainCategories ON dbo.SubCategories.MainCategoryId = dbo.MainCategories.MainCategoryId";

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

        // GET: api/SubCategory/GetDropDownList/
        [Route("api/SubCategory/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(SubCategory))]
        public IHttpActionResult GetDropDownList()
        {
            var list = db.SubCategories.Select(e => new { SubCategoryId = e.SubCategoryId, SubCategoryName = e.SubCategoryName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        [Route("api/SubCategory/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(SubCategory))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var unitList = db.SubCategories.Select(e => new { id = e.SubCategoryId, text = e.SubCategoryName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }











        // GET: api/SubCategory
        public IQueryable<SubCategory> GetSubCategories()
        {
            return db.SubCategories;
        }

        // GET: api/SubCategory/5
        [ResponseType(typeof(SubCategory))]
        public async Task<IHttpActionResult> GetSubCategory(int id)
        {
            SubCategory subCategory = await db.SubCategories.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return Ok(subCategory);
        }

        // PUT: api/SubCategory/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSubCategory(int id, SubCategory subCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subCategory.SubCategoryId)
            {
                return BadRequest();
            }

            db.Entry(subCategory).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubCategoryExists(id))
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

        // POST: api/SubCategory
        [ResponseType(typeof(SubCategory))]
        public async Task<IHttpActionResult> PostSubCategory(SubCategory subCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SubCategories.Add(subCategory);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = subCategory.SubCategoryId }, subCategory);
        }

        // DELETE: api/SubCategory/5
        [ResponseType(typeof(SubCategory))]
        public async Task<IHttpActionResult> DeleteSubCategory(int id)
        {
            SubCategory subCategory = await db.SubCategories.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            db.SubCategories.Remove(subCategory);
            await db.SaveChangesAsync();

            return Ok(subCategory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SubCategoryExists(int id)
        {
            return db.SubCategories.Count(e => e.SubCategoryId == id) > 0;
        }
    }
}