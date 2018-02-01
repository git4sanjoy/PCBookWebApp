using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class MemoView
    {       
        public DateTime MemoDate { get; set; }
        public int MemoMasterId { get; set; }
        public int CustomerId { get; set; }
        public int ShowRoomId { get; set; }

        public string CustomerName { get; set; }
        public string CustomerNameBangla { get; set; }
        public string ShowRoomName { get; set; }
        public string ShowRoomNameBangla { get; set; }
        public string MemoNo { get; set; }
        public string ExpencessRemarks { get; set; }
        public string Address { get; set; }
        public string AddressBangla { get; set; }

        public double MemoDiscount { get; set; }
        public double GatOther { get; set; }
        public double ActualMemoAmount { get; set; }
        public double NetMemoAmount { get; set; }
        public double MemoPaidAmount { get; set; }

        public List<MemoDetailView> MemoDetailViews { get; set; }
    }
}