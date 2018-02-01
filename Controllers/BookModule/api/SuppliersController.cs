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
using PCBookWebApp.Models;
using Microsoft.AspNet.Identity;

namespace PCBookWebApp.Controllers
{
    [Authorize]
    public class SuppliersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/Suppliers/SuppliersMultiSelectList")]
        [HttpGet]
        public IHttpActionResult SuppliersMultiSelectList()
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var list = db.Suppliers
                .Where(p => p.CreatedBy == userName)
                .Select(e => new { id = e.SupplierId, label = e.SupplierName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }
        [Route("api/Suppliers/GetSupplierDdList")]
        [HttpGet]
        public IHttpActionResult GetSupplierDdList()
        {
            string userName = User.Identity.GetUserName();
            var list = db.Suppliers
                .Where(a => a.CreatedBy == userName)
                .Select(e => new { SupplierId = e.SupplierId, SupplierName = e.SupplierName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/Suppliers/GetSupplierList")]
        [HttpGet]
        public IHttpActionResult GetSupplierList()
        {
            string userName = User.Identity.GetUserName();
            var list = db.Suppliers
                .Where(a => a.CreatedBy == userName)
                .Select(e => new { id = e.SupplierId, name = e.SupplierName, SupplierAddress = e.SupplierAddress, SupplierPhone = e.SupplierPhone, SupplierEmail = e.SupplierEmail });
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
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSupplier(int id, Supplier supplier)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            supplier.CreatedBy = userName;
            supplier.DateCreated = createdAt;
            supplier.DateUpdated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplier.SupplierId)
            {
                return BadRequest();
            }

            db.Entry(supplier).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Suppliers
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> PostSupplier(Supplier supplier)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            supplier.CreatedBy = userName;
            supplier.DateCreated = createdAt;
            supplier.DateUpdated = createdAt;
            if (db.Suppliers.Any(m => m.SupplierName == supplier.SupplierName && m.CreatedBy == userName))
            {
                ModelState.AddModelError("SupplierName", "Supplier Name Already Exists!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = supplier.SupplierId }, supplier);
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