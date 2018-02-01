using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookViewModel
{
    public class LedgerView
    {
        public int LedgerId { get; set; }       
        public string LedgerName { get; set; }
        public DateTime Date { get; set; }
        public double DrAmount { get; set; }
        public double CrAmount { get; set; }
        public string VoucherNo { get; set; }
        public string Narration { get; set; }
        public int ShowRoomId { get; set; }
        public string ShowRoomName { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
    }
}