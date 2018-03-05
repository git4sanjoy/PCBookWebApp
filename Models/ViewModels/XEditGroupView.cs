using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ViewModels
{
    public class XEditGroupView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int group { get; set; }
        public string groupName { get; set; }
        public int status { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string UnderGroup { get; set; }
        public string PrimaryName { get; set; }
        public string GeneralLedgerName { get; set; }
        public string DistrictNameBangla { get; set; }

        public bool TrialBalance { get;set;}
        public bool Provision { get; set; }

        public double OpeningAmount { get; set; }
        public double ProvisionAmount { get; set; }
        public double ActualAmount { get; set; }
        public double ClosingAmount { get; set; }
    }
}