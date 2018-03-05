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
    public class VoucherDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int VoucherDetailId { get; set; }

        [Required]
        public int VoucherId { get; set; }

        [Required]
        public int TrialBalanceId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public int LedgerId { get; set; }

        [Required]
        public int TransctionTypeId { get; set; }

        public double DrAmount { get; set; }
        public double CrAmount { get; set; }

        public bool ReceiveOrPayment { get; set; }

        public int? CheckId { get; set; }
        public int? BankAccountId { get; set; }
        public int? CostCenterId { get; set; }

        //Additional Attribute
        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }

        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual Voucher Voucher { get; set; }
        public virtual Ledger Ledger { get; set; }
        public virtual TransctionType TransctionType { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
        public virtual ICollection<CheckReceive> CheckReceives { get; set; }
    }
}