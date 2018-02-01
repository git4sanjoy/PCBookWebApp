using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class MemoDetailView
    {
        public int MemoDetailId { get; set; }
        public int MemoMasterId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductNameBangla { get; set; }
        public double Than { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }
        public string Image { get; set; }

        public int SubCategoryId { get; set; }
        public int MainCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string MainCategoryName { get; set; }
    }
}