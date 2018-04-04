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
    [Authorize]
    public class PurchasesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/Purchases/GetPurchasesList")]
        [HttpGet]
        public IHttpActionResult GetPurchasesList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var list = (from item in db.Purchases
                        where item.ShowRoomId == showRoomId && item.ProcesseLocation ==null
                        select new
                        {
                            item.PurchaseId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.Quantity,
                            item.SE,
                            item.OrderNo,
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
                                where item.ShowRoomId == showRoomId
                                select new
                                {
                                    item.SupplierId,
                                    item.SupplierName
                                }).ToList();
            var purchasedProductList = (from item in db.PurchasedProducts
                                        where item.ShowRoomId == showRoomId && item.ProductTypeId==1
                                        select new
                                        {
                                            item.ProductTypeId,
                                            item.PurchasedProductId,
                                            item.PurchasedProductName
                                        }).ToList();
            //var showRoomList = (from item in db.ShowRooms
            //                    select new
            //                    {
            //                        item.ShowRoomId,
            //                        item.ShowRoomName
            //                    }).ToList();
            var processlocationList = (from item in db.ProcesseLocations
                                       where item.ShowRoomId == showRoomId
                                select new
                                {
                                    item.ProcesseLocationId,
                                    item.ProcesseLocationName
                                }).ToList();
            var processList = (from item in db.ProcessLists
                               where item.ShowRoomId == showRoomId && item.ProcessListName != "Short/Excess" && item.ProcessListName != "Finished"
                               select new
                                {
                                    item.ProcessListId,
                                    item.ProcessListName
                                }).ToList();
            var orderNumber = (from item in db.FinishedGoodStocks
                                      where item.ShowRoomId == showRoomId
                                       select new
                                       {                                           
                                           item.OrderNumber
                                       })
                                       //.Distinct()
                                       .ToList();

            if (list == null)
            {
                return NotFound();
            }

            return Ok(new { list, supplierList, purchasedProductList, processlocationList, processList, orderNumber });
        }

        [Route("api/Purchases/GetPurchasesListShow")]
        [HttpGet]
        public IHttpActionResult GetPurchasesListShow()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var list = (from item in db.Purchases
                        where item.ShowRoomId == showRoomId && item.ProcesseLocation == null
                        select new
                        {
                            item.PurchaseId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.Quantity,
                            item.SE,
                            item.OrderNo,
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
                        }).ToList().OrderByDescending(x => x.DateCreated);


            if (list == null)
            {
                return NotFound();
            }

            return Ok(new { list });
        }

        [Route("api/Purchases/GetSearch/{fromdate}/{todate}/{supplierId}/{productId}")]
        [HttpGet]
        //[ResponseType(typeof(Purchase))]
        public IHttpActionResult GetSearch(DateTime? fromdate, DateTime? todate, int? supplierId, int? productId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();

            object list = new List<object>();

            if (fromdate != null && todate != null && supplierId == null && productId == null)
            {
                list = (from item in db.Purchases
                        join prod in db.PurchasedProducts on item.PurchasedProductId equals prod.PurchasedProductId
                        join s in db.Suppliers on item.SupplierId equals s.SupplierId
                        where item.PurchaseDate >= fromdate && item.PurchaseDate <= todate && item.ShowRoomId==showRoomId
                        select new
                        {
                            item.PurchaseId,
                            item.ProcesseLocationId,
                            item.ProcessListId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.Amount,
                            item.Quantity,
                            item.SE,
                            item.OrderNo,
                            item.Discount,
                            item.DeliveryQuantity,
                            prod.PurchasedProductName,
                            prod.PurchasedProductId,
                            s.SupplierId,
                            s.SupplierName
                        }).ToList();
            }
            else if (fromdate != null && todate != null && supplierId != null && productId == null)
            {
                list = (from item in db.Purchases
                        join prod in db.PurchasedProducts on item.PurchasedProductId equals prod.PurchasedProductId
                        join s in db.Suppliers on item.SupplierId equals s.SupplierId
                        where (item.PurchaseDate >= fromdate && item.PurchaseDate <= todate)
                        && item.SupplierId == supplierId && item.ShowRoomId == showRoomId
                        select new
                        {
                            item.PurchaseId,
                            item.ProcesseLocationId,
                            item.ProcessListId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.Amount,
                            item.Quantity,
                            item.SE,
                            item.OrderNo,
                            item.Discount,
                            item.DeliveryQuantity,
                            prod.PurchasedProductName,
                            prod.PurchasedProductId,
                            s.SupplierId,
                            s.SupplierName
                        }).ToList();
            }
            else if (fromdate != null && todate != null && supplierId == null && productId != null)
            {
                list = (from item in db.Purchases
                        join prod in db.PurchasedProducts on item.PurchasedProductId equals prod.PurchasedProductId
                        join s in db.Suppliers on item.SupplierId equals s.SupplierId
                        where (item.PurchaseDate >= fromdate && item.PurchaseDate <= todate)
                        && item.PurchasedProductId == productId && item.ShowRoomId == showRoomId
                        select new
                        {
                            item.PurchaseId,
                            item.ProcesseLocationId,
                            item.ProcessListId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.Amount,
                            item.Quantity,
                            item.SE,
                            item.OrderNo,
                            item.Discount,
                            item.DeliveryQuantity,
                            prod.PurchasedProductName,
                            prod.PurchasedProductId,
                            s.SupplierId,
                            s.SupplierName
                        }).ToList();
            }
            else if (fromdate != null && todate != null && supplierId != null && productId != null)
            {
                list = (from item in db.Purchases
                        join prod in db.PurchasedProducts on item.PurchasedProductId equals prod.PurchasedProductId
                        join s in db.Suppliers on item.SupplierId equals s.SupplierId
                        where (item.PurchaseDate >= fromdate && item.PurchaseDate <= todate)
                        && item.SupplierId == supplierId
                        && item.PurchasedProductId == productId && item.ShowRoomId == showRoomId
                        select new
                        {
                            item.PurchaseId,
                            item.ProcesseLocationId,
                            item.ProcessListId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.Amount,
                            item.Quantity,
                            item.SE,
                            item.OrderNo,
                            item.Discount,
                            item.DeliveryQuantity,
                            prod.PurchasedProductName,
                            prod.PurchasedProductId,
                            s.SupplierId,
                            s.SupplierName
                        }).ToList();
            }
            return Ok(new { list });
        }
        // GET: api/Purchases
        public IQueryable<Purchase> GetPurchases()
        {
            return db.Purchases;
        }
        // GET: api/Purchases/5
        [ResponseType(typeof(Purchase))]
        public async Task<IHttpActionResult> GetPurchase(int id)

        //public IHttpActionResult GetPurchase(int id)
        {
            Purchase purchase = await db.Purchases.FindAsync(id);
           
            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }

        // PUT: api/Purchases/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPurchase(int id, Purchase purchase)
        //public IHttpActionResult PutPurchase(int id, Purchase purchase)

        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            purchase.ShowRoomId = showRoomId;
            purchase.CreatedBy = userName;
            purchase.DateCreated = DateTime.Now;
            purchase.DateUpdated = DateTime.Now;

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
                await db.SaveChangesAsync();
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
        public async Task<IHttpActionResult> PostPurchase(Purchase purchase)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            var FinishedGoodStockId = db.FinishedGoodStocks.Where(fs => fs.OrderNumber == purchase.OrderNo).Select(fs => fs.FinishedGoodStockId).FirstOrDefault();
            var itemRateObj = db.PurchasedProductRates
                .Where(r => r.PurchasedProductId==purchase.PurchasedProductId && r.FinishedGoodStockId == FinishedGoodStockId)
                .ToArray();

            double stockQuantity = 0;
            double avgRate = 0;


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (purchase.Quantity > 0 && purchase.DeliveryQuantity == 0)
                {
                    if (itemRateObj.Length > 0)
                    {
                        stockQuantity = itemRateObj[0].Quantity;
                        avgRate = itemRateObj[0].AvgRate;
                        int purchasedProductId = itemRateObj[0].PurchasedProductId;
                        int finishedGoodStockId = itemRateObj[0].FinishedGoodStockId;
                        double balanceQuantityAmount = stockQuantity * avgRate;

                        double totalTk = balanceQuantityAmount + ((double)purchase.Amount - (double)purchase.Discount);
                        double totalQuantity = stockQuantity + ((double)purchase.Quantity - (double)purchase.SE);
                        double newAvgRate = Math.Round(totalTk / totalQuantity, 2);

                        var aRateObj = db.PurchasedProductRates
                            .Where(x => x.PurchasedProductId == purchasedProductId && x.FinishedGoodStockId == finishedGoodStockId)
                            .FirstOrDefault();

                        aRateObj.Quantity = Math.Round(stockQuantity + purchase.Quantity, 2);
                        aRateObj.AvgRate = newAvgRate;
                        db.PurchasedProductRates.AddOrUpdate(aRateObj);
                        db.SaveChanges();
                    }
                    else
                    {
                        PurchasedProductRate aRateObj = new PurchasedProductRate();
                        aRateObj.PurchasedProductId = purchase.PurchasedProductId;
                        aRateObj.FinishedGoodStockId = FinishedGoodStockId;
                        aRateObj.Quantity = purchase.Quantity;
                        aRateObj.AvgRate = Math.Round( ((double)purchase.Amount - (double)purchase.Discount) / (double)purchase.Quantity, 2);
                        aRateObj.ShowRoomId = showRoomId;
                        db.PurchasedProductRates.Add(aRateObj);
                        await db.SaveChangesAsync();
                    }
                }
                else {
                    stockQuantity = itemRateObj[0].Quantity;
                    //avgRate = itemRateObj[0].AvgRate;
                    int purchasedProductId = itemRateObj[0].PurchasedProductId;
                    int finishedGoodStockId = itemRateObj[0].FinishedGoodStockId;

                    var aRateObj = db.PurchasedProductRates
                        .Where(x => x.PurchasedProductId == purchasedProductId && x.FinishedGoodStockId == finishedGoodStockId)
                        .FirstOrDefault();

                    aRateObj.Quantity = Math.Round(stockQuantity - purchase.DeliveryQuantity, 2);
                    //aRateObj.AvgRate = avgRate;
                    db.PurchasedProductRates.AddOrUpdate(aRateObj);
                    db.SaveChanges();
                }


                purchase.ShowRoomId = showRoomId;
                purchase.CreatedBy = userName;
                purchase.DateCreated = DateTime.Now;
                purchase.DateUpdated = purchase.DateCreated;
                purchase.Active = true;                
                db.Purchases.Add(purchase);
                await db.SaveChangesAsync();


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

            var aPurchasesObj = db.Purchases
                .Where(r => r.PurchaseId == id)
                .ToArray();
            double purchaseAmount = aPurchasesObj[0].Amount;
            double purchaseQuantity = aPurchasesObj[0].Quantity;
            string orderNo = aPurchasesObj[0].OrderNo;
            int purchasedProductId = aPurchasesObj[0].PurchasedProductId;

            var FinishedGoodStockId = db.FinishedGoodStocks.Where(fs => fs.OrderNumber == orderNo).Select(fs => fs.FinishedGoodStockId).FirstOrDefault();
            var aRateObj = db.PurchasedProductRates
                        .Select(x => new {
                            x.PurchasedProductId,
                            x.FinishedGoodStockId,
                            x.Quantity,
                            x.AvgRate,
                        })
                        .Where(x => x.PurchasedProductId == purchasedProductId && x.FinishedGoodStockId == FinishedGoodStockId)
                        .FirstOrDefault();
            double presentTotalQuantity = aRateObj.Quantity;
            double presentTotalPrice = aRateObj.Quantity * aRateObj.AvgRate;
            double nextQuantity = presentTotalQuantity - purchaseQuantity;
            double nextAvegRate = Math.Round((presentTotalPrice - purchaseAmount) / (presentTotalQuantity- purchaseQuantity), 2);


            var aUpdateRateObj = db.PurchasedProductRates
                        .Where(x => x.PurchasedProductId == purchasedProductId && x.FinishedGoodStockId == FinishedGoodStockId)
                        .FirstOrDefault();

            aUpdateRateObj.Quantity = Math.Round(nextQuantity, 0);
            aUpdateRateObj.AvgRate = Math.Round(nextAvegRate, 0);
            db.PurchasedProductRates.AddOrUpdate(aUpdateRateObj);
            db.SaveChanges();

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