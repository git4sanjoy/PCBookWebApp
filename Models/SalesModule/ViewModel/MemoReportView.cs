using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class MemoReportView
    {
        public int MemoMasterId { get; set; }
        public string MemoNo { get; set; }
        public DateTime MemoDate { get; set; }

        public double MemoCost { get; set; }
        public double MemoDiscount { get; set; }
        public double GatOther { get; set; }
        
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
    }
}