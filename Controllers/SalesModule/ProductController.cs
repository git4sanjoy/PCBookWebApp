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




namespace PCBookWebApp.Controllers
{
    public class ProductController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Product/GetProductsList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomView))]
        public IHttpActionResult GetProductsList()
        {
            List<ShowRoomView> ImportProductList = new List<ShowRoomView>();
            ShowRoomView importProduct = new ShowRoomView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.Products.ProductId AS id, dbo.Products.ProductName AS name, dbo.Products.SubCategoryId AS [group], dbo.SubCategories.SubCategoryName AS groupName, dbo.Products.MultiplyWith, dbo.Products.Rate, 
                                    dbo.Products.Discount
                                    FROM            
                                    dbo.Products 
                                    INNER JOIN
                                    dbo.SubCategories ON dbo.Products.SubCategoryId = dbo.SubCategories.SubCategoryId";

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
                        double multiplyWith = (double)reader["MultiplyWith"];
                        double rate = (double)reader["Rate"];
                        double discount = (double)reader["Discount"];

                        importProduct = new ShowRoomView();
                        importProduct.id = id;
                        importProduct.name = name;
                        importProduct.group = group;
                        importProduct.groupName = groupName;
                        importProduct.MultiplyWith = multiplyWith;
                        importProduct.Rate = rate;
                        importProduct.Discount = discount;

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
        // GET: api/Product/GetDropDownList/
        [Route("api/Product/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetDropDownList()
        {
            var unitList = db.Products.Select(e => new { ProductId = e.ProductId, ProductName = e.ProductName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }


        [Route("api/Product/GetProductRateById/{ProductName}")]
        [HttpGet]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProductRateById(string ProductName)
        {
            //var productRate = db.Products
            //    .Where(e=> e.ProductName == ProductName)
            //    .Select(e => new { ProductId = e.ProductId, ProductName = e.ProductName, Rate = e.Rate, Discount = e.Discount });
            //if (productRate == null)
            //{
            //    return NotFound();
            //}
            //return Ok(productRate);

            Product product = new Product();
            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = "SELECT SubCategoryId, ProductId,ProductName, Rate, Discount, CreatedBy FROM dbo.Products WHERE ProductName='"+ ProductName +"'";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["ProductId"];
                        string productName = (string)reader["ProductName"];
                        double rate = (double)reader["Rate"];
                        double discount = (double)reader["Discount"];

                        product = new Product();
                        product.ProductId = id;
                        product.ProductName = productName;
                        product.Rate = rate;
                        product.Discount = discount;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(product);
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