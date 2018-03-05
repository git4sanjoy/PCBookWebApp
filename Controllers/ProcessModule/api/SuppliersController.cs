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
    public class SuppliersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        [Route("api/Suppliers/SuppliersMultiSelectList")]
        [HttpGet]
        public IHttpActionResult GetSuppliersMultiSelectList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.Suppliers
                            .Where(d => d.ShowRoomId == showRoomId)
                            .OrderBy(d => d.SupplierName)
                            .Select(e => new {
                                id = e.SupplierId,
                                label = e.SupplierName
                            });

            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }
        // GET: api/Suppliers
        public IQueryable<Supplier> GetSuppliers()
        {
            return db.Suppliers;
        }

        [Route("api/Suppliers/GetSupplierList")]
        [HttpGet]        
        public IHttpActionResult GetSupplierList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var list = (from item in db.Suppliers
                        where item.ShowRoomId == showRoomId
                        select new
                        {
                            item.SupplierId,
                            item.SupplierName,
                            item.Address,
                            item.Email,
                            item.Phone,
                            item.Active,
                            item.ShowRoomId,
                            item.ShowRoom.ShowRoomName,
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

        // GET: api/Suppliers/5
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> GetSupplier(int id)
        {
            Supplier supplier = await db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        // PUT: api/Suppliers/5        
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> PutSupplier(int id, Supplier supplier)
        {
            var msg = 0;
            var check = db.Suppliers.FirstOrDefault(m => m.SupplierName == supplier.SupplierName);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != supplier.SupplierId)
            {
                return BadRequest();
            }
            //db.Entry(supplier).State = EntityState.Modified;
            if (check == null)
            {
                try
                {
                    var obj = db.Suppliers.FirstOrDefault(m => m.SupplierId == supplier.SupplierId);
                    supplier.CreatedBy = obj.CreatedBy;
                    supplier.ShowRoomId = obj.ShowRoomId;
                    supplier.DateCreated = obj.DateCreated;
                    supplier.DateUpdated = DateTime.Now;
                    supplier.Active = true;
                    db.Suppliers.AddOrUpdate(supplier);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(id))
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

        // POST: api/Suppliers
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> PostSupplier(Supplier supplier)
        {
            var msg = 0;
            var check = db.Suppliers.FirstOrDefault(m => m.SupplierName == supplier.SupplierName);
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (check == null)
            {
                try
                {
                    supplier.ShowRoomId = showRoomId;
                    supplier.CreatedBy = userName;
                    supplier.DateCreated = DateTime.Now;
                    supplier.DateCreated = supplier.DateCreated;
                    supplier.Active = true;
                    db.Suppliers.Add(supplier);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (Exception)
                {
                    throw;
                }
            }
           
            return Ok(msg);
            //return CreatedAtRoute("DefaultApi", new { id = supplier.SupplierId }, supplier);
        }

        // DELETE: api/Suppliers/5        
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> DeleteSupplier(int id)
        {
            Supplier supplier = await db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            db.Suppliers.Remove(supplier);
            await db.SaveChangesAsync();

            return Ok(supplier);
        }       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierExists(int id)
        {
            return db.Suppliers.Count(e => e.SupplierId == id) > 0;
        }
    }
}