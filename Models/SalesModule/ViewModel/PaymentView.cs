using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class PaymentView
    {
        public int PaymentId { get; set; }
        public int? MemoMasterId { get; set; }
        public int CustomerId { get; set; }
        public int ShowRoomId { get; set; }
        public DateTime PaymentDate { get; set; }
        public double SSAmount { get; set; }
        public double TSAmount { get; set; }
        public double SCAmount { get; set; }
        public double TCAmount { get; set; }
        public double SDiscount { get; set; }
        public double TDiscount { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string CustomerName { get; set; }
        public string PaymentType { get; set; }
        public string BankAccountNo { get; set; }
        public string CheckNo { get; set; }
        public DateTime HonourDate { get; set; }
        public string Remarks { get; set; }
    }
}