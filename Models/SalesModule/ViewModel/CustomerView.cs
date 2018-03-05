using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule.ViewModel
{
    public class CustomerView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int group { get; set; }
        public string groupName { get; set; }
        public int status { get; set; }
        public string statusName { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int ShowRoomId { get; set; }
        public string ShowRoomName { get; set; }
        public string ShowRoomNameBangla { get; set; }
        public string CustomerNameBangla { get; set; }
        public string AddressBangla { get; set; }


        public double CreditLimit { get; set; }
        public double BfAmount { get; set; }
        public double ActualCredit { get; set; }
        public double TotalSale { get; set; }
        public double TotalCollection { get; set; }
        public double TotalDiscount { get; set; }


        public string ShopName { get; set; }
        public string Image { get; set; } 
        public string DistrictName { get; set; }
        public DateTime BFDate { get; set; }
    }
}