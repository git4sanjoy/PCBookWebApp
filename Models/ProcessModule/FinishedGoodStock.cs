using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class FinishedGoodStock
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int FinishedGoodStockId { get; set; }        
        public int? FinishedGoodId { get; set; }        
        public int? ProcesseLocationId { get; set; }
        public DateTime ReceiveDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string OrderNumber { get; set; }
        public double OrderQuantity { get; set; }
        public double ReceiveQuantity { get; set; }
        public double DeliveryQuantity { get; set; }
        public string BuyerName { get; set; }

        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        [Required(ErrorMessage = "Show Room Name is required.")]
        public int ShowRoomId { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
        public virtual FinishedGood FinishedGood { get; set; }
        public virtual ProcesseLocation ProcesseLocation { get; set; }
    }
}