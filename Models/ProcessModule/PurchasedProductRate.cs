using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class PurchasedProductRate
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PurchasedProductRateId { get; set; }

        [Required]
        public int PurchasedProductId { get; set; }

        [Required]
        public int FinishedGoodStockId { get; set; }

        public double Quantity { get; set; }
        public double AvgRate { get; set; }

        [Required(ErrorMessage = "Show Room Name is required.")]
        public int ShowRoomId { get; set; }

        public virtual ShowRoom ShowRoom { get; set; }
        public virtual FinishedGoodStock FinishedGoodStock { get; set; }
        public virtual PurchasedProduct PurchasedProduct { get; set; }
    }
}