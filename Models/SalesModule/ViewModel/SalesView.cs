using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class SalesView
    {
        public string SalesManName { get; set; }
        public double TotalSaleTaka { get; set; }
        public double TotalCollectionTaka { get; set; }
        public string DistrictName { get; set; }
        public string UpazilaName { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string ShowRoomName { get; set; }
    }
}