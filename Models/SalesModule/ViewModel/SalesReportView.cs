using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class SalesReportView
    {
        public int ProductId { get; set; }
        public int SubCategoryId { get; set; }
        public string ProductName { get; set; }
        public string SubCategoryName { get; set; }

        public double Quantity { get; set; }
        public double PrintQu { get; set; }
        public double PoplinQu { get; set; }
        public double VoilQu { get; set; }
        public double PrintPoplinVoilQu { get; set; }
        public double ThreePicQu { get; set; }
        public double ShareeQu { get; set; }
        public double BedSheetQu { get; set; }
        public double PordaQu { get; set; }
        public double BedQu { get; set; }
        public double BedCoverQu { get; set; }
        public double LungeeaQu { get; set; }
        public double OrnaQu { get; set; }

        public string UpazilaName { get; set; }
        public string DistrictName { get; set; }
        public string SaleZoneName { get; set; }
        public string ZoneManagerName { get; set; }
        public string DivisionName { get; set; }



    }
}