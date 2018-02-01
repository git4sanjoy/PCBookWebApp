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
using PCBookWebApp.Models.ViewModels;
using System.Configuration;
using System.Data.SqlClient;

namespace PCBookWebApp.Controllers
{
    public class SalesMenController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        [Route("api/SalesMen/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(SalesMan))]
        public IHttpActionResult GetDropDownList()
        {
            var list = db.SalesMen.Select(e => new { SalesManId = e.SalesManId, SalesManName = e.SalesManName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/SalesMen/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(SalesMan))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var list = db.SalesMen.Select(e => new { id = e.SalesManId, text = e.SalesManName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        [Route("api/SalesMen/SalesMenList")]
        [HttpGet]
        [ResponseType(typeof(SalesMan))]
        public IHttpActionResult SalesMenList()
        {
            var list = db.SalesMen.Select(e => new { value = e.SalesManId, text = e.SalesManName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }








        // GET: api/SalesMen
        public IQueryable<SalesMan> GetSalesMen()
        {
            return db.SalesMen;
        }

        // GET: api/SalesMen/5
        [ResponseType(typeof(SalesMan))]
        public async Task<IHttpActionResult> GetSalesMan(int id)
        {
            SalesMan salesMan = await db.SalesMen.FindAsync(id);
            if (salesMan == null)
            {
                return NotFound();
            }

            return Ok(salesMan);
        }

        // PUT: api/SalesMen/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSalesMan(int id, SalesMan salesMan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesMan.SalesManId)
            {
                return BadRequest();
            }

            db.Entry(salesMan).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesManExists(id))
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

        // POST: api/SalesMen
        [ResponseType(typeof(SalesMan))]
        public async Task<IHttpActionResult> PostSalesMan(SalesMan salesMan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SalesMen.Add(salesMan);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = salesMan.SalesManId }, salesMan);
        }

        // DELETE: api/SalesMen/5
        [ResponseType(typeof(SalesMan))]
        public async Task<IHttpActionResult> DeleteSalesMan(int id)
        {
            SalesMan salesMan = await db.SalesMen.FindAsync(id);
            if (salesMan == null)
            {
                return NotFound();
            }

            db.SalesMen.Remove(salesMan);
            await db.SaveChangesAsync();

            return Ok(salesMan);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalesManExists(int id)
        {
            return db.SalesMen.Count(e => e.SalesManId == id) > 0;
        }
    }
}