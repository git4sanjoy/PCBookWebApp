using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule.ViewModels
{
    public class VmConversionDetails
    {
        public int ConversionDetailsId { get; set; }
        public string ConversionId { get; set; }
        public string ConversionName { get; set; }
        public double Quantity { get; set; }
        public string PurchaseProductId { get; set; }
        public string PurchasedProductName { get; set; }
    }
}