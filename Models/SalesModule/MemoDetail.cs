using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule
{
    public class MemoDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MemoDetailId { get; set; }

        [Required]
        public int MemoMasterId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public double Than { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }
        

        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual MemoMaster MemoMaster { get; set; }
        public virtual Product Product { get; set; }
    }
}