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
    public class ConversionDetailsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/ConversionDetails/GetPurchaseProductList")]
        [HttpGet]
        public IHttpActionResult GetPurchaseProductList()
        {
            var list = (from item in db.PurchasedProducts
                        select new
                        {
                            item.PurchasedProductId,
                            item.PurchasedProductName
                        }).ToList();
            var list2 = (from item in db.Conversions
                       select new
                       {
                           item.ConversionId,
                           item.ConversionName
                       }).ToList();
            if (list == null)
            {
                return NotFound();
            }

            return Ok(new { list, list2 });
        }

        [HttpGet]
        [ResponseType(typeof(VmConversionDetails))]
        public IHttpActionResult GetConversionDetails()
        {
            //var conversionDetail = (from cd in db.Set<ConversionDetail>()
            //            join c in db.Conversions on cd.ConversionId equals c.ConversionId
            //                        join p in db.PurchasedProducts on cd.PurchaseProductId equals p.PurchasedProductId
            //            select new
            //            {
            //                ID = cd.ConversionDetailsId,
            //                ConversionName = c.ConversionName,
            //                PurchaseProductName = p.PurchasedProductName

            //            }).ToList()
            //            .Select(x => new ConversionDetail
            //            {
            //                ConversionDetailsId = x.ID,
            //                Conversion = new Conversion
            //                {
            //                    ConversionName = x.ConversionName
            //                },
            //                PurchaseProduct = new PurchasedProduct
            //                {
            //                    PurchasedProductName = x.PurchaseProductName
            //                }

            //            });
            List<VmConversionDetails> List = new List<VmConversionDetails>();
            VmConversionDetails Obj = new VmConversionDetails();

            var conversionDetailList = (from item in db.ConversionDetails
                                  select new
                                  {
                                      item.ConversionDetailsId,
                                      item.ConversionId,
                                      item.Conversion.ConversionName,
                                      item.PurchaseProductId,
                                      item.PurchaseProduct.PurchasedProductName,                                      
                                      item.Active,
                                      item.CreatedBy,
                                      item.DateCreated,
                                      item.DateUpdated
                                  }).ToList();
            foreach (var item in conversionDetailList)
            {
                Obj = new VmConversionDetails();
                {
                    Obj.ConversionDetailsId = item.ConversionDetailsId;                    
                    Obj.ConversionId = item.ConversionId.ToString();
                    Obj.PurchaseProductId = item.PurchaseProductId.ToString();
                    Obj.ConversionName = item.ConversionName;
                    Obj.PurchasedProductName = item.PurchasedProductName;
                };
                List.Add(Obj);
            }
            if (List == null)
            {
                return NotFound();
            }

            return Ok(List);
        }

        // GET: api/ConversionDetails/5
        [ResponseType(typeof(ConversionDetail))]
        public IHttpActionResult GetConversionDetail(int id)
        {
            ConversionDetail conversionDetail = db.ConversionDetails.Find(id);
            if (conversionDetail == null)
            {
                return NotFound();
            }

            return Ok(conversionDetail);
        }

        // PUT: api/ConversionDetails/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutConversionDetail(int id, ConversionDetail conversionDetail)
        {
            conversionDetail.DateCreated = db.ConversionDetails.Where(x => x.ConversionDetailsId == id).Select(x => x.DateCreated).FirstOrDefault();
            conversionDetail.DateUpdated = DateTime.Now;
            conversionDetail.Active = true;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != conversionDetail.ConversionDetailsId)
            {
                return BadRequest();
            }

            db.Entry(conversionDetail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConversionDetailExists(id))
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

        // POST: api/ConversionDetails
        [ResponseType(typeof(ConversionDetail))]
        public IHttpActionResult PostConversionDetail(ConversionDetail conversionDetail)
        {
            var msg = 0;
            var check = db.ConversionDetails.FirstOrDefault(m => m.PurchaseProductId == conversionDetail.PurchaseProductId 
                                                                 && m.ConversionId == conversionDetail.ConversionId);

            if(check == null)
            {
                conversionDetail.DateCreated = DateTime.Now;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.ConversionDetails.Add(conversionDetail);
                db.SaveChanges();
                msg = 1;
            }
            return Ok(msg);
            //return CreatedAtRoute("DefaultApi", new { id = conversionDetail.ConversionDetailsId }, conversionDetail);
        }

        // DELETE: api/ConversionDetails/5
        [ResponseType(typeof(ConversionDetail))]
        public IHttpActionResult DeleteConversionDetail(int id)
        {
            ConversionDetail conversionDetail = db.ConversionDetails.Find(id);
            if (conversionDetail == null)
            {
                return NotFound();
            }

            db.ConversionDetails.Remove(conversionDetail);
            db.SaveChanges();

            return Ok(conversionDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ConversionDetailExists(int id)
        {
            return db.ConversionDetails.Count(e => e.ConversionDetailsId == id) > 0;
        }
    }
}