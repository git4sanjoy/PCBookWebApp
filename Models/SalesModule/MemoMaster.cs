using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule
{
    public class MemoMaster
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MemoMasterId { get; set; }

        [Required]
        [Display(Name = "Memo Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime MemoDate { get; set; }

        public int CustomerId {get;set; }

        [Required]
        public int ShowRoomId { get; set; }

        [Required]
        public string MemoNo { get; set; }
        public double? MemoDiscount { get; set; }
        public double? GatOther { get; set; }
        public string ExpencessRemarks { get; set; }
        public double? MemoCost { get; set; }

        //Additional Property
        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }

        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<MemoDetail> MemoDetails { get; set; }
    }
}