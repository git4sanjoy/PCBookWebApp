using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class CustomerLedgerReportView
    {
        public int UnitId { get; set; }
        public int ShowRoomId { get; set; }
        public int CustomerId { get; set; }

        public DateTime Date { get; set; }

        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string DistrictName { get; set; }
        public string UpazilaName { get; set; }
        public string SaleZoneName { get; set; }
        public string ZoneManagerName { get; set; }
        public string DivisionName { get; set; }
        public string MemoNo { get; set; }

        public double Opening { get; set; }
        public double MemoCost { get; set; }
        public double MemoDiscount { get; set; }
        public double GatOther { get; set; }

        public double Sales { get; set; }
        public double Payments { get; set; }
        public double Discounts { get; set; }

    }
}