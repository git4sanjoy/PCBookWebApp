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
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.ViewModels;
using System.Data.SqlClient;
using System.Configuration;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class DistrictController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/District/GetDropDownListXedit/
        [Route("api/District/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var unitList = db.Districts.Select(e => new { id = e.DistrictId, text = e.DistrictName });
            if (unitList == null)
            {
                return NotFound();
            }
            return Ok(unitList);
        }

        // GET: api/District/GetDropDownList/
        [Route("api/District/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetDropDownList()
        {
            var listObj = db.Districts.Select(e => new { DistrictId = e.DistrictId, DistrictName = e.DistrictName, DistrictNameBangla = e.DistrictNameBangla });
            if (listObj == null)
            {
                return NotFound();
            }
            return Ok(listObj);
        }



        [Route("api/District/DistrictListForColumnEdit")]
        [HttpGet]
        public IHttpActionResult GetDistrictListForColumnEdit()
        {
            //string currentUserId = User.Identity.GetUserId();
            //string currentUserName = User.Identity.GetUserName();
            //var showRoomId = db.ShowRoomUsers
            //    .Where(a => a.Id == currentUserId)
            //    .Select(a => a.ShowRoomId)
            //    .FirstOrDefault();

            //var list = db.Districts
            //    .Select(e => new {
            //        id = e.DistrictId,
            //        name = e.DistrictName,
            //        status = e.SaleZone.SaleZoneId,
            //        e.DistrictNameBangla
            //    })
            //    .OrderBy(e => e.name);
            //return Ok(list);
            List<XEditGroupView> list = new List<XEditGroupView>();
            SqlConnection checkConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            checkConnection.Open();
            try
            {
                SqlDataReader ledgerReader = null;
                string sql = @"SELECT        
                                dbo.Districts.DistrictId AS id, dbo.Districts.DistrictName AS name, dbo.Districts.DistrictNameBangla, dbo.Districts.SaleZoneId AS status, dbo.SaleZones.SaleZoneName
                                FROM            
                                dbo.Districts LEFT OUTER JOIN
                                dbo.SaleZones ON dbo.Districts.SaleZoneId = dbo.SaleZones.SaleZoneId";
                SqlCommand command = new SqlCommand(sql, checkConnection);
                ledgerReader = command.ExecuteReader();
                while (ledgerReader.Read())
                {
                    XEditGroupView customerObj = new XEditGroupView();
                    customerObj.id = (int)ledgerReader["id"];
                    customerObj.name = (string)ledgerReader["name"];
                    if (ledgerReader["status"] != DBNull.Value) {
                        customerObj.status = (int)ledgerReader["status"];
                    }
                    
                    customerObj.DistrictNameBangla = (string)ledgerReader["DistrictNameBangla"];
                    list.Add(customerObj);
                }
                ledgerReader.Close();
                checkConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(list);
        }


        [Route("api/District/ZoneListXEdit")]
        [HttpGet]
        [ResponseType(typeof(ZoneManager))]
        public IHttpActionResult GetZoneManagersListXEdit()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var list = db.SaleZones
                .Select(e => new { value = e.SaleZoneId, text = e.SaleZoneName })
                .OrderBy(e => e.text);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }







        // GET: api/District
        public IQueryable<District> GetDistricts()
        {
            return db.Districts;
        }

        // GET: api/District/5
        [ResponseType(typeof(District))]
        public async Task<IHttpActionResult> GetDistrict(int id)
        {
            District district = await db.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            return Ok(district);
        }

        [Route("api/District/DistrictUpdate/{id}/{column}/{value}")]
        [HttpPost]
        //[ResponseType(typeof(District))]
        public IHttpActionResult RateMgrUpdateRate(int id, string column, string value)
        {
            string updateColumnName = "";
            if (column == "status") {
                updateColumnName = "SaleZoneId";
            } else if(column == "DistrictNameBangla") {
                updateColumnName = "DistrictNameBangla";
            } else {
                updateColumnName = "DistrictName";
            }
            if (value != "0") {
                string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
                SqlConnection sqlBUpdateCon = new SqlConnection(connectionString);
                SqlCommand cmdBUpdate = new SqlCommand();
                cmdBUpdate.CommandType = System.Data.CommandType.Text;
                cmdBUpdate.CommandText = "UPDATE dbo.Districts SET ["+ updateColumnName + "] = @updateValue WHERE [DistrictId] = @DistrictId";
                cmdBUpdate.Parameters.AddWithValue("@updateValue", value);
                cmdBUpdate.Parameters.AddWithValue("@DistrictId", id);
                cmdBUpdate.Connection = sqlBUpdateCon;

                sqlBUpdateCon.Open();
                cmdBUpdate.ExecuteNonQuery();
                sqlBUpdateCon.Close();
            }

            return Ok();
        }

        // PUT: api/District/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDistrict(int id, District district)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != district.DistrictId)
            {
                return BadRequest();
            }

            db.Entry(district).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistrictExists(id))
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

        // POST: api/District
        [ResponseType(typeof(District))]
        public async Task<IHttpActionResult> PostDistrict(District district)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Districts.Add(district);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = district.DistrictId }, district);
        }

        // DELETE: api/District/5
        [ResponseType(typeof(District))]
        public async Task<IHttpActionResult> DeleteDistrict(int id)
        {
            District district = await db.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            db.Districts.Remove(district);
            await db.SaveChangesAsync();

            return Ok(district);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DistrictExists(int id)
        {
            return db.Districts.Count(e => e.DistrictId == id) > 0;
        }
    }
}