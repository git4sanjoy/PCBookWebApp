using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ViewModels
{
    public class UnitUserView
    {
        public int UnitUserId { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string FullName { get; set; }
    }
}