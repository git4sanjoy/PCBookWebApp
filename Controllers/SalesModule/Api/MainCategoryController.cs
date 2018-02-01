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
using PCBookWebApp.Models.SalesModule;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class MainCategoryController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/MainCategory/GetDropDownList/
        [Route("api/MainCategory/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(MainCategory))]
        public IHttpActionResult GetDropDownList()
        {
            var unitList = db.MainCategories.Select(e => new { MainCategoryId = e.MainCategoryId, MainCategoryName = e.MainCategoryName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }
        // GET: api/MainCategory/GetDropDownListXedit/
        [Route("api/MainCategory/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(MainCategory))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var unitList = db.MainCategories.Select(e => new { id = e.MainCategoryId, text = e.MainCategoryName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }

        // GET: api/MainCategory
        public IQueryable<MainCategory> GetMainCategories()
        {
            return db.MainCategories;
        }

        // GET: api/MainCategory/5
        [ResponseType(typeof(MainCategory))]
        public async Task<IHttpActionResult> GetMainCategory(int id)
        {
            MainCategory mainCategory = await db.MainCategories.FindAsync(id);
            if (mainCategory == null)
            {
                return NotFound();
            }

            return Ok(mainCategory);
        }

        // PUT: api/MainCategory/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMainCategory(int id, MainCategory mainCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mainCategory.MainCategoryId)
            {
                return BadRequest();
            }

            db.Entry(mainCategory).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MainCategoryExists(id))
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

        // POST: api/MainCategory
        [ResponseType(typeof(MainCategory))]
        public async Task<IHttpActionResult> PostMainCategory(MainCategory mainCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MainCategories.Add(mainCategory);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = mainCategory.MainCategoryId }, mainCategory);
        }

        // DELETE: api/MainCategory/5
        [ResponseType(typeof(MainCategory))]
        public async Task<IHttpActionResult> DeleteMainCategory(int id)
        {
            MainCategory mainCategory = await db.MainCategories.FindAsync(id);
            if (mainCategory == null)
            {
                return NotFound();
            }

            db.MainCategories.Remove(mainCategory);
            await db.SaveChangesAsync();

            return Ok(mainCategory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MainCategoryExists(int id)
        {
            return db.MainCategories.Count(e => e.MainCategoryId == id) > 0;
        }
    }
}