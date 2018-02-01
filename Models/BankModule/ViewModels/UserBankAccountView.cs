using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookViewModel.ViewModels
{
    public class UserBankAccountView
    {
        public UserBankAccountView()
        {
        }
        public int UserBankAccountID { get; set; }
        public int BankAccountID { get; set; }
        public string BankAccountNumber { get; set; }
        public string UserName { get; set; }

        public string UnitName { get; set; }
        public string BankName { get; set; }

    }
}