using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBookWebApp.Models.BookViewModel.ViewModels
{
    public class CheckView
    {
        public int CheckId { get; set; }
        public int UnitId { get; set; }
        public int BankAccountId { get; set; }
        public int PartyId { get; set; }

        public string CheckNo { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string UnitName { get; set; }
        public string PartyName { get; set; }
        public string Remarks { get; set; }

        public double Amount { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime CheckDate { get; set; }
        public DateTime HonourDate { get; set; }

        public string ApprovedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CheckBookNo { get; set; }


    }
}
