using PCBookWebApp.Models.BankModule;
using PCBookWebApp.Models.BookViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookModule
{
    public class Voucher
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]       
        public int VoucherId { get; set; }

        [Required]
        [Display(Name = "Voucher Type")]
        public int VoucherTypeId { get; set; }

        [Required]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Voucher Date")]
        public DateTime VoucherDate { get; set; }

        [Required]
        [Display(Name = "Voucher No")]
        public string VoucherNo { get; set; }

        [StringLength(245)]
        public string Naration { get; set; }
        [Required]
        public int ShowRoomId { get; set; }

        public bool IsBank { get; set; }
        public bool IsHonored { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> HonoredDate { get; set; }

        //Additional Attribute
        public bool Authorized { get; set; }
        public string AuthorizedBy { get; set; }

        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }

        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public bool Active { get; set; }

        public virtual VoucherType VoucherType { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }

        public virtual ICollection<VoucherDetail> VoucherDetails { get; set; }
        public virtual ICollection<CheckReceive> CheckReceives { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
    }
}