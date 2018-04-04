using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class FinishedGoodStockDetails
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int FinishedGoodStockDetailsId { get; set; }
        [Required]
        public int FinishedGoodId { get; set; }
        [Required]
        public int FinishedGoodStockId { get; set; }
        public double OrderQuantity { get; set; }       
        public virtual FinishedGood FinishedGood { get; set; }
        public virtual FinishedGoodStock FinishedGoodStock { get; set; }

    }
}