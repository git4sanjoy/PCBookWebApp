using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using PCBookWebApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(PCBookWebApp.Startup))]

namespace PCBookWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndAdminUsers();
        }
        private void CreateRolesAndAdminUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User   
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool  
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                 
                var user = new ApplicationUser();
                user.UserName = "admin@pakizagroup.com";
                user.FullName = "Sanjoy Debnath";
                //user.LastName = "Collection";
                user.Email = "admin@pakizagroup.com";
                user.UserImage = "sanjoy.jpg";
                string userPWD = "P@kiza123";
                var chkUser = UserManager.Create(user, userPWD);
                //Add default User to Role Admin  
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }
            if (!roleManager.RoleExists("GM"))
            {

                // first we create Admin rool  
                var role = new IdentityRole();
                role.Name = "GM";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                 
                var user = new ApplicationUser();
                user.UserName = "fd@pakizagroup.com";
                user.FullName = "Shadek Sha Jahan";
                //user.LastName = "Collection";
                user.Email = "fd@pakizagroup.com";
                user.UserImage = "fd.jpg";
                string userPWD = "FD123456";
                var chkUser = UserManager.Create(user, userPWD);
                //Add default User to Role Admin  
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "GM");
                }
            }
            // creating Creating Manager role   
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("GM"))
            {
                var role = new IdentityRole();
                role.Name = "GM";
                roleManager.Create(role);
            }
            // creating Creating Employee role   
            if (!roleManager.RoleExists("Accounts"))
            {
                var role = new IdentityRole();
                role.Name = "Accounts";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Cashier"))
            {
                var role = new IdentityRole();
                role.Name = "Show Room Manager";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Purchase"))
            {
                var role = new IdentityRole();
                role.Name = "Purchase";
                roleManager.Create(role);
            }


            if (!roleManager.RoleExists("Show Room Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Show Room Manager";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Unit Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Unit Manager";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Zone Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Zone Manager";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Cash Sale"))
            {
                var role = new IdentityRole();
                role.Name = "Cash Sale";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Show Room Sales"))
            {
                var role = new IdentityRole();
                role.Name = "Show Room Sales";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Accounts Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Accounts Manager";
                roleManager.Create(role);
            }


            if (!roleManager.RoleExists("Process Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Process Manager";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Process Officer"))
            {
                var role = new IdentityRole();
                role.Name = "Process Officer";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Design Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Design Manager";
                roleManager.Create(role);
            }
        }
    }
}
