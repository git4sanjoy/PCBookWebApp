using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class Purchase
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PurchaseId { get; set; }
        [Display(Name = "Purchase Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }
        public string PChallanNo { get; set; }
        public double Quantity { get; set; }
        public double? SE { get; set; }
        public string OrderNo { get; set; }
        public double Amount { get; set; }
        public double? Discount { get; set; }
        public int? ProcesseLocationId { get; set; }
        public double DeliveryQuantity { get; set; }
        public int? ProcessListId { get; set; }
        //[Required(ErrorMessage = "Supplier Name is required.")]
        public int? SupplierId { get; set; }
        [Required(ErrorMessage = "Product Name is required.")]
        public int PurchasedProductId { get; set; }
        [Required(ErrorMessage = "Show Room Name is required.")]
        public int ShowRoomId { get; set; }

        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual PurchasedProduct PurchasedProduct { get; set; }
        public virtual ProcessList ProcessList { get; set; }

        public virtual ProcesseLocation ProcesseLocation { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }

    }
}