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
using System.Configuration;
using System.Data.SqlClient;
using PCBookWebApp.Models.ProcessModule.ViewModels;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;

namespace PCBookWebApp.Controllers.ProcessModule.api
{
    public class ProcessListsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: api/ProcessLists
        public IQueryable<ProcessList> GetProcessLists()
        {
            return db.ProcessLists;
        }

        // GET: api/ProcessLists/5
        [ResponseType(typeof(ProcessList))]
        public IHttpActionResult GetProcessList(int id)
        {
            ProcessList processList = db.ProcessLists.Find(id);
            if (processList == null)
            {
                return NotFound();
            }

            return Ok(processList);
        }

        // PUT: api/ProcessLists/5
        [ResponseType(typeof(void))]
       
       public async Task<IHttpActionResult> PutProcessList(int id, ProcessList processList)
        {
            var msg = 0;
            var check = db.ProcessLists.FirstOrDefault(m => m.ProcessListName == processList.ProcessListName);
            //GetProcessList();
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != processList.ProcessListId)
            {
                return BadRequest();
            }

            // db.Entry(processList).State = EntityState.Modified;

            if (check == null)
            {
                try
                {
                    var obj = db.ProcessLists.FirstOrDefault(m => m.ProcessListId == processList.ProcessListId);
                    processList.CreatedBy = obj.CreatedBy;
                    processList.DateCreated = obj.DateCreated;
                    processList.DateUpdated = DateTime.Now;
                    processList.ShowRoomId = obj.ShowRoomId;
                    processList.Active = true;
                    db.ProcessLists.AddOrUpdate(processList);
                    await db.SaveChangesAsync();
                    msg = 1;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcessListExists(id))
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

        [Route("api/ProcessLists/GetProcessList")]
        [HttpGet]
        [ResponseType(typeof(VmProcessList))]
        public IHttpActionResult GetProcessList()
        {
          
            //var list = db.ShowRooms.Select(e => new { ShowRoomId = e.ShowRoomId, ShowRoomName = e.ShowRoomName });
            //if (list == null)
            //{
            //    return NotFound();
            //}
            //return Ok(list);
            List<VmProcessList> ProcessList = new List<VmProcessList>();
            VmProcessList ProcessListObj = new VmProcessList();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"select p.ProcessListId,p.ProcessListName,p.UnitRoleId,r.UnitRoleName from ProcessLists p
                                          join UnitRoles r on r.UnitRoleId = p.UnitRoleId";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                //command.Parameters.Add(new SqlParameter("@userId", userId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                       
                        ProcessListObj = new VmProcessList();
                        ProcessListObj.ProcessListId = (int)reader["ProcessListId"];
                        ProcessListObj.ProcessListName = (string)reader["ProcessListName"];
                        ProcessListObj.UnitRoleId = reader["UnitRoleId"].ToString();
                        ProcessListObj.UnitRoleName = (string)reader["UnitRoleName"];
                        ProcessList.Add(ProcessListObj);
                        
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(ProcessList);
        }

        // POST: api/ProcessLists
        [ResponseType(typeof(ProcessList))]
        //public IHttpActionResult PostProcessList(ProcessList processList)
       public async Task<IHttpActionResult> PostProcessList(ProcessList processList)
        {
            //GetProcessList();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            string userName = User.Identity.GetUserName();
            bool isTrue = db.ProcessLists.Any(s => s.ProcessListName == processList.ProcessListName.Trim());
            if (isTrue == false)
            {
                processList.ShowRoomId = showRoomId;
                processList.CreatedBy = userName;
                processList.DateCreated = DateTime.Now;
                processList.DateCreated = processList.DateCreated;
                processList.Active = true;
                db.ProcessLists.Add(processList);
                await db.SaveChangesAsync();
            }
            return CreatedAtRoute("DefaultApi", new { id = processList.ProcessListId }, processList);
        }

        // DELETE: api/ProcessLists/5
        [ResponseType(typeof(ProcessList))]
        public IHttpActionResult DeleteProcessList(int id)
        {
          
            ProcessList processList = db.ProcessLists.Find(id);
            if (processList == null)
            {
                return NotFound();
            }

            db.ProcessLists.Remove(processList);
            db.SaveChanges();

            return Ok(processList);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProcessListExists(int id)
        {
            return db.ProcessLists.Count(e => e.ProcessListId == id) > 0;
        }
    }
}