using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ViewModels
{
    public class ShowRoomView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int group { get; set; }
        public string groupName { get; set; }

        public double MultiplyWith { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }

        public int status { get; set; }
        public string statusName { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int ShowRoomId { get; set; }

        public string CustomerNameBangla { get; set; }
        public string AddressBangla { get; set; }
        public double CreditLimit { get; set; }
        public string ProductNameBangla { get; set; }
        public double TotalCredit { get; set; }
        public string ShowRoomNameBangla { get; set; }
        public double ActualCredit { get; set; }
        public string ShopName { get; set; }
    }
}