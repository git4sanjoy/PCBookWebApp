using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using PCBookWebApp.Models;
using PCBookWebApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace PCBookWebApp.Controllers
{
    [Authorize(Roles="Admin")]
    public class RolesController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();

        // GET: api/Roles
        [Route("api/Roles")]
        [HttpGet]
        public IHttpActionResult GetRoles()
        {
            //var users = context.Users
            //    .Where(x => x.Roles.Select(y => y.Id).Contains(roleId))
            //    .ToList();

            //var roleStore = new RoleStore<IdentityRole>(context);
            //var roleMngr = new RoleManager<IdentityRole>(roleStore);
            //var roles = roleMngr.Roles.ToList();
            ////var roles = context.Roles.ToList();

            //if (roles == null)
            //    return NotFound();

            //return Ok(roles);
            var list = context.Roles.Select(e => new { Id = e.Id, Name = e.Name });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);

        }

        [Route("api/Roles/GetRolesList")]
        [HttpGet]
        [ResponseType(typeof(RoleView))]
        public IHttpActionResult GetRolesList()
        {
            //var Roles = context.Roles.ToList();
            //return Ok(Roles);
            List<RoleView> Roles = new List<RoleView>();
            RoleView Role = new RoleView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT Id as id, Name as name FROM  dbo.AspNetRoles";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        string id = (string)reader["id"];
                        string name = (string)reader["name"];
                        Role = new RoleView();
                        Role.id = id;
                        Role.name = name;

                        Roles.Add(Role);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(Roles);
        }

        // POST: /Roles/Create
        [Route("api/Roles/PostRole")]
        [HttpPost]
        [ResponseType(typeof(RoleView))]
        public IHttpActionResult PostRole(RoleView role)
        {
            try
            {
                var context = new ApplicationDbContext();
                context.Roles.Add(new IdentityRole()
                {
                    Name = role.name
                });
                context.SaveChanges();
                return CreatedAtRoute("DefaultApi", new { controller = "Roles", id = role.id }, role);
            }
            catch
            {
                var error = new HttpError();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, error));
            }
        }
        // PUT: /Roles/Edit/5 **** Not working
        [HttpPut]
        [Route("api/Roles/PutRole/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRole(int id, IdentityRole role)
        {
            try
            {
                var context = new ApplicationDbContext();
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return Json(new { status = "Ok" });
            }
            catch
            {
                return Json(new { status = "Error" });

            }
        }
        // DELETE: /Roles/Delete/5
        [HttpDelete]
        [Route("api/Roles/DeleteRole/{RoleName}")]
        public IHttpActionResult DeleteRole(string RoleName)
        {
            try
            {
                var context = new ApplicationDbContext();
                var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                context.Roles.Remove(thisRole);
                context.SaveChanges();
                return Json(new { status = "ok" });

            }
            catch
            {
                var error = new HttpError();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, error));
            }
        }



        // POST: /UserRoles/Create
        [HttpPost]
        [Route("api/Roles/PostUserRole")]
        [ResponseType(typeof(Role))]
        public IHttpActionResult PostUserRole(Role role)
        {
            string UserName = role.UserName;
            string RoleName = role.RoleName;
            try
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var result1 = userManager.AddToRole(user.Id, RoleName);
                return CreatedAtRoute("DefaultApi", new { controller = "Roles", id = role.UserName }, role);
            }
            catch
            {
                var error = new HttpError();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, error));
            }
        }

        [Route("api/Roles/GetUserRoleList")]
        [HttpGet]
        [ResponseType(typeof(RoleView))]
        public IHttpActionResult GetUserRoleList()
        {
            List<UserRole> userRoles = new List<UserRole>();
            UserRole userRole = new UserRole();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.AspNetUserRoles.UserId, dbo.AspNetUsers.UserName, dbo.AspNetUserRoles.RoleId, dbo.AspNetRoles.Name
                                    FROM            
                                    dbo.AspNetUserRoles INNER JOIN
                                    dbo.AspNetUsers ON dbo.AspNetUserRoles.UserId = dbo.AspNetUsers.Id INNER JOIN
                                    dbo.AspNetRoles ON dbo.AspNetUserRoles.RoleId = dbo.AspNetRoles.Id";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        string userName = (string)reader["UserName"];
                        string roleName = (string)reader["Name"];
                        userRole = new UserRole();
                        userRole.UserName = userName;
                        userRole.RoleName = roleName;

                        userRoles.Add(userRole);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(userRoles);
        }

        [Route("api/Roles/GetUsersDropDownList")]
        [HttpGet]
        public IHttpActionResult GetDropDownListXedit()
        {
            var context = new ApplicationDbContext();
            var usersList = context.Users.Select(e => new { Id = e.Id, UserName = e.UserName });
            if (usersList == null)
            {
                return NotFound();
            }
            return Ok(usersList);
        }

        // DELETE: /Roles/Delete/5
        [HttpDelete]
        [Route("api/Roles/DeleteUserRole/{UserName}/{RoleName}")]
        public IHttpActionResult DeleteUserRole(string UserName, string RoleName)
        {
            try
            {
                List<string> roles;
                List<string> users;
                using (var context = new ApplicationDbContext())
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    roles = (from r in roleManager.Roles select r.Name).ToList();

                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);

                    users = (from u in userManager.Users select u.UserName).ToList();

                    var user = userManager.FindByName(UserName);
                    if (user == null)
                        throw new Exception("User not found!");

                    if (userManager.IsInRole(user.Id, RoleName))
                    {
                        userManager.RemoveFromRole(user.Id, RoleName);
                        context.SaveChanges();
                        return Json(new { status = "Role removed from this user successfully !" });
                    }
                    else
                    {
                        return Json(new { status = "This user doesn't belong to selected role." });
                    }

                }

            }
            catch
            {
                var error = new HttpError();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, error));
            }
        }


    }
}
