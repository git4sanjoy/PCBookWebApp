using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule.ViewModels
{
    public class ProductWiseStoreBalanceRptView
    {
        
        public string ProcessListName { get; set; }
        public string ProcesseLocationName { get; set; }
        
        public string PurchasedProductName { get; set; }
        public string ShowRoomName { get; set; }
        public double DeliveryQuantity { get; set; }
        public double PurchaseQuantity { get; set; }
       
    }
}