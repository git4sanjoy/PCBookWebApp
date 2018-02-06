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
    //[Authorize]
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
                        }).ToList();

            var prosessList = (from item in db.ProcessLists
                               select new
                               {
                                   item.ProcessListId,
                                   item.ProcessListName
                               }).ToList();
            var purchasedProductList = (from item in db.PurchasedProducts
                                        select new
                                        {
                                            item.PurchasedProductId,
                                            item.PurchasedProductName
                                        }).ToList();
            var processeLocationList = (from item in db.ProcesseLocations
                                        select new
                                        {
                                            item.ProcesseLocationId,
                                            item.ProcesseLocationName
                                        }).ToList();
            var conversionList = (from item in db.Conversions
                                  select new
                                  {
                                      item.ConversionId,
                                      item.ConversionName
                                  }).ToList();
            var showRoomList = (from item in db.ShowRooms
                                select new
                                {
                                    item.ShowRoomId,
                                    item.ShowRoomName
                                }).ToList();
            //if (list == null)
            //{
            //    return NotFound();
            //}

            return Ok(new { list, prosessList, purchasedProductList, processeLocationList, conversionList, showRoomList });
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != process.ProcessId)
            {
                return BadRequest();
            }

            db.Entry(process).State = EntityState.Modified;

            try
            {
                var createdDate = db.Processes.Where(x => x.ProcessId == id).Select(x => x.DateCreated).FirstOrDefault();
                process.DateCreated = createdDate;
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
        [ResponseType(typeof(Process))]
        public async Task<IHttpActionResult> PostProcess(Process process)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
    }
}