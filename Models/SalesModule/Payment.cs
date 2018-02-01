using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule
{
    public class Payment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        public int? MemoMasterId  {get;set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ShowRoomId { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]        
        public DateTime PaymentDate { get; set; }

        public double SSAmount { get; set; }
        public double TSAmount { get; set; }

        public double SCAmount { get; set; }
        public double TCAmount{ get; set; }

        public double SDiscount { get; set; }
        public double TDiscount { get; set; }

        public string PaymentType { get; set; }
        public Nullable<System.DateTime> HonourDate { get; set; }
        public string CheckNo { get; set; }
        public string BankAccountNo { get; set; }
        public string Remarks { get; set; }

        public bool Active { get; set; }
        public bool AdjustmentBf { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        //public virtual MemoMaster MemoMaster { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
    }
}