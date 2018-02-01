using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Models.BankModule
{
    public class Check
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CheckId { get; set; }
        [Required]
        public int BankAccountId { get; set; }
        [Required]
        public int LedgerId { get; set; }

        public int VoucherId { get; set; }
        public int? VoucherDetailId { get; set; }

        public int CheckBookPageId { get; set; }

        [Required(ErrorMessage = "Check Number is required")]
        //[Remote("doesCheckNoExist", "Checks", HttpMethod = "POST", ErrorMessage = "Check No already exists. Please enter a different Check No.")]
        [StringLength(25)]
        public string CheckNumber { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, 999999999, ErrorMessage = "Amount must be between 0 and 999999999")]
        public double Amount { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }
        public DateTime CheckDate { get; set; }
        public DateTime HonourDate { get; set; }
        public string Remarks { get; set; }
        public string ApprovedBy { get; set; }


        public string CreatedBy { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual CheckBookPage CheckBookPage { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual Ledger Ledger { get; set; }
        public virtual Voucher Voucher { get; set; }
        //public virtual VoucherDetail VoucherDetail { get; set; }

    }
}