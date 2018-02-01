using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class UpazilaView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int group { get; set; }
        public string groupName { get; set; }

        public int status { get; set; }
        public string statusName { get; set; }

        public string UpazilaNameBangla { get; set; }

    }
}