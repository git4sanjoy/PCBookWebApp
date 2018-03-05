using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class UnitManagerView
    {
        public int UnitManagerId { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string ManagerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }

    }
}