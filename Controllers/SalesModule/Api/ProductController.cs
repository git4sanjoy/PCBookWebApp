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
using PCBookWebApp.Models.SalesModule.ViewModel;
using System.Data.Entity.Migrations;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class ProductController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Product/GetProductsList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomView))]
        public IHttpActionResult GetProductsList()
        {
            string userId = User.Identity.GetUserId();
            var unitId = db.UnitManagers
                .Where(a => a.Id == userId)
                .Select(a => a.UnitId)
                .FirstOrDefault();

            var list = db.Products
                            .Include(p=> p.SubCategory)
                            .Where(p => p.UnitId == unitId)
                            .Select(e => new {
                                id = e.ProductId,
                                name = e.ProductName,
                                ProductNameBangla = e.ProductNameBangla,
                                Rate = e.Rate,
                                Discount = e.Discount,
                                group = e.SubCategory.SubCategoryId,
                                groupName = e.SubCategory.SubCategoryName,
                                MultiplyWith=e.MultiplyWith,
                                Active = e.Active
                            });
            return Ok(list);
        }

        // GET: api/Product/GetDropDownList/
        [Route("api/Product/ProductDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProductDropDownList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var zoneManagerId = db.ZoneManagers.Where(a => a.Id == userId).Select(a => a.ZoneManagerId).FirstOrDefault();
            var unitId = db.ShowRooms.Where(a => a.ShowRoomId == showRoomId).Select(a => a.UnitId).FirstOrDefault();

            var list = db.Products
                    .Where(p => p.UnitId == unitId)
                    .Select(e => new { ProductId = e.ProductId,
                        ProductName = e.ProductName,
                        ProductNameBangla = e.ProductNameBangla,
                        Rate = e.Rate,
                        Discount = e.Discount
                    });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        [Route("api/Product/GetProductRateById/{ProductName}")]
        [HttpGet]
        [ResponseType(typeof(ProductView))]
        public IHttpActionResult GetProductRateById(string ProductName)
        {
            var productRate = db.Products
                .Include(p => p.SubCategory )
                .Include(p => p.SubCategory.MainCategory)
                .Where(e => e.ProductName == ProductName)
                .Select(e => new {
                    ProductId = e.ProductId,
                    ProductName = e.ProductName,
                    Rate = e.Rate,
                    MultiplyWith = e.MultiplyWith,
                    Discount = e.Discount,
                    ProductNameBangla = e.ProductNameBangla,
                    SubCategoryId = e.SubCategoryId,
                    MainCategoryName = e.SubCategory.MainCategory.MainCategoryName
                }).FirstOrDefault();
            if (productRate == null)
            {
                return NotFound();
            }
            return Ok(productRate);

            //ProductView product = new ProductView();
            //string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            //string queryString = @"SELECT        
            //                        dbo.Products.SubCategoryId, dbo.Products.ProductId, dbo.Products.ProductName, dbo.Products.ProductNameBangla, dbo.Products.Rate, dbo.Products.Discount, dbo.Products.CreatedBy, 
            //                        dbo.SubCategories.SubCategoryName, dbo.SubCategories.MainCategoryId, dbo.MainCategories.MainCategoryName
            //                        FROM            
            //                        dbo.Products INNER JOIN
            //                        dbo.SubCategories ON dbo.Products.SubCategoryId = dbo.SubCategories.SubCategoryId INNER JOIN
            //                        dbo.MainCategories ON dbo.SubCategories.MainCategoryId = dbo.MainCategories.MainCategoryId WHERE ProductName='" + ProductName +"'";

            //using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    SqlCommand command = new SqlCommand(queryString, connection);
            //    connection.Open();

            //    SqlDataReader reader = command.ExecuteReader();
            //    try
            //    {
            //        while (reader.Read())
            //        {
            //            int id = (int)reader["ProductId"];
            //            int subCategoryId = (int)reader["SubCategoryId"];
            //            int mainCategoryId = (int)reader["MainCategoryId"];
            //            string productName = (string) reader["ProductName"];
            //            string productNameBangla = (string)reader["ProductNameBangla"];
            //            double rate = (double)reader["Rate"];
            //            double discount = (double)reader["Discount"];
            //            string mainCategoryName = (string)reader["MainCategoryName"]; 
            //            product = new ProductView();
            //            product.ProductId = id;
            //            product.ProductName = productName;
            //            product.ProductNameBangla = productNameBangla;
            //            product.Rate = rate;
            //            product.Discount = discount;
            //            product.SubCategoryId = subCategoryId;
            //            product.MainCategoryName = mainCategoryName;
            //        }
            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}
            //return Ok(product);
        }



        [Route("api/Product/UpdateImageNameToProducts/{ProductId}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateImageNameToProducts(int ProductId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var aCustomer = db.Products
                .Where(x => x.ProductId == ProductId)
                .FirstOrDefault();

            if (aCustomer != null)
            {
                aCustomer.Image = ProductId.ToString() + ".jpg";
                db.Products.AddOrUpdate(aCustomer);
                db.SaveChanges();
            }
            return Ok();
        }






        // GET: api/Product
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: api/Product/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Product/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(int id, Product product)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
            if (showRoomId != 0) {
                product.ShowRoomId = showRoomId;
            } else {
                product.ShowRoomId = null;
            }
            if (product.Image == ""){product.Image = null;}
            product.UnitId = unitId;
            product.CreatedBy = userName;
            product.DateCreated = createdAt;
            product.DateUpdated = createdAt;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Product
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(Product product)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
            if (product.ShowRoomId == 0)
            {
                product.ShowRoomId = null;
            }
            if (product.Image == "") { product.Image = null; }
            product.UnitId = unitId;
            product.CreatedBy = userName;
            product.DateCreated = createdAt;
            product.DateUpdated = createdAt;
            if (db.Products.Any(m => m.ProductName == product.ProductName && m.ShowRoomId == showRoomId)) {
                ModelState.AddModelError("ProductName", "Product Name Already Exists!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
        }

        // DELETE: api/Product/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}