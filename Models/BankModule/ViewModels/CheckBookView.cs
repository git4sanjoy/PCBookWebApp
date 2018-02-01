using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookViewModel.ViewModels
{
    public class CheckBookView
    {
        public int CheckBookId { get; set; }
        public int BankAccountId { get; set; }
        public string BankAccountNumber { get; set; }
        public string CheckBookNo { get; set; }
        public string StartSuffices { get; set; }
        public double StartNo { get; set; }
        public double EndNo { get; set; }
    }
}