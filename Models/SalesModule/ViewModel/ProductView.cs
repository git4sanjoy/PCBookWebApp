using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class ProductView
    {

        public int ProductId { get; set; }
        public int ShowRoomId { get; set; }
        public int SubCategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductNameBangla { get; set; }
        public string Image { get; set; }
        public double MultiplyWith { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int MainCategoryId { get; set; }
        public string MainCategoryName { get; set; }
        public string SubCategoryName { get; set; }

    }
}