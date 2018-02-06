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
        public IHttpActionResult PutProcessList(int id, ProcessList processList)
        {
            processList.DateCreated = DateTime.Now;
            processList.DateUpdated = DateTime.Now;
            //GetProcessList();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != processList.ProcessListId)
            {
                return BadRequest();
            }

            db.Entry(processList).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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

            return StatusCode(HttpStatusCode.NoContent);
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
                                         left join UnitRoles r on r.UnitRoleId = p.UnitRoleId";

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
        public IHttpActionResult PostProcessList(ProcessList processList)
        {
            //GetProcessList();
            processList.DateCreated = DateTime.Now;
            try
            {
                if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
                db.ProcessLists.Add(processList);
                db.SaveChanges();
              
            }
            catch(Exception ex)
            {
                ex.ToString();
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