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

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    public class PurchasedProductRatesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/PurchasedProductRates/GetAvailableQuantity/{purchasedProductId}/{OrderNo}")]
        [HttpGet]
        //[ResponseType(typeof(Purchase))]
        public IHttpActionResult GetAvailableQuantity(int purchasedProductId, string OrderNo)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();

            var FinishedGoodStockId = db.FinishedGoodStocks
                .Where(fs => fs.OrderNumber == OrderNo)
                .Select(fs => fs.FinishedGoodStockId)
                .FirstOrDefault();

            var list = db.PurchasedProductRates
                .Select(pr => new {
                    pr.PurchasedProductId,
                    pr.PurchasedProductRateId,
                    pr.FinishedGoodStockId,
                    pr.Quantity,
                    pr.AvgRate,
                    pr.ShowRoomId
                })
                .Where(pr => pr.PurchasedProductId == purchasedProductId && pr.FinishedGoodStockId == FinishedGoodStockId && pr.ShowRoomId==showRoomId)
                .ToList();

            return Ok(list);
        }

        // GET: api/PurchasedProductRates
        public IQueryable<PurchasedProductRate> GetPurchasedProductRates()
        {
            return db.PurchasedProductRates;
        }

        // GET: api/PurchasedProductRates/5
        [ResponseType(typeof(PurchasedProductRate))]
        public async Task<IHttpActionResult> GetPurchasedProductRate(int id)
        {
            PurchasedProductRate purchasedProductRate = await db.PurchasedProductRates.FindAsync(id);
            if (purchasedProductRate == null)
            {
                return NotFound();
            }

            return Ok(purchasedProductRate);
        }

        // PUT: api/PurchasedProductRates/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPurchasedProductRate(int id, PurchasedProductRate purchasedProductRate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchasedProductRate.PurchasedProductRateId)
            {
                return BadRequest();
            }

            db.Entry(purchasedProductRate).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchasedProductRateExists(id))
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

        // POST: api/PurchasedProductRates
        [ResponseType(typeof(PurchasedProductRate))]
        public async Task<IHttpActionResult> PostPurchasedProductRate(PurchasedProductRate purchasedProductRate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PurchasedProductRates.Add(purchasedProductRate);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = purchasedProductRate.PurchasedProductRateId }, purchasedProductRate);
        }

        // DELETE: api/PurchasedProductRates/5
        [ResponseType(typeof(PurchasedProductRate))]
        public async Task<IHttpActionResult> DeletePurchasedProductRate(int id)
        {
            PurchasedProductRate purchasedProductRate = await db.PurchasedProductRates.FindAsync(id);
            if (purchasedProductRate == null)
            {
                return NotFound();
            }

            db.PurchasedProductRates.Remove(purchasedProductRate);
            await db.SaveChangesAsync();

            return Ok(purchasedProductRate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PurchasedProductRateExists(int id)
        {
            return db.PurchasedProductRates.Count(e => e.PurchasedProductRateId == id) > 0;
        }
    }
}