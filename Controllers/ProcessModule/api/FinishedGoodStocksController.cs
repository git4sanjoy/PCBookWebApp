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
    public class FinishedGoodStocksController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/FinishedGoodStocks
        public IQueryable<FinishedGoodStock> GetFinishedGoodStocks()
        {
            return db.FinishedGoodStocks;
        }

        // GET: api/FinishedGoodStocks/5
        [ResponseType(typeof(FinishedGoodStock))]
        public async Task<IHttpActionResult> GetFinishedGoodStock(int id)
        {
            FinishedGoodStock finishedGoodStock = await db.FinishedGoodStocks.FindAsync(id);
            if (finishedGoodStock == null)
            {
                return NotFound();
            }

            return Ok(finishedGoodStock);
        }

        // PUT: api/FinishedGoodStocks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFinishedGoodStock(int id, FinishedGoodStock finishedGoodStock)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (id != finishedGoodStock.FinishedGoodStockId)
            //{
            //    return BadRequest();
            //}

            //db.Entry(finishedGoodStock).State = EntityState.Modified;

            try
            {
                var obj = db.FinishedGoodStocks.FirstOrDefault(m => m.FinishedGoodStockId == id);
                finishedGoodStock.CreatedBy = obj.CreatedBy;
                finishedGoodStock.ShowRoomId = obj.ShowRoomId;
                finishedGoodStock.DateCreated = obj.DateCreated;
                finishedGoodStock.DateUpdated = DateTime.Now;
                finishedGoodStock.Active = true;
                db.FinishedGoodStocks.AddOrUpdate(finishedGoodStock);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinishedGoodStockExists(id))
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

        // POST: api/FinishedGoodStocks
        [ResponseType(typeof(FinishedGoodStock))]
        public async Task<IHttpActionResult> PostFinishedGoodStock(FinishedGoodStock finishedGoodStock)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            try
            {
                finishedGoodStock.ShowRoomId = showRoomId;
                finishedGoodStock.CreatedBy = userName;
                finishedGoodStock.DateCreated = DateTime.Now;
                finishedGoodStock.DateUpdated = finishedGoodStock.DateCreated;
                finishedGoodStock.Active = true;
                db.FinishedGoodStocks.Add(finishedGoodStock);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }            

            return CreatedAtRoute("DefaultApi", new { id = finishedGoodStock.FinishedGoodStockId }, finishedGoodStock);
        }

        // DELETE: api/FinishedGoodStocks/5
        [ResponseType(typeof(FinishedGoodStock))]
        public async Task<IHttpActionResult> DeleteFinishedGoodStock(int id)
        {
            FinishedGoodStock finishedGoodStock = await db.FinishedGoodStocks.FindAsync(id);
            if (finishedGoodStock == null)
            {
                return NotFound();
            }

            db.FinishedGoodStocks.Remove(finishedGoodStock);
            await db.SaveChangesAsync();

            return Ok(finishedGoodStock);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FinishedGoodStockExists(int id)
        {
            return db.FinishedGoodStocks.Count(e => e.FinishedGoodStockId == id) > 0;
        }
        [Route("api/FinishedGoodStocks/GetAllList")]
        [HttpGet]
        public IHttpActionResult GetAllList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var list = (from item in db.FinishedGoodStocks
                        where item.ShowRoomId == showRoomId
                        && item.OrderQuantity != 0
                        select new
                        {
                            item.FinishedGoodStockId,
                            item.FinishedGoodId,
                            item.FinishedGood.FinishedGoodName,
                            item.OrderQuantity,
                            item.ReceiveDate,
                            item.OrderNumber,
                            item.BuyerName,                            
                            item.ShowRoomId,
                            item.CreatedBy,
                            item.Active,
                            item.DateCreated,
                            item.DateUpdated
                        }).ToList();

            var finishedGoodList = (from item in db.FinishedGoods
                                   where item.ShowRoomId == showRoomId
                                   select new
                                   {
                                       item.FinishedGoodId,
                                       item.FinishedGoodName
                                   }).ToList();

            return Ok(new { list, finishedGoodList });
        }
    }
}