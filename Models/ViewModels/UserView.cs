using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ViewModels
{
    public class UserView
    {
        public string id { get; set; }
        public string text { get; set; }
        public int UnitId { get; set; }
        public string FullName { get; set; }
        public string UserImage { get; set; }
        public string UserSignature { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}