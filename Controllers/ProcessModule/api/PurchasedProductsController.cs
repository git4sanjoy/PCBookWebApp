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
using PCBookWebApp.Models.ProcessModule;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using PCBookWebApp.Models.ProcessModule.ViewModels;
using System.Configuration;
using System.Data.SqlClient;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    [Authorize]
    public class PurchasedProductsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/PurchasedProducts
        public IQueryable<PurchasedProduct> GetPurchasedProducts()
        {
            return db.PurchasedProducts;
        }

        [Route("api/PurchasedProducts/GetProductTypeList")]
        [HttpGet]
        public IHttpActionResult GetProductTypeList()
        {
            var list = (from item in db.ProductTypes
                        select new
                        {
                            item.ProductTypeId,
                            item.ProductTypeName
                        }).ToList();
            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }

        //[Route("api/PurchasedProducts/GetPurchasedProductList")]
        //[HttpGet]
        //public IHttpActionResult GetPurchasedProductList()
        //{
        //    var list = (from item in db.PurchasedProducts
        //                select new
        //                {
        //                    item.PurchasedProductId,
        //                    item.PurchasedProductName,
        //                    item.ProductTypeId,
        //                    item.ProductType.ProductTypeName,
        //                    item.Active,
        //                    item.CreatedBy,
        //                    item.DateCreated,
        //                    item.DateUpdated
        //                }).ToList();
        //    if (list == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(list);
        //}

        [Route("api/PurchasedProducts/GetPurchasedProductList")]
        [HttpGet]
        [ResponseType(typeof(vMPurchasedProduct))]
        public IHttpActionResult GetPurchasedProductList()
        {
            List<vMPurchasedProduct> List = new List<vMPurchasedProduct>();
            vMPurchasedProduct Obj = new vMPurchasedProduct();

            var PurchasedProductList = (from item in db.PurchasedProducts
                                        select new
                                        {
                                            item.PurchasedProductId,
                                            item.PurchasedProductName,
                                            item.ProductTypeId,
                                            item.ProductType.ProductTypeName,
                                            item.Active,
                                            item.CreatedBy,
                                            item.DateCreated,
                                            item.DateUpdated
                                        }).ToList();
            foreach (var item in PurchasedProductList)
            {
                Obj = new vMPurchasedProduct();
                {
                    Obj.PurchasedProductId = item.PurchasedProductId;
                    Obj.PurchasedProductName = item.PurchasedProductName;
                    Obj.ProductTypeId = item.ProductTypeId.ToString();
                    Obj.ProductTypeName = item.ProductTypeName;                   
                };
                List.Add(Obj);
            }
            if (List == null)
            {
                return NotFound();
            }
            //string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            //string queryString = @"select p.PurchasedProductId,p.PurchasedProductName,p.ProductTypeId,pt.ProductTypeName from PurchasedProducts p
            //                             left join ProductTypes pt on pt.ProductTypeId = p.ProductTypeId";

            //using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    SqlCommand command = new SqlCommand(queryString, connection);
            //    connection.Open();
            //    SqlDataReader reader = command.ExecuteReader();
            //    try
            //    {
            //        while (reader.Read())
            //        {
            //            Obj = new vMPurchasedProduct();
            //            Obj.PurchasedProductId = (int)reader["PurchasedProductId"];
            //            Obj.PurchasedProductName = (string)reader["PurchasedProductName"];
            //            Obj.ProductTypeId = reader["ProductTypeId"].ToString();
            //            Obj.ProductTypeName = (string)reader["ProductTypeName"];
            //            List.Add(Obj);
            //        }
            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}

            return Ok(List);
        }

        // GET: api/PurchasedProducts/5
        [ResponseType(typeof(PurchasedProduct))]
        public async Task<IHttpActionResult> GetPurchasedProduct(int id)
        {
            PurchasedProduct purchasedProduct = await db.PurchasedProducts.FindAsync(id);
            if (purchasedProduct == null)
            {
                return NotFound();
            }

            return Ok(purchasedProduct);
        }

        // PUT: api/PurchasedProducts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPurchasedProduct(int id, PurchasedProduct purchasedProduct)
        {
            var msg = 0;
            var check = db.PurchasedProducts.FirstOrDefault(m => m.PurchasedProductName == purchasedProduct.PurchasedProductName);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchasedProduct.PurchasedProductId)
            {
                return BadRequest();
            }
            //db.Entry(purchasedProduct).State = EntityState.Modified;
            if (check == null)
            {
                try
                {
                    var createdDate = db.PurchasedProducts.Where(x => x.PurchasedProductId == id).Select(x => x.DateCreated).FirstOrDefault();
                    purchasedProduct.DateCreated = createdDate;
                    purchasedProduct.DateUpdated = DateTime.Now;
                    purchasedProduct.Active = true;
                    db.PurchasedProducts.AddOrUpdate(purchasedProduct);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchasedProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return Ok(msg);
            //return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/PurchasedProducts
        [ResponseType(typeof(PurchasedProduct))]
        public async Task<IHttpActionResult> PostPurchasedProduct(PurchasedProduct purchasedProduct)
        {
            var msg = 0;
            var check = db.PurchasedProducts.FirstOrDefault(m => m.PurchasedProductName == purchasedProduct.PurchasedProductName);
            string userName = User.Identity.GetUserName();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (check == null)
            {
                try
                {
                    purchasedProduct.CreatedBy = userName;
                    purchasedProduct.DateCreated = DateTime.Now;
                    purchasedProduct.DateUpdated = purchasedProduct.DateCreated;
                    purchasedProduct.Active = true;
                    db.PurchasedProducts.Add(purchasedProduct);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return Ok(msg);
            //return CreatedAtRoute("DefaultApi", new { id = purchasedProduct.PurchasedProductId }, purchasedProduct);
        }

        // DELETE: api/PurchasedProducts/5
        [ResponseType(typeof(PurchasedProduct))]
        public async Task<IHttpActionResult> DeletePurchasedProduct(int id)
        {
            PurchasedProduct purchasedProduct = await db.PurchasedProducts.FindAsync(id);
            if (purchasedProduct == null)
            {
                return NotFound();
            }

            db.PurchasedProducts.Remove(purchasedProduct);
            await db.SaveChangesAsync();

            return Ok(purchasedProduct);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PurchasedProductExists(int id)
        {
            return db.PurchasedProducts.Count(e => e.PurchasedProductId == id) > 0;
        }
    }
}