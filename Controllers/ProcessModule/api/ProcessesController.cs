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

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    [Authorize]
    public class ProcessesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/Processes/GetAllList")]
        [HttpGet]
        public IHttpActionResult GetAllList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var list = (from item in db.Processes
                        where item.ShowRoomId == showRoomId
                        select new
                        {
                            item.ProcessId,
                            item.ProcessDate,
                            item.LotNo,
                            item.DeliveryQuantity,
                            item.ReceiveQuantity,
                            item.SE,
                            item.Amount,
                            item.Rate,
                            item.Discount,
                            item.ProcessListId,
                            item.ProcessList.ProcessListName,
                            item.PurchasedProductId,
                            item.PurchasedProduct.PurchasedProductName,
                            item.ProcesseLocationId,
                            item.ProcesseLocation.ProcesseLocationName,
                            item.ConversionId,
                            item.Conversion.ConversionName,
                            item.ShowRoomId,
                            item.ShowRoom.ShowRoomName,
                            item.Active,
                            item.CreatedBy,
                            item.DateCreated,
                            item.DateUpdated
                        }).ToList().GroupBy(x => new { x.ProcessDate, x.LotNo, x.ProcesseLocationId, x.ProcessListId, x.ShowRoomId })
                        .Select(
                                g => new
                                {
                                    Key = g.Key,
                                    ProcessId = g.First().ProcessId,
                                    ProcessListId = g.First().ProcessListId,
                                    ProcessListName = g.First().ProcessListName,
                                    ProcesseLocationId = g.First().ProcesseLocationId,
                                    ProcesseLocationName = g.First().ProcesseLocationName,
                                    ProcessDate = g.First().ProcessDate,
                                    LotNo = g.First().LotNo,
                                    DeliveryQuantity = g.Sum(s => s.DeliveryQuantity),
                                    ReceiveQuantity = g.Sum(s => s.ReceiveQuantity),
                                    SE = g.Sum(s => s.SE),
                                    ConversionId = g.First().ConversionId,
                                    ConversionName = g.First().ConversionName,
                                    PurchasedProductId = g.First().PurchasedProductId,
                                    PurchasedProductName = g.First().PurchasedProductName,
                                    ShowRoomId = g.First().ShowRoomId,
                                    ShowRoomName = g.First().ShowRoomName,
                                    Active = g.First().Active,
                                    TotalProduct = g.Count()
                                }).Where(q => q.ReceiveQuantity != (q.DeliveryQuantity + q.SE));

            var prosessList = (from item in db.ProcessLists
                               where item.ShowRoomId == showRoomId && item.ProcessListName != "Short/Excess" && item.ProcessListName != "Finished"
                               select new
                               {
                                   item.ProcessListId,
                                   item.ProcessListName
                               }).ToList();
            var prosessListAll = (from item in db.ProcessLists
                                  where item.ShowRoomId == showRoomId
                                  select new
                                  {
                                      item.ProcessListId,
                                      item.ProcessListName
                                  }).ToList();
            var purchasedProductList = (from item in db.PurchasedProducts
                                        where item.ShowRoomId == showRoomId
                                        select new
                                        {
                                            item.PurchasedProductId,
                                            item.PurchasedProductName
                                        }).ToList();
            var processeLocationList = (from item in db.ProcesseLocations
                                        where item.ShowRoomId == showRoomId
                                        select new
                                        {
                                            item.ProcesseLocationId,
                                            item.ProcesseLocationName
                                        }).ToList();
            var conversionList = (from item in db.Conversions
                                  where item.ShowRoomId == showRoomId
                                  select new
                                  {
                                      item.ConversionId,
                                      item.ConversionName
                                  }).ToList();
            //var showRoomList = (from item in db.ShowRooms
            //                    select new
            //                    {
            //                        item.ShowRoomId,
            //                        item.ShowRoomName
            //                    }).ToList();
            //if (list == null)
            //{
            //    return NotFound();
            //}

            return Ok(new { list, prosessList, purchasedProductList, processeLocationList, conversionList, prosessListAll });
        }

        [HttpGet, Route("api/Processes/GetProcessData/{processeLocationId}/{processListId}/{lotNo}")]
        public IHttpActionResult GetProcessData(int processeLocationId, int processListId, string lotNo)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var process = (from item in db.Processes
                           where item.ProcessListId == processListId && item.LotNo == lotNo
                           && item.ShowRoomId == showRoomId
                           && item.DeliveryQuantity == 0
                           && item.SE == 0
                           select new
                           {
                               item.ProcessId,
                               item.ProcessDate,
                               item.LotNo,
                               item.DeliveryQuantity,
                               item.ReceiveQuantity,
                               item.SE,
                               item.Amount,
                               item.Rate,
                               item.Discount,
                               item.ProcessListId,
                               item.ProcessList.ProcessListName,
                               item.PurchasedProductId,
                               item.PurchasedProduct.PurchasedProductName,
                               item.ProcesseLocationId,
                               item.ProcesseLocation.ProcesseLocationName,
                               item.ConversionId,
                               item.Conversion.ConversionName,
                               item.ShowRoomId,
                               item.ShowRoom.ShowRoomName,
                               item.Active,
                               item.CreatedBy,
                               item.DateCreated,
                               item.DateUpdated
                           }).ToList();

            return Ok(process);
        }

        [HttpGet, Route("api/Processes/LotNoList/{ProcesseLocationId}/{ProcessListId}")]
        public IHttpActionResult GetLotNoList(int ProcesseLocationId, int ProcessListId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();

            var lotNoList = (from item in db.Processes
                             where item.ProcesseLocationId == ProcesseLocationId
                             && item.ProcessListId == ProcessListId
                             && item.ShowRoomId == showRoomId
                             && item.DeliveryQuantity == 0
                             select new { item.LotNo }).Distinct();

            return Ok(lotNoList);
        }

        // GET: api/Processes
        public IQueryable<Process> GetProcesses()
        {
            return db.Processes;
        }

        // GET: api/Processes/5
        [ResponseType(typeof(Process))]
        public async Task<IHttpActionResult> GetProcess(int id)
        {
            Process process = await db.Processes.FindAsync(id);
            if (process == null)
            {
                return NotFound();
            }

            return Ok(process);
        }

        // PUT: api/Processes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProcess(int id, Process process)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (id != process.ProcessId)
            //{
            //    return BadRequest();
            //}

            //db.Entry(process).State = EntityState.Modified;

            try
            {
                var obj = db.Processes.Where(x => x.ProcessId == id).FirstOrDefault();
                process.ShowRoomId = obj.ShowRoomId;
                process.DateCreated = obj.DateCreated;
                process.CreatedBy = obj.CreatedBy;
                process.DateUpdated = DateTime.Now;
                process.Active = true;
                db.Processes.AddOrUpdate(process);
                await db.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcessExists(id))
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

        // POST: api/Processes       
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostProcess(Process process)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                process.ShowRoomId = showRoomId;
                process.CreatedBy = userName;
                process.DateCreated = DateTime.Now;
                process.DateUpdated = process.DateCreated;
                process.Active = true;
                db.Processes.Add(process);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return CreatedAtRoute("DefaultApi", new { id = process.ProcessId }, process);
        }

        // DELETE: api/Processes/5
        [ResponseType(typeof(Process))]
        public async Task<IHttpActionResult> DeleteProcess(int id)
        {
            Process process = await db.Processes.FindAsync(id);
            if (process == null)
            {
                return NotFound();
            }

            db.Processes.Remove(process);
            await db.SaveChangesAsync();

            return Ok(process);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProcessExists(int id)
        {
            return db.Processes.Count(e => e.ProcessId == id) > 0;
        }

        [Route("api/Processes/GetSearch/{fromdate}/{todate}/{processeLocationId}/{processListId}")]
        [HttpGet]
        public IHttpActionResult GetSearch(DateTime? fromdate, DateTime? todate, int? processeLocationId, int? processListId)
        {
            object list = new List<object>();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();

            if (fromdate != null && todate != null && processeLocationId == 0 && processListId == 0)
            {
                list = (from item in db.Processes
                        where item.ShowRoomId == showRoomId
                        && (item.ProcessDate >= fromdate && item.ProcessDate <= todate)
                        select new
                        {
                            item.ProcessId,
                            item.ProcessDate,
                            item.LotNo,
                            item.DeliveryQuantity,
                            item.ReceiveQuantity,
                            item.SE,
                            item.Amount,
                            item.Rate,
                            item.Discount,
                            item.ProcessListId,
                            item.ProcessList.ProcessListName,
                            item.PurchasedProductId,
                            item.PurchasedProduct.PurchasedProductName,
                            item.ProcesseLocationId,
                            item.ProcesseLocation.ProcesseLocationName,
                            item.ConversionId,
                            item.Conversion.ConversionName,
                            item.ShowRoomId,
                            item.ShowRoom.ShowRoomName,
                            item.Active,
                            item.CreatedBy,
                            item.DateCreated,
                            item.DateUpdated
                        }).ToList().GroupBy(x => new { x.ProcessDate, x.LotNo, x.ProcesseLocationId, x.ProcessListId, x.ShowRoomId })
                            .Select(
                                    g => new
                                    {
                                        Key = g.Key,
                                        ProcessId = g.First().ProcessId,
                                        ProcessListId = g.First().ProcessListId,
                                        ProcessListName = g.First().ProcessListName,
                                        ProcesseLocationId = g.First().ProcesseLocationId,
                                        ProcesseLocationName = g.First().ProcesseLocationName,
                                        ProcessDate = g.First().ProcessDate,
                                        LotNo = g.First().LotNo,
                                        DeliveryQuantity = g.Sum(s => s.DeliveryQuantity),
                                        ReceiveQuantity = g.Sum(s => s.ReceiveQuantity),
                                        SE = g.Sum(s => s.SE),
                                        ConversionId = g.First().ConversionId,
                                        ConversionName = g.First().ConversionName,
                                        PurchasedProductId = g.First().PurchasedProductId,
                                        PurchasedProductName = g.First().PurchasedProductName,
                                        ShowRoomId = g.First().ShowRoomId,
                                        ShowRoomName = g.First().ShowRoomName,
                                        Active = g.First().Active,
                                        TotalProduct = g.Count()
                                    }).Where(q => q.ReceiveQuantity != (q.DeliveryQuantity + q.SE));
            }
            else if (fromdate != null && todate != null && processeLocationId != 0 && processListId == 0)
            {
                list = (from item in db.Processes
                        where item.ShowRoomId == showRoomId
                        && (item.ProcessDate >= fromdate && item.ProcessDate <= todate)
                        && item.ProcesseLocationId == processeLocationId
                        select new
                        {
                            item.ProcessId,
                            item.ProcessDate,
                            item.LotNo,
                            item.DeliveryQuantity,
                            item.ReceiveQuantity,
                            item.SE,
                            item.Amount,
                            item.Rate,
                            item.Discount,
                            item.ProcessListId,
                            item.ProcessList.ProcessListName,
                            item.PurchasedProductId,
                            item.PurchasedProduct.PurchasedProductName,
                            item.ProcesseLocationId,
                            item.ProcesseLocation.ProcesseLocationName,
                            item.ConversionId,
                            item.Conversion.ConversionName,
                            item.ShowRoomId,
                            item.ShowRoom.ShowRoomName,
                            item.Active,
                            item.CreatedBy,
                            item.DateCreated,
                            item.DateUpdated
                        }).ToList().GroupBy(x => new { x.ProcessDate, x.LotNo, x.ProcesseLocationId, x.ProcessListId, x.ShowRoomId })
                            .Select(
                                    g => new
                                    {
                                        Key = g.Key,
                                        ProcessId = g.First().ProcessId,
                                        ProcessListId = g.First().ProcessListId,
                                        ProcessListName = g.First().ProcessListName,
                                        ProcesseLocationId = g.First().ProcesseLocationId,
                                        ProcesseLocationName = g.First().ProcesseLocationName,
                                        ProcessDate = g.First().ProcessDate,
                                        LotNo = g.First().LotNo,
                                        DeliveryQuantity = g.Sum(s => s.DeliveryQuantity),
                                        ReceiveQuantity = g.Sum(s => s.ReceiveQuantity),
                                        SE = g.Sum(s => s.SE),
                                        ConversionId = g.First().ConversionId,
                                        ConversionName = g.First().ConversionName,
                                        PurchasedProductId = g.First().PurchasedProductId,
                                        PurchasedProductName = g.First().PurchasedProductName,
                                        ShowRoomId = g.First().ShowRoomId,
                                        ShowRoomName = g.First().ShowRoomName,
                                        Active = g.First().Active,
                                        TotalProduct = g.Count()
                                    }).Where(q => q.ReceiveQuantity != (q.DeliveryQuantity + q.SE));
            }
            else if (fromdate != null && todate != null && processeLocationId == 0 && processListId != 0)
            {
                list = (from item in db.Processes
                        where item.ShowRoomId == showRoomId
                        && (item.ProcessDate >= fromdate && item.ProcessDate <= todate)
                          && item.ProcessListId == processListId
                        select new
                        {
                            item.ProcessId,
                            item.ProcessDate,
                            item.LotNo,
                            item.DeliveryQuantity,
                            item.ReceiveQuantity,
                            item.SE,
                            item.Amount,
                            item.Rate,
                            item.Discount,
                            item.ProcessListId,
                            item.ProcessList.ProcessListName,
                            item.PurchasedProductId,
                            item.PurchasedProduct.PurchasedProductName,
                            item.ProcesseLocationId,
                            item.ProcesseLocation.ProcesseLocationName,
                            item.ConversionId,
                            item.Conversion.ConversionName,
                            item.ShowRoomId,
                            item.ShowRoom.ShowRoomName,
                            item.Active,
                            item.CreatedBy,
                            item.DateCreated,
                            item.DateUpdated
                        }).ToList().GroupBy(x => new { x.ProcessDate, x.LotNo, x.ProcesseLocationId, x.ProcessListId, x.ShowRoomId })
                            .Select(
                                    g => new
                                    {
                                        Key = g.Key,
                                        ProcessId = g.First().ProcessId,
                                        ProcessListId = g.First().ProcessListId,
                                        ProcessListName = g.First().ProcessListName,
                                        ProcesseLocationId = g.First().ProcesseLocationId,
                                        ProcesseLocationName = g.First().ProcesseLocationName,
                                        ProcessDate = g.First().ProcessDate,
                                        LotNo = g.First().LotNo,
                                        DeliveryQuantity = g.Sum(s => s.DeliveryQuantity),
                                        ReceiveQuantity = g.Sum(s => s.ReceiveQuantity),
                                        SE = g.Sum(s => s.SE),
                                        ConversionId = g.First().ConversionId,
                                        ConversionName = g.First().ConversionName,
                                        PurchasedProductId = g.First().PurchasedProductId,
                                        PurchasedProductName = g.First().PurchasedProductName,
                                        ShowRoomId = g.First().ShowRoomId,
                                        ShowRoomName = g.First().ShowRoomName,
                                        Active = g.First().Active,
                                        TotalProduct = g.Count()
                                    }).Where(q => q.ReceiveQuantity != (q.DeliveryQuantity + q.SE));
            }
            else if (fromdate != null && todate != null && processeLocationId != 0 && processListId != 0)
            {
                list = (from item in db.Processes
                        where item.ShowRoomId == showRoomId
                        && (item.ProcessDate >= fromdate && item.ProcessDate <= todate)
                         && item.ProcesseLocationId == processeLocationId
                          && item.ProcessListId == processListId
                        select new
                        {
                            item.ProcessId,
                            item.ProcessDate,
                            item.LotNo,
                            item.DeliveryQuantity,
                            item.ReceiveQuantity,
                            item.SE,
                            item.Amount,
                            item.Rate,
                            item.Discount,
                            item.ProcessListId,
                            item.ProcessList.ProcessListName,
                            item.PurchasedProductId,
                            item.PurchasedProduct.PurchasedProductName,
                            item.ProcesseLocationId,
                            item.ProcesseLocation.ProcesseLocationName,
                            item.ConversionId,
                            item.Conversion.ConversionName,
                            item.ShowRoomId,
                            item.ShowRoom.ShowRoomName,
                            item.Active,
                            item.CreatedBy,
                            item.DateCreated,
                            item.DateUpdated
                        }).ToList().GroupBy(x => new { x.ProcessDate, x.LotNo, x.ProcesseLocationId, x.ProcessListId, x.ShowRoomId })
                             .Select(
                                     g => new
                                     {
                                         Key = g.Key,
                                         ProcessId = g.First().ProcessId,
                                         ProcessListId = g.First().ProcessListId,
                                         ProcessListName = g.First().ProcessListName,
                                         ProcesseLocationId = g.First().ProcesseLocationId,
                                         ProcesseLocationName = g.First().ProcesseLocationName,
                                         ProcessDate = g.First().ProcessDate,
                                         LotNo = g.First().LotNo,
                                         DeliveryQuantity = g.Sum(s => s.DeliveryQuantity),
                                         ReceiveQuantity = g.Sum(s => s.ReceiveQuantity),
                                         SE = g.Sum(s => s.SE),
                                         ConversionId = g.First().ConversionId,
                                         ConversionName = g.First().ConversionName,
                                         PurchasedProductId = g.First().PurchasedProductId,
                                         PurchasedProductName = g.First().PurchasedProductName,
                                         ShowRoomId = g.First().ShowRoomId,
                                         ShowRoomName = g.First().ShowRoomName,
                                         Active = g.First().Active,
                                         TotalProduct = g.Count()
                                     }).Where(q => q.ReceiveQuantity != (q.DeliveryQuantity + q.SE));
            }
            return Ok(list);
        }

        [HttpGet, Route("api/Processes/GetProcessDataForEdit/{processDate}/{ProcesseLocationId}/{processListId}/{lotNo}")]
        public IHttpActionResult GetProcessDataForEdit(DateTime processDate, int processeLocationId, int processListId, string lotNo)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var process = (from item in db.Processes
                           where item.ProcessListId == processListId && item.LotNo == lotNo
                           && item.ProcesseLocationId == processeLocationId
                           && item.ProcessDate == processDate
                           && item.ShowRoomId == showRoomId
                           && item.PurchaseId == null
                           select new
                           {
                               item.ProcessId,
                               item.ProcessDate,
                               item.LotNo,
                               item.DeliveryQuantity,
                               item.ReceiveQuantity,
                               item.SE,
                               item.Amount,
                               item.Rate,
                               item.Discount,
                               item.ProcessListId,
                               item.ProcessList.ProcessListName,
                               item.PurchasedProductId,
                               item.PurchasedProduct.PurchasedProductName,
                               item.ProcesseLocationId,
                               item.ProcesseLocation.ProcesseLocationName,
                               item.ConversionId,
                               item.Conversion.ConversionName,
                               item.ShowRoomId,
                               item.ShowRoom.ShowRoomName,
                               item.Active,
                               item.CreatedBy,
                               item.DateCreated,
                               item.DateUpdated
                           }).ToList();

            return Ok(process);
        }


        //[HttpGet, Route("api/Processes/GetReceiveQuantity/{ProcesseLocationId}/{processListId}/{lotNo}/{productId}/{deliveryQuantity}")]
        //public IHttpActionResult GetReceiveQuantity(int ProcesseLocationId, int processListId, string lotNo, int productId, double deliveryQuantity)
        //{
        //    string userId = User.Identity.GetUserId();
        //    var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
        //    var process = (from item in db.Processes
        //                   where item.ProcessListId == processListId && item.LotNo == lotNo
        //                   && item.ShowRoomId == showRoomId
        //                   && item.PurchasedProductId == productId
        //                   select new
        //                   {
        //                       item.ProcessId,
        //                       item.ProcessDate,
        //                       item.LotNo,
        //                       item.DeliveryQuantity,
        //                       item.ReceiveQuantity,
        //                       item.SE,
        //                       item.Amount,
        //                       item.Rate,
        //                       item.Discount,
        //                       item.ProcessListId,
        //                       item.ProcessList.ProcessListName,
        //                       item.PurchasedProductId,
        //                       item.PurchasedProduct.PurchasedProductName,
        //                       item.ProcesseLocationId,
        //                       item.ProcesseLocation.ProcesseLocationName,
        //                       item.ConversionId,
        //                       item.Conversion.ConversionName,
        //                       item.ShowRoomId,
        //                       item.ShowRoom.ShowRoomName
        //                   }).ToList().GroupBy(x => new { x.LotNo, x.ProcesseLocationId, x.ProcessListId, x.ShowRoomId, x.PurchasedProductId })
        //                .Select(
        //                        g => new
        //                        {
        //                            Key = g.Key,                                   
        //                            DeliveryQuantity = (g.Sum(s => s.ReceiveQuantity)) 
        //                                                  - (g.Sum(s => s.DeliveryQuantity) + g.Sum(s => s.SE) + deliveryQuantity),
        //                        }).Where(q => q.DeliveryQuantity >= 0 );
        //    return Ok(process);
        //}
    }
}