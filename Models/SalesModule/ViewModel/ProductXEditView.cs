using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class ProductXEditView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int group { get; set; }
        public string groupName { get; set; }
        public string ProductNameBangla { get; set; }

        public double MultiplyWith { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }

        public bool status { get; set; }

    }
}