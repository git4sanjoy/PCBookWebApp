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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PCBookWebApp.Controllers
{
    [Authorize]
    public class ProjectsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();




        //My Custome Methods
        [Route("api/Projects/AppDetails")]
        [HttpGet]
        public IHttpActionResult GetAppDetails()
        {
            string currentMonth = DateTime.Now.Month.ToString();
            string currentYear = DateTime.Now.Year.ToString();

            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var noOfShowRooms = db.ShowRooms.Count();
            var noOfVoucherTypes = db.VoucherTypes.Count();
            var noOfGroups = db.Groups.Count();

            var noOfLedgers = db.Ledgers.Where(a => a.ShowRoomId == showRoomId).Count();
            var noOfCustomers = db.Customers.Where(a => a.ShowRoomId == showRoomId).Count();
            var noOfProducts = db.Products.Where(a => a.ShowRoomId == showRoomId).Count();
            var lastSaleEntry = db.MemoMasters.Where(a => a.ShowRoomId == showRoomId).OrderByDescending(a => a.MemoDate).FirstOrDefault();
            var lastBookEntry = db.Vouchers.Where(a => a.ShowRoomId == showRoomId).OrderByDescending(a => a.VoucherDate).FirstOrDefault();


            return Json(new
            {
                //lastSaleEntryDate = (DateTime) lastSaleEntry.MemoDate,
                lastSaleEntryDate = DateTime.Now,
                noOfCustomers = noOfCustomers,
                noOfProducts = noOfProducts,
                noOfVoucherTypes = noOfVoucherTypes,
                noOfGroups = noOfGroups,
                noOfLedgers = noOfLedgers,
                lastBookEntryDate = DateTime.Now
            });
        }


        [Authorize]
        [Route("api/Projects/LogedInUserRole")]
        [HttpGet]
        public IHttpActionResult LogedInUserRole()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());


            var context = new ApplicationDbContext();
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(currentUser.UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            List<string> rolesUser = new List<string>();

            SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            Connection.Open();
            try
            {
                SqlDataReader productReader = null;
                string sql = @"SELECT AspNetUsers.UserName, AspNetRoles.Name 
                               FROM AspNetUsers 
                               LEFT JOIN AspNetUserRoles ON  AspNetUserRoles.UserId = AspNetUsers.Id 
                               LEFT JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId
                               WHERE AspNetUsers.Id = @Id";
                SqlCommand spCommand = new SqlCommand(sql, Connection);
                spCommand.Parameters.Add(new SqlParameter("@Id", user.Id));

                productReader = spCommand.ExecuteReader();
                while (productReader.Read())
                {
                    //roles = roles + (string) productReader["Name"];
                    string roleName = (string)productReader["Name"];
                    rolesUser.Add(roleName.ToString());
                }
                productReader.Close();
                Connection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //return Ok(currentUser);
            return Json(new { user = currentUser, role = rolesUser });
        }

        [Route("api/Projects/GetProjectsList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetProjectsList()
        {
            List<XEditGroupView> ImportProductList = new List<XEditGroupView>();
            XEditGroupView importProduct = new XEditGroupView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT ProjectId AS id, ProjectName AS name, Address, Email, Phone, Website FROM dbo.Projects";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        string name = (string)reader["name"];
                        importProduct = new XEditGroupView();
                        importProduct.id = id;
                        importProduct.name = name;

                        if (reader["Address"] != System.DBNull.Value)
                        {
                            importProduct.Address = reader["Address"].ToString();
                        }
                        if (reader["Email"] != System.DBNull.Value)
                        {
                            importProduct.Email = reader["Email"].ToString();
                        }
                        if (reader["Phone"] != System.DBNull.Value)
                        {
                            importProduct.Phone = reader["Phone"].ToString();
                        }
                        if (reader["Website"] != System.DBNull.Value)
                        {
                            importProduct.Website = reader["Website"].ToString();
                        }
                        ImportProductList.Add(importProduct);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(ImportProductList);
        }

        [Route("api/Projects/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(Project))]
        public IHttpActionResult GetDropDownListXedit()
        {
            var list = db.Projects.Select(e => new { id = e.ProjectId, text = e.ProjectName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // GET: api/Projects
        public IQueryable<Project> GetProjects()
        {
            return db.Projects;
        }

        // GET: api/Projects/5
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> GetProject(int id)
        {
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        // PUT: api/Projects/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProject(int id, Project project)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            project.CreatedBy = userName;
            project.DateCreated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            db.Entry(project).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> PostProject(Project project)
        {
            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;

            project.CreatedBy = userName;
            project.DateCreated = createdAt;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Projects.Add(project);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = project.ProjectId }, project);
        }

        // DELETE: api/Projects/5
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> DeleteProject(int id)
        {
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            db.Projects.Remove(project);
            await db.SaveChangesAsync();

            return Ok(project);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProjectExists(int id)
        {
            return db.Projects.Count(e => e.ProjectId == id) > 0;
        }
    }
}