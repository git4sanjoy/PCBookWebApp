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
using PCBookWebApp.Models.ProcessModule.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using System.Configuration;
using System.Data.SqlClient;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    [Authorize]
    public class ConversionsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        
        [Route("api/Conversions/ConversionsList")]
        [HttpGet]
        public IHttpActionResult ConversionsList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.Conversions
                            .Where(d => d.ShowRoomId == showRoomId)
                            .OrderBy(d => d.ConversionId)
                            .Select(e => new
                            {
                                id = e.ConversionId,
                                ConversionName = e.ConversionName,
                                MatricId1 = e.MatricId1,
                                MatricId2 = e.MatricId2,
                                createDate = e.DateCreated
                            });

            //var list = (from conv in db.Conversions
            //            join m1 in db.Matrics on conv.MatricId1 equals m1.MatricId
            //            join m2 in db.Matrics on conv.MatricId2 equals m2.MatricId
            //            select new
            //            {
            //               id= conv.ConversionId,
            //               name= conv.ConversionName,
            //                status = m1.MatricName
            //               //mat2= m2.MatricName
            //            }).ToList();


            //var list = db.Conversions
            //                .Include(a => a.MatricId1)
            //                .Include(a => a.MatricId2)
            //                .Where(d => d.ShowRoomId == showRoomId)
            //                .Select(e => new {
            //                    id = e.ConversionId,
            //                    name = e.ConversionName,
            //                    group = e.MatricId1,
            //                    groupName = e.MatricId1
            //                });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/Conversions/MatricsListXEdit")]
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
                            .Where(m => m.ShowRoomId == showRoomId)
                            .Select(e => new { value = e.MatricId, text = e.MatricName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        
        // PUT: api/PurchasedProducts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUpdateConversion(int id, Conversion conversion)
        {
            var msg = 0;
            
            if (id != conversion.ConversionId)
            {
                return BadRequest();
            }
            try
            {
                var obj = db.Conversions.FirstOrDefault(m => m.ConversionId == conversion.ConversionId);
                conversion.DateCreated = obj.DateCreated;
                conversion.CreatedBy = obj.CreatedBy;
                conversion.DateUpdated = DateTime.Now;
                conversion.ShowRoomId = obj.ShowRoomId;
                conversion.Active = true;

                db.Conversions.AddOrUpdate(conversion);
                await db.SaveChangesAsync();
                msg = 1;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConversionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(msg);
        }

        // POST: api/PurchasedProducts
        [ResponseType(typeof(PurchasedProduct))]
        public async Task<IHttpActionResult> PostSaveConversion(Conversion conversion)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            conversion.CreatedBy = userName;
            conversion.ShowRoomId = showRoomId;
            conversion.DateCreated = DateTime.Now;
            conversion.Active = true;
            db.Conversions.Add(conversion);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = conversion.ConversionId }, conversion);
        }

        // DELETE: api/PurchasedProducts/5
        [ResponseType(typeof(PurchasedProduct))]
        public async Task<IHttpActionResult> DeletePurchasedProduct(int id)
        {
            Conversion conversion = await db.Conversions.FindAsync(id);
            if (conversion == null)
            {
                return NotFound();
            }

            db.Conversions.Remove(conversion);
            await db.SaveChangesAsync();

            return Ok(conversion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ConversionExists(int id)
        {
            return db.Conversions.Count(e => e.ConversionId == id) > 0;
        }
    }
}