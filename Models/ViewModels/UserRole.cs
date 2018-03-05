using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ViewModels
{
    public class UserRole
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }

        public string ShowRoomName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Id { get; set; }

    }
}