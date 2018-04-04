using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule.ViewModels
{
    public class FactorywisePurchaseRptView
    {
        public int PurchaseId { get; set; }

        public DateTime PurchaseDate { get; set; }
        public string PChallanNo { get; set; }
        public double Rate { get; set; }
        public string ProcessListName { get; set; }
        public string ProcesseLocationName { get; set; }
        public string SupplierName { get; set; }
        public string PurchasedProductName { get; set; }
        public string ShowRoomName { get; set; }

        public double Quantity { get; set; }
        public double SE { get; set; }
        public double Amount { get; set; }
        public double Discount { get; set; }
        public double DeliveryQuantity { get; set; }

        public int ProcessListId { get; set; }
        public int ProcesseLocationId { get; set; }
        public int SupplierId { get; set; }
        public int PurchasedProductId { get; set; }
        public int ShowRoomId { get; set; }
    }
}