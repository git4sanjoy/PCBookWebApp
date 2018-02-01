using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookViewModel.ViewModels
{
    public class CheckSummaryView
    {
        public string BankAccountNumber { get; set; }
        public DateTime HonourDate { get; set; }
        public double Amount { get; set; }
    }
}