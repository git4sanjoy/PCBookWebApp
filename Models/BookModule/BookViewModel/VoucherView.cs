using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookModule.BookViewModel
{
    public class VoucherView
    {
        public string ShowRoomName { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherNo { get; set; }
        public string Naration { get; set; }
        public string VoucherTypeName { get; set; }
        public string TransctionTypeName { get; set; }
        public string LedgerName { get; set; }
        public  double DrAmount { get; set; }
        public double CrAmount { get; set; }
        public bool IsBank { get; set; }
        public DateTime HonoredDate { get; set; }
        public bool ReceiveOrPayment { get; set; }
        public int CheckId { get; set; }
        public int VoucherDetailId { get; set; }
        public int VoucherId { get; set; }
    }
}