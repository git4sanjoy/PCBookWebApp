using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class CustomerBalanceView
    {
        public int CustomerId { get; set; }
        public double Limit { get; set; }
        public double Credit { get; set; }
        public double Adjustment { get; set; }
        public double Balance { get; set; }
        public double SaleCost { get; set; }
        public double GatOther { get; set; }
        public double MemoDiscount { get; set; }
        public double PaymentAmount { get; set; }
        public string CustomerName { get; set; }

        public string UpazilaName { get; set; }
        public string DistrictName { get; set; }
        public string SalesManName { get; set; }
        public string ShopName { get; set; }
        public DateTime MemoDate { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}