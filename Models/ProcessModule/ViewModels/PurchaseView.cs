using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule.ViewModels
{
    public class PurchaseView
    {
        public int PurchaseId { get; set; }

        public DateTime PurchaseDate { get; set; }
        public string PChallanNo { get; set; }
        public double Quantity { get; set; }
        public double Amount { get; set; }
        public double? Discount { get; set; }
        public string SupplierName { get; set; }
        public string ShowRoomName { get; set; }

    }
}