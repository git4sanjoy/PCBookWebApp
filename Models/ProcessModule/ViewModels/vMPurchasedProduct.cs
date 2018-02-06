using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule.ViewModels
{
    public class vMPurchasedProduct
    {
        public int PurchasedProductId { get; set; }


        public string ProductTypeId { get; set; }


        public string PurchasedProductName { get; set; }

        public string ProductTypeName { get; set; }
    }
}