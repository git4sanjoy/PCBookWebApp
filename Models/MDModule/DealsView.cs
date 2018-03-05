using PCBookWebApp.Models.MDModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Controllers.MDModule
{
    public class DealsView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double TotalProduction { get; set; }
        public DateTime LastProduction { get; set; }
        public List<Deal_Image> Deal_Images { get; set; }
        public List<DealProduction> DealProductions { get; set; }
    }
}