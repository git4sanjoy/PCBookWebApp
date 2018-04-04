using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class ClosingBalanceReportView
    {
        public int UnitId { get; set; }
        public int ShowRoomId { get; set; }
        public int CustomerId { get; set; }

        public string CustomerName  { get; set; }
        public string Address  { get; set; }
        public string Phone  { get; set; }
        public string DistrictName { get; set; }
        public string UpazilaName { get; set; }
        public string SaleZoneName { get; set; }
        public string ZoneManagerName { get; set; }
        public string DivisionName { get; set; }

        public double Opening { get; set; }
        public double MemoDiscount { get; set; }
        public double GatOther { get; set; }
        public double TotalPayments { get; set; }
        public double ActualSales { get; set; }
        public double TotalBf { get; set; }
        public double TotalDiscounts { get; set; }
        public double Balance { get; set; }
    }
}