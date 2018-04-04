using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class SalesCollectionDiscountReportView
    {
        public int UnitId { get; set; }
        public int ShowRoomId { get; set; }
        public string ShowRoomName { get; set; }
        public string UnitName { get; set; }

        public DateTime Date { get; set; }
        public double CashSale { get; set; }
        public double CreditSale { get; set; }
        public double Other { get; set; }
        public double Collection { get; set; }
        public double Discount { get; set; }

        public string UpazilaName { get; set; }
        public string DistrictName { get; set; }
        public string SaleZoneName { get; set; }
        public string ZoneManagerName { get; set; }
        public string DivisionName { get; set; }
    }
}