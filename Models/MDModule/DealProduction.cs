using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.MDModule
{
    public class DealProduction
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DealProductionId { get; set; }
        public System.Guid DealId { get; set; }
        public string FactoryName { get; set; }
        public double Quantity { get; set; }
        public DateTime DealProductionDate { get; set; }
        public string ReProduction { get; set; }


        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }

        public virtual Deal Deal { get; set; }
    }
}