using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule
{
    public class Product
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        //[Required]
        public int? ShowRoomId { get; set; }
        [Required]
        public int SubCategoryId { get; set; }

        [Required]
        [StringLength(145)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(145)]
        public string ProductNameBangla { get; set; }

        public string Image { get; set; }
        public double MultiplyWith { get; set; }

        public double Rate { get; set; }
        public double Discount { get; set; }
        public int? UnitId { get; set; }

        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual SubCategory SubCategory { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
        public virtual Unit Unit { get; set; }
    }
}