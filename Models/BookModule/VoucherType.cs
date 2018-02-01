using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookModule
{
    public class VoucherType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VoucherTypeId { get; set; }

        [Display(Name = "Voucher Type Name")]
        [Required(ErrorMessage = "Voucher Type Name is required.")]
        [MaxLength(50), MinLength(2)]
        public string VoucherTypeName { get; set; }
        

        //Additional Attribute
        public string CreatedBy { get; set; }
        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public bool Active { get; set; }

    }
}