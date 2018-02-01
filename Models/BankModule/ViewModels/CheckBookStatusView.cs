using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBookWebApp.Models.BookViewModel.ViewModels
{
    class CheckBookStatusView
    {
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string CheckBookNo { get; set; }
        public string StartSuffices { get; set; }

        public double StartNo { get; set; }
        public double EndNo { get; set; }
        public int UsedCheck { get; set; }
        public int UnUsedCheck { get; set; }
    }
}
