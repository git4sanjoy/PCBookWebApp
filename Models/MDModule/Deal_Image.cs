using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.MDModule
{
    public class Deal_Image
    {
        public System.Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public System.Guid DealId { get; set; }

        public virtual Deal Deal { get; set; }
    }
}