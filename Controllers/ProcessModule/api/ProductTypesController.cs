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
using Newtonsoft.Json;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    [Authorize]
    public class ProductTypesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/ProductTypes 
        public IQueryable<ProductType> GetProductTypes()
        {            
            return db.ProductTypes;
        }
        [Route("api/ProductTypes/GetProductTypeList")]
        [HttpGet]
        public IHttpActionResult GetProductTypeList()
        {
            var list = (from item in db.ProductTypes
                        select new
                        {
                            item.ProductTypeId,
                            item.ProductTypeName,
                            item.ShowRoomId,
                            item.ShowRoom.ShowRoomName,
                            item.Active,
                            item.CreatedBy,
                            item.DateCreated,
                            item.DateUpdated
                        }).ToList();
            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }

        //public async Task<IHttpActionResult> GetProductType(int id)
        //{
        //    ProductType productType = await db.ProductTypes.FindAsync(id);
        //    if (productType == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(productType);
        //}

        //// GET: api/ProductTypes/5
        //[ResponseType(typeof(ProductType))]
        //public async Task<IHttpActionResult> GetProductTypes(int id)
        //{
        //    ProductType productType = await db.ProductTypes.FindAsync(id);
        //    if (productType == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(productType);
        //}

        // PUT: api/ProductTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProductType(int id, ProductType productType)
        {
            var msg = 0;
            var check = db.ProductTypes.FirstOrDefault(m => m.ProductTypeName == productType.ProductTypeName);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != productType.ProductTypeId)
            {
                return BadRequest();
            }

            //db.Entry(productType).State = EntityState.Modified;

            if (check == null)
            {
                try
                {
                    var obj = db.ProductTypes.FirstOrDefault(m => m.ProductTypeId == productType.ProductTypeId); 
                    productType.ShowRoomId = obj.ShowRoomId;
                    productType.CreatedBy = obj.CreatedBy;
                    productType.DateCreated = obj.DateCreated;
                    productType.DateUpdated = DateTime.Now;
                    productType.Active = true;
                    db.ProductTypes.AddOrUpdate(productType);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductTypeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }         
            return Ok(msg);
            //return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ProductTypes
        [ResponseType(typeof(ProductType))]
        public async Task<IHttpActionResult> PostProductType(ProductType productType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            var msg = 0;
            var check = db.ProductTypes.FirstOrDefault(m => m.ProductTypeName == productType.ProductTypeName);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if(check == null)
            {
                try
                {
                    productType.ShowRoomId = showRoomId;
                    productType.CreatedBy = userName;
                    productType.DateCreated = DateTime.Now;
                    productType.DateCreated = productType.DateCreated;
                    productType.Active = true;
                    db.ProductTypes.Add(productType);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (Exception)
                {
                    throw;
                }
            }         

            return Ok(msg);
            //return CreatedAtRoute("DefaultApi", new { id = productType.ProductTypeId }, productType);
        }

        // DELETE: api/ProductTypes/5
        [ResponseType(typeof(ProductType))]
        public async Task<IHttpActionResult> DeleteProductType(int id)
        {
            ProductType productType = await db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            db.ProductTypes.Remove(productType);
            await db.SaveChangesAsync();

            return Ok(productType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductTypeExists(int id)
        {
            return db.ProductTypes.Count(e => e.ProductTypeId == id) > 0;
        }

        //public IHttpActionResult ChkDuplicateNo(string name)
        //{ 
        //    ProductType objNo = null;
        //    try
        //    {
        //        objNo = db.ProductTypes.FirstOrDefault(m => m.ProductTypeName == name);
        //    }
        //    catch (Exception e)
        //    {
        //        e.ToString();
        //    }
        //    return Ok(objNo);
        //}        
    }
}