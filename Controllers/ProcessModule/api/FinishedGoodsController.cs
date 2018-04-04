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
    public class FinishedGoodsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/FinishedGoods/GetAllList")]
        [HttpGet]
        public IHttpActionResult GetAllList()
        {
            string userId = User.Identity.GetUserId();           
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var list = (from item in db.FinishedGoods
                        where item.ShowRoomId == showRoomId
                        select new
                        {
                            item.FinishedGoodId,
                            item.FinishedGoodName,
                            item.DesignNo,
                            item.ProductTypeId,
                            item.ProductType.ProductTypeName,
                            item.ShowRoomId,
                            item.CreatedBy,
                            item.Active,
                            item.DateCreated,
                            item.DateUpdated
                        }).ToList();

            var productTypeList = (from item in db.ProductTypes
//                                   where item.ShowRoomId == showRoomId
                                   select new
                                   {
                                       item.ProductTypeId,
                                       item.ProductTypeName
                                   }).ToList();

            return Ok(new { list, productTypeList });
        }

        // GET: api/FinishedGoods
        public IQueryable<FinishedGood> GetFinishedGoods()
        {
            return db.FinishedGoods;
        }

        // GET: api/FinishedGoods/5
        [ResponseType(typeof(FinishedGood))]
        public async Task<IHttpActionResult> GetFinishedGood(int id)
        {
            FinishedGood finishedGood = await db.FinishedGoods.FindAsync(id);
            if (finishedGood == null)
            {
                return NotFound();
            }

            return Ok(finishedGood);
        }

        // PUT: api/FinishedGoods/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFinishedGood(int id, FinishedGood finishedGood)
        {            

            try
            {
                var obj = db.FinishedGoods.FirstOrDefault(m => m.FinishedGoodId == id);
                finishedGood.CreatedBy = obj.CreatedBy;
                finishedGood.ShowRoomId = obj.ShowRoomId;
                finishedGood.DateCreated = obj.DateCreated;
                finishedGood.DateUpdated = DateTime.Now;
                finishedGood.Active = true;
                db.FinishedGoods.AddOrUpdate(finishedGood);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinishedGoodExists(id))
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

        // POST: api/FinishedGoods
        [ResponseType(typeof(FinishedGood))]
        public async Task<IHttpActionResult> PostFinishedGood(FinishedGood finishedGood)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            try
            {
                finishedGood.ShowRoomId = showRoomId;
                finishedGood.CreatedBy = userName;
                finishedGood.DateCreated = DateTime.Now;
                finishedGood.DateUpdated = finishedGood.DateCreated;
                finishedGood.Active = true;
                db.FinishedGoods.Add(finishedGood);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = finishedGood.FinishedGoodId }, finishedGood);
        }

        // DELETE: api/FinishedGoods/5
        [ResponseType(typeof(FinishedGood))]
        public async Task<IHttpActionResult> DeleteFinishedGood(int id)
        {
            FinishedGood finishedGood = await db.FinishedGoods.FindAsync(id);
            if (finishedGood == null)
            {
                return NotFound();
            }

            db.FinishedGoods.Remove(finishedGood);
            await db.SaveChangesAsync();

            return Ok(finishedGood);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FinishedGoodExists(int id)
        {
            return db.FinishedGoods.Count(e => e.FinishedGoodId == id) > 0;
        }
    }
}