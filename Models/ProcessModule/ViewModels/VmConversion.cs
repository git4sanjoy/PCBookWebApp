using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule.ViewModels
{
    public class VmConversion
    {
        public int ConversionId { get; set; }


        public string PurchasedProductId { get; set; }


        public string ConversionName { get; set; }

        public string PurchasedProductName { get; set; }
    }
}