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

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    public class ConversionsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        ////GET: api/Conversions
        //public IEnumerable<Conversion> GetConversions()
        //{
        //    var conversionData = (from con in db.Set<Conversion>()

        //                          join p in db.PurchasedProducts on con.PurchaseProductId equals p.PurchasedProductId
        //                          select new
        //                          {
        //                              ID = con.ConversionId,
        //                              ConversionName = con.ConversionName,
        //                              PurchaseProductName = p.PurchasedProductName,
        //                              PurchasedProductId = p.PurchasedProductId
        //                          }).ToList()
        //                .Select(x => new Conversion
        //                {
        //                    ConversionId = x.ID,
        //                    ConversionName = x.ConversionName,
        //                    PurchaseProductId = x.PurchasedProductId,
        //                    PurchaseProduct = new PurchasedProduct
        //                    {
        //                        PurchasedProductName = x.PurchaseProductName
        //                    }
        //                });

        //    return conversionData;
        //}
        //[Route("api/Conversions/GetPurchaseProductList")]
        //[HttpGet]
        //public IHttpActionResult GetPurchaseProductList()
        //{
        //    var list = (from item in db.PurchasedProducts
        //                select new
        //                {
        //                    item.PurchasedProductId,
        //                    item.PurchasedProductName
        //                }).ToList();
        //    if (list == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(list);
        //}

        [HttpGet]
        //[ResponseType(typeof(VmConversion))]
        public IQueryable<Conversion> GetConversions()
        {
            return db.Conversions;

            //return Ok(List);
        }

        //public IQueryable<Conversion> GetTest()
        //{
        //    return db.Conversions;
        //}

        // GET: api/Conversions/5
        [ResponseType(typeof(Conversion))]
        public IHttpActionResult GetConversion(int id)
        {
            Conversion conversion = db.Conversions.Find(id);
            if (conversion == null)
            {
                return NotFound();
            }

            return Ok(conversion);
        }

        // PUT: api/Conversions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutConversion(int id, Conversion conversion)
        {
            
            conversion.DateUpdated = DateTime.Now;
            conversion.DateCreated= db.Conversions.Where(x => x.ConversionId == id).Select(x => x.DateCreated).FirstOrDefault();
            conversion.Active = true;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != conversion.ConversionId)
            {
                return BadRequest();
            }

            db.Entry(conversion).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Conversions
        [ResponseType(typeof(Conversion))]
        public IHttpActionResult PostConversion(Conversion conversion)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            conversion.DateCreated = DateTime.Now;
            db.Conversions.Add(conversion);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = conversion.ConversionId }, conversion);
        }

        // DELETE: api/Conversions/5
        [ResponseType(typeof(Conversion))]
        public IHttpActionResult DeleteConversion(int id)
        {
            Conversion conversion = db.Conversions.Find(id);
            if (conversion == null)
            {
                return NotFound();
            }

            db.Conversions.Remove(conversion);
            db.SaveChanges();

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