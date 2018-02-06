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
using Microsoft.AspNet.Identity;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    public class UnitRolesController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/UnitRoles
        public IQueryable<UnitRole> GetUnitRoles()
        {
            return db.UnitRoles;
        }

        // GET: api/UnitRoles/5
        [ResponseType(typeof(UnitRole))]
        public IHttpActionResult GetUnitRole(int id)
        {
            UnitRole unitRole = db.UnitRoles.Find(id);
            if (unitRole == null)
            {
                return NotFound();
            }

            return Ok(unitRole);
        }

        // PUT: api/UnitRoles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUnitRole(int id, UnitRole unitRole)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            unitRole.ShowRoomId = showRoomId;

            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            unitRole.CreatedBy = userName;
            unitRole.DateCreated = createdAt;
            unitRole.DateUpdated = createdAt;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != unitRole.UnitRoleId)
            {
                return BadRequest();
            }

            db.Entry(unitRole).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitRoleExists(id))
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

        // POST: api/UnitRoles
        [ResponseType(typeof(UnitRole))]
        public IHttpActionResult PostUnitRole(UnitRole unitRole)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            unitRole.ShowRoomId = showRoomId;

            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            unitRole.CreatedBy = userName;
            unitRole.DateCreated = createdAt;
            unitRole.DateUpdated = createdAt;

            unitRole.DateCreated = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool isTrue = db.UnitRoles.Any(s => s.UnitRoleName == unitRole.UnitRoleName.Trim());
            if (isTrue == false)
            { 
            db.UnitRoles.Add(unitRole);
            db.SaveChanges();
            }
            return CreatedAtRoute("DefaultApi", new { id = unitRole.UnitRoleId }, unitRole);
        }

        // DELETE: api/UnitRoles/5
        [ResponseType(typeof(UnitRole))]
        public IHttpActionResult DeleteUnitRole(int id)
        {
            UnitRole unitRole = db.UnitRoles.Find(id);
            if (unitRole == null)
            {
                return NotFound();
            }

            db.UnitRoles.Remove(unitRole);
            db.SaveChanges();

            return Ok(unitRole);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UnitRoleExists(int id)
        {
            return db.UnitRoles.Count(e => e.UnitRoleId == id) > 0;
        }
    }
}