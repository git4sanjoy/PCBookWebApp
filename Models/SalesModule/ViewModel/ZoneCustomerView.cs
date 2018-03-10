using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class ZoneCustomerView
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string UpazilaName { get; set; }
        public string DistrictName { get; set; }
        public string SaleZoneName { get; set; }
        public string DivisionName { get; set; }
        public string ZoneManagerName { get; set; }
        public string SalesManName { get; set; }
        public string UnitName { get; set; }
        public int UnitId { get; set; }
        public int UpazilaId { get; set; }
        public int DistrictId { get; set; }
        public int SalesManId { get; set; }
        public int SaleZoneId { get; set; }
        public int ZoneManagerId { get; set; }
        public int DivisionId { get; set; }

        

        public double TotalSale { get; set; }
        public double MemoDiscount { get; set; }
        public double GatOther { get; set; }
        public double ActualSale { get; set; }

        public double TotalCollection { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalBf { get; set; }
    }
}