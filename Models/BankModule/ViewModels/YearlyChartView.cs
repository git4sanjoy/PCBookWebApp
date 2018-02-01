using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookViewModel.ViewModels
{
    public class YearlyChartView
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Label { get; set; }
        public double DataLineOne { get; set; }
        public double DataLineTwo { get; set; }
        public int CheckCount { get; set; }
        public string CreatedBy { get; set; }
    }
}