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



        [Route("api/PurchasedProducts/PurchasedProductsMultiSelectList")]
        [HttpGet]
        public IHttpActionResult GetPurchasedProductsMultiSelectList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.PurchasedProducts
                            .Where(d => d.ShowRoomId == showRoomId)
                            .OrderBy(d => d.PurchasedProductName)
                            .Select(e => new {
                                id = e.PurchasedProductId,
                                label = e.PurchasedProductName
                            });

            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        [Route("api/PurchasedProducts/PurchasedProductsList")]
        [HttpGet]
        public IHttpActionResult GetPurchasedProductsList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.PurchasedProducts
                            .Include(a => a.Matric)
                            .Include(a => a.ProductType)
                            .Where(d => d.ShowRoomId == showRoomId)
                            .OrderBy(d => d.PurchasedProductName)
                            .Select(e => new {
                                id = e.PurchasedProductId,
                                name = e.PurchasedProductName,
                                group = e.ProductTypeId,
                                groupName = e.ProductType.ProductTypeName,
                                status = e.Matric.MatricId,
                                createDate = e.DateCreated
                            });

            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/PurchasedProducts/MatricsListXEdit")]
        [HttpGet]
        [ResponseType(typeof(Matric))]
        public IHttpActionResult GetMatricsListXEdit()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var list = db.Matrics
                            .Where(m => m.ShowRoomId== showRoomId)
                            .Select(e => new { value = e.MatricId, text = e.MatricName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/PurchasedProducts/ProductTypeListXEdit")]
        [HttpGet]
        [ResponseType(typeof(ProductType))]
        public IHttpActionResult GetProductTypeListXEdit()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            string userName = User.Identity.GetUserName();
            string currentUserId = User.Identity.GetUserId();

            var list = db.ProductTypes
                            .Where(a => a.ShowRoomId == showRoomId)
                            .Select(e => new { id = e.ProductTypeId, text = e.ProductTypeName })
                            .OrderBy(e => e.text);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // End XEdit Method

        [Route("api/PurchasedProducts/ProductBalanceById/{ProductId}")]
        [HttpGet]
        public IHttpActionResult GetProductBalance(int ProductId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var list = (from item in db.Purchases
                        where item.PurchasedProductId == ProductId
                        select new
                        {
                            item.PurchasedProductId,
                            item.Quantity,
                            item.DeliveryQuantity,
                            item.SE,
                            item.Amount,
                            item.Discount
                        }).ToList().GroupBy(x => new {  x.PurchasedProductId })
                        .Select(
                                g => new
                                {
                                    Key = g.Key,
                                    DeliveryQuantity = g.Sum(s => s.DeliveryQuantity),
                                    ReceiveQuantity = g.Sum(s => s.Quantity),
                                    SE = g.Sum(s => s.SE)

                                }).Where(q => q.ReceiveQuantity != (q.DeliveryQuantity + q.SE));

            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }


        // GET: api/PurchasedProducts
        public IQueryable<PurchasedProduct> GetPurchasedProducts()
        {
            return db.PurchasedProducts;
        }

        [Route("api/PurchasedProducts/GetProductTypeList")]
        [HttpGet]
        public IHttpActionResult GetProductTypeList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = (from item in db.ProductTypes
                        where item.ShowRoomId == showRoomId
                        select new
                        {
                            item.ProductTypeId,
                            item.ProductTypeName
                        }).ToList();
            var MatricsList = (from item in db.Matrics
                        where item.ShowRoomId == showRoomId
                        select new
                        {
                            item.MatricId,
                            item.MatricName
                        }).ToList();

            if (list == null)
            {
                return NotFound();
            }

            return Ok(new {list, MatricsList });
        }

        [Route("api/PurchasedProducts/GetPurchasedProductList")]
        [HttpGet]
        [ResponseType(typeof(vMPurchasedProduct))]
        public IHttpActionResult GetPurchasedProductList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            List<vMPurchasedProduct> List = new List<vMPurchasedProduct>();
            vMPurchasedProduct Obj = new vMPurchasedProduct();

            var PurchasedProductList = (from item in db.PurchasedProducts
                                        where item.ShowRoomId == showRoomId
                                        select new
                                        {
                                            item.PurchasedProductId,
                                            item.PurchasedProductName,
                                            item.ProductTypeId,
                                            item.ProductType.ProductTypeName,
                                            item.ShowRoomId,
                                            item.MatricId,
                                            item.Matric.MatricName,
                                            item.ShowRoom.ShowRoomName,
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
                    Obj.MatricId = item.MatricId;
                    Obj.MatricName = item.MatricName;
                };
                List.Add(Obj);
            }
            if (List == null)
            {
                return NotFound();
            }

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
                    var obj = db.PurchasedProducts.Where(x => x.PurchasedProductId == purchasedProduct.PurchasedProductId).FirstOrDefault();
                    purchasedProduct.ShowRoomId = obj.ShowRoomId;
                    purchasedProduct.DateCreated = obj.DateCreated;
                    purchasedProduct.DateUpdated = DateTime.Now;
                    purchasedProduct.CreatedBy = obj.CreatedBy;
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
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (check == null)
            {
                try
                {
                    purchasedProduct.ShowRoomId = showRoomId;
                    purchasedProduct.CreatedBy = userName;
                    purchasedProduct.DateCreated = DateTime.Now;                    
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