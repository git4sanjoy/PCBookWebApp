using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ViewModels
{
    public class ChartView
    {
        public string Label { get; set; }
        public double DataLineOne { get; set; }
        public double DataLineTwo { get; set; }
        public double Data { get; set; }
    }
}