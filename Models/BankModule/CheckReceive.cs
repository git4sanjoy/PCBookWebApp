using PCBookWebApp.Models.BookModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BankModule
{
    public class CheckReceive
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheckReceiveId { get; set; }

        [Required]
        public int VoucherId { get; set; }
        public int? VoucherDetailId { get; set; }
        public string BankOrPartyName { get; set; }
        public double Amount { get; set; }
        public string CheckOrMoneyReceiptNo { get; set; }

        [Display(Name = "Honour Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> HonourDate { get; set; }

        public int ShowRoomId { get; set; }

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
        public virtual ShowRoom ShowRoom { get; set; }

    }
}