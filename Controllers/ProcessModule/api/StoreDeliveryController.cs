using Microsoft.AspNet.Identity;
using PCBookWebApp.DAL;
using PCBookWebApp.Models.ProcessModule;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    [Authorize]
    public class StoreDeliveryController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();
        Process process;
        [Route("api/StoreDelivery/GetDetailhData")]
        [HttpGet]
        //[ResponseType(typeof(Purchase))]
        public IHttpActionResult GetDetailhData()
        {
            var list = (from item in db.Purchases
                        join prod in db.PurchasedProducts on item.PurchasedProductId equals prod.PurchasedProductId
                        select new
                        {
                            item.PurchaseId,
                            item.ProcesseLocationId,
                            item.ProcesseLocation.ProcesseLocationName,
                            item.ProcessListId,
                            item.ProcessList.ProcessListName,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.DeliveryQuantity,
                            prod.PurchasedProductName,
                            prod.PurchasedProductId
                        }).ToList()
                         .Where(p => p.PChallanNo.Contains("2018"))
                            .GroupBy(d => d.PChallanNo)
                            .Select(
                                g => new
                                {
                                    Key = g.Key,
                                    DeliveryQuantity = g.Sum(s => s.DeliveryQuantity),
                                    PurchaseId = g.First().PurchaseId,
                                    ProcessListName = g.First().ProcessListName,
                                    ProcesseLocationId = g.First().ProcesseLocationId,
                                    ProcesseLocationName = g.First().ProcesseLocationName,
                                    ProcessListId = g.First().ProcessListId,
                                    PurchaseDate = g.First().PurchaseDate,
                                    PChallanNo = g.First().PChallanNo,
                                    PurchasedProductName = g.First().PurchasedProductName,
                                    PurchasedProductId = g.First().PurchasedProductId
                                });
            return Ok(list);
        }
        [Route("api/StoreDelivery/GetSearchData/{chalanNo}")]
        [HttpGet]
        //[ResponseType(typeof(Purchase))]
        public IHttpActionResult GetSearchData(string chalanNo)
        {
            var list = (from item in db.Purchases
                        join prod in db.PurchasedProducts on item.PurchasedProductId equals prod.PurchasedProductId
                        where item.PChallanNo == chalanNo
                        select new
                        {
                            item.PurchaseId,
                            item.ProcesseLocationId,
                            item.ProcessListId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.DeliveryQuantity,
                            prod.PurchasedProductName,
                            prod.PurchasedProductId
                        }).ToList();
            return Ok(new { list });
        }

        // GET: api/Purchases
        public IQueryable<Purchase> GetPurchases()
        {
            return db.Purchases;
        }
        [Route("api/Purchases/GetPurchases")]
        [HttpGet]
        public IHttpActionResult GetPurchaseses()
        {
            var list = (from item in db.Purchases
                        select new
                        {
                            item.PurchaseId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.DeliveryQuantity,
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
        [HttpPost]
        [Route("api/StoreDelivery/PurchasesListForGrid")]
        public IQueryable<Purchase> PurchasesListForGrid(string chalanNo)
        {
            return db.Purchases;
            //string userId = User.Identity.GetUserId();
            //var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            //var list = (from item in db.Purchases
            //            where item.ShowRoomId == showRoomId
            //            select new
            //            {
            //                item.PurchaseId,
            //                item.PurchaseDate,
            //                item.PChallanNo,
            //                item.DeliveryQuantity,
            //                item.SE,
            //                item.Amount,
            //                item.Discount,
            //                item.Active,
            //                item.CreatedBy,
            //                item.DateCreated,
            //                item.DateUpdated,
            //                item.ShowRoomId,
            //                item.ShowRoom.ShowRoomName,
            //                item.PurchasedProductId,
            //                item.PurchasedProduct.PurchasedProductName,
            //                item.SupplierId,
            //                item.Supplier.SupplierName
            //                //item.Active

            //            }).ToList();



            //if (list == null)
            //{
            //    return NotFound();
            //}

            //return Ok(new { list });
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
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (id != purchase.PurchaseId)
            //{
            //    return BadRequest();
            //}

            // db.Entry(purchase).State = EntityState.Modified;

            var chalanNoObj = purchase.PChallanNo;

            try
            {              

                var obj = db.Purchases.FirstOrDefault(m => m.PurchaseId == purchase.PurchaseId);

                purchase.DateCreated = obj.DateCreated;
                purchase.DateUpdated = DateTime.Now;
                purchase.CreatedBy = obj.CreatedBy;
                purchase.ShowRoomId = 1;
                purchase.Active = true;
                purchase.SupplierId = 2;
                purchase.Amount = 0;                
                db.Purchases.AddOrUpdate(purchase);
                db.SaveChanges();

                //var process = (from item in db.Processes
                //           where item.ProcesseLocationId == purchase.ProcesseLocationId &&
                //           item.ProcessListId == purchase.ProcessListId &&
                //           item.LotNo == purchase.PChallanNo &&
                //           item.PurchasedProductId == purchase.PurchasedProductId &&
                //           item.ReceiveQuantity == obj.DeliveryQuantity
                //           select item).FirstOrDefault();
              
                //process.ReceiveQuantity = purchase.DeliveryQuantity;               
                //db.Processes.AddOrUpdate(process);
                //db.SaveChanges();
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

            return Ok(chalanNoObj);
            //return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Purchases
        [ResponseType(typeof(Purchase))]
        public async Task<IHttpActionResult> PostPurchase(Purchase purchase)
        {
            // From 24 Feb 2018

            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();
            
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    purchase.ShowRoomId = showRoomId;
                    purchase.CreatedBy = userName;
                    purchase.DateCreated = DateTime.Now;
                    purchase.DateUpdated = purchase.DateCreated;
                    purchase.Active = true;
                    db.Purchases.Add(purchase);
                    await db.SaveChangesAsync();
                    if (purchase.PurchaseId > 0)
                    {
                        process = new Process
                        {
                            PurchaseId = purchase.PurchaseId,
                            LotNo = purchase.PChallanNo,
                            PurchasedProductId = purchase.PurchasedProductId,
                            ProcessListId = (int)purchase.ProcessListId,
                            ReceiveQuantity = purchase.DeliveryQuantity,
                            ProcesseLocationId = (int)purchase.ProcesseLocationId,
                            ProcessDate = purchase.PurchaseDate,
                            SE = purchase.SE,
                            Discount = purchase.Discount,
                            ShowRoomId = showRoomId,
                            CreatedBy = userName,
                            DateCreated = DateTime.Now
                        };

                        db.Processes.Add(process);
                        db.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                }

            }
            return CreatedAtRoute("DefaultApi", new { id = purchase.PurchaseId }, purchase);
        }
        [Route("api/StoreDelivery/GetMemoId")]
        [HttpGet]
        public IHttpActionResult GetMemoId()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            DateTime bdate = DateTime.Now;
            string currentMonth = bdate.Month.ToString();
            string currentYear = bdate.Year.ToString();


            Purchase chalanNoObj = new Purchase();
            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT CAST(ISNULL(MAX(RIGHT(PChallanNo, 6)), 0) + 1 AS varchar(20)) AS NewId
                                    FROM  Purchases where ShowRoomId=@showRoomId and year(PurchaseDate)= @year 
                                    and ProcesseLocationId is not null";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@year", currentYear));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        chalanNoObj = new Purchase();
                        if (reader["NewId"] != DBNull.Value)
                        {
                            chalanNoObj.PChallanNo = (string)reader["NewId"];
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(chalanNoObj);
        }


        // DELETE: api/Purchases/5
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult DeletePurchase(int id)
        {
            Purchase purchase = db.Purchases.Find(id);

            var pro = (from item in db.Processes
                       where item.ProcesseLocationId == purchase.ProcesseLocationId &&
                       item.ProcessListId == purchase.ProcessListId &&
                       item.LotNo == purchase.PChallanNo &&
                       item.PurchasedProductId == purchase.PurchasedProductId &&
                       item.ReceiveQuantity == purchase.DeliveryQuantity
                       select new
                       {
                           ProcessId = item.ProcessId
                       }).FirstOrDefault();
            if(pro !=null)
            {
                Process process = db.Processes.Find(pro.ProcessId);
                db.Processes.Remove(process);
                db.SaveChanges();
            }
            


            db.Purchases.Remove(purchase);
            db.SaveChanges();

            if (purchase == null)
            {
                return NotFound();
            }



            return Ok(purchase);
        }

        [Route("api/StoreDelivery/DeleteByLotNo/{lotNo}/{processeLocationId}")]
        [HttpDelete]
        public IHttpActionResult DeleteByLotNo(string lotNo, int processeLocationId)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    string userId = User.Identity.GetUserId();
                    var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
                    string userName = User.Identity.GetUserName();


                    IEnumerable<Process> rpcessesIds = db.Processes.Where(pro => pro.LotNo == lotNo &&
                                                pro.ProcesseLocationId == processeLocationId &&
                                                pro.ShowRoomId == showRoomId);


                    if (rpcessesIds != null)
                    {
                        db.Processes.RemoveRange(rpcessesIds);
                        db.SaveChanges();
                    }

                    IEnumerable<Purchase> purchaseIds = db.Purchases.Where(pur => pur.PChallanNo == lotNo &&
                                                pur.ProcesseLocationId == processeLocationId &&
                                                pur.ShowRoomId == showRoomId);

                    if (purchaseIds != null)
                    {

                        db.Purchases.RemoveRange(purchaseIds);
                        db.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {

                    dbContextTransaction.Rollback();
                }
                
            }
            return Ok();
        }

        [Route("api/StoreDelivery/GetData/{chalanNo}")]
        [HttpGet]
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult GetData(string chalanNo)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var list = (from item in db.Purchases
                        where item.ShowRoomId == showRoomId && item.PChallanNo == chalanNo
                        select new
                        {
                            item.PurchaseId,
                            item.PurchaseDate,
                            item.PChallanNo,
                            item.DeliveryQuantity,
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



            if (list == null)
            {
                return NotFound();
            }

            return Ok(new { list });
            //return Ok(id);
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
