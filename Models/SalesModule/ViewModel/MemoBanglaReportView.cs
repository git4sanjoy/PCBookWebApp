using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class MemoBanglaReportView
    {
        public int MemoMasterId { get; set; }
        public string MemoNo { get; set; }
        public int CustomerId { get; set; }
        public string Address { get; set; }
        public string AddressBangla { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNameBangla { get; set; }
        public string UpazilaName { get; set; }
        public string UpazilaNameBangla { get; set; }
        public string DistrictName { get; set; }
        public string DistrictNameBangla { get; set; }
        public string SaleType { get; set; }
        public DateTime MemoDate { get; set; }
        public int ShowRoomId { get; set; }
        public double GatOther { get; set; }
        public double MemoDiscount { get; set; }
        public double MemoCost { get; set; }
        public double MemoPaidAmount { get; set; }
        public List<MemoItemReportView> MemoItems { get; set; }

        public int MemoDetailId { get; set; }
        public int ProductId { get; set; }
        public int SubCategoryId { get; set; }
        public int MainCategoryId { get; set; }

        public string ProductName { get; set; }
        public string ProductNameBangla { get; set; }
        public string SubCategoryName { get; set; }
        public string MainCategoryName { get; set; }

        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }
    }
}