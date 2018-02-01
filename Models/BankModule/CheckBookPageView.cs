using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BankModule
{
    public class CheckBookPageView
    {
        public int CheckBookId { get; set; }
        public double StartNo { get; set; }
        public double EndNo { get; set; }
        public string StartSuffices { get; set; }
    }
}