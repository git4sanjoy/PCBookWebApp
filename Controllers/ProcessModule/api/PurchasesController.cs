using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PCBookWebApp.DAL;
using PCBookWebApp.Models.ProcessModule;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    public class PurchasesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/Purchases
        public IQueryable<Purchase> GetPurchases()
        {
            return db.Purchases;
        }
        [Route("api/Purchases/GetPurchasesList")]
        [HttpGet]
        public IHttpActionResult GetPurchasesList()
        {
            var list = (from item in db.Purchases
                        select new
                        {
                            item.PurchaseId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.Quantity,
                            item.SE,
                            item.Amount,
                            item.Discount,
                            item.Active,
                            item.CreatedBy,
                            item.DateCreated,
                            item.DateUpdated,
                            item.ShowRoomId,
                            item.ShowRoom.ShowRoomName,
                            item.PurchasedProductId,
                            item.PurchasedProduct.PurchasedProductName,
                            item.SupplierId,
                            item.Supplier.SupplierName
                            //item.Active

                        }).ToList();
            var supplierList = (from item in db.Suppliers
                                select new
                                {
                                    item.SupplierId,
                                    item.SupplierName
                                }).ToList();
            var purchasedProductList = (from item in db.PurchasedProducts
                                        select new
                                        {
                                            item.PurchasedProductId,
                                            item.PurchasedProductName
                                        }).ToList();
            var showRoomList = (from item in db.ShowRooms
                                select new
                                {
                                    item.ShowRoomId,
                                    item.ShowRoomName
                                }).ToList();
            var processlocationList = (from item in db.ProcesseLocations
                                select new
                                {
                                    item.ProcesseLocationId,
                                    item.ProcesseLocationName
                                }).ToList();
            var processList = (from item in db.ProcessLists
                                select new
                                {
                                    item.ProcessListId,
                                    item.ProcessListName
                                }).ToList();

            if (list == null)
            {
                return NotFound();
            }

            return Ok(new { list, supplierList, purchasedProductList, showRoomList, processlocationList, processList });
        }
        // GET: api/Purchases/5
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult GetPurchase(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }

        // PUT: api/Purchases/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPurchase(int id, Purchase purchase)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchase.PurchaseId)
            {
                return BadRequest();
            }

            db.Entry(purchase).State = EntityState.Modified;

            try
            {
                var createdDate = db.Purchases.Where(x => x.PurchaseId == id).Select(x => x.DateCreated).FirstOrDefault();
                purchase.DateCreated = createdDate;
                purchase.DateUpdated = DateTime.Now;
                purchase.Active = true;
                db.Purchases.AddOrUpdate(purchase);
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
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

        // POST: api/Purchases
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult PostPurchase(Purchase purchase)
        {
            string userName = User.Identity.GetUserName();
            //purchase.PurchaseDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                purchase.CreatedBy = userName;
                purchase.DateCreated = DateTime.Now;
                purchase.DateUpdated = purchase.DateCreated;
                purchase.Active = true;
                db.Purchases.Add(purchase);
                db.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }
           

            return CreatedAtRoute("DefaultApi", new { id = purchase.PurchaseId }, purchase);
        }

        // DELETE: api/Purchases/5
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult DeletePurchase(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return NotFound();
            }

            db.Purchases.Remove(purchase);
            db.SaveChanges();

            return Ok(purchase);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PurchaseExists(int id)
        {
            return db.Purchases.Count(e => e.PurchaseId == id) > 0;
        }
    }
}