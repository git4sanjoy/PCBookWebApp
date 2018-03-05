using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookModule
{
    public class Ledger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LedgerId { get; set; }

        [Display(Name = "Ledger Name")]
        [Required(ErrorMessage = "Ledger Name is required.")]
        [MaxLength(240), MinLength(2)]
        public string LedgerName { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int BookId { get; set; }
        [Required]
        public int TrialBalanceId { get; set; }

        [Required]
        public int ShowRoomId { get; set; }
        public bool TrialBalance { get; set; }
        public bool Provision { get; set; }
        public int? BankAccountId { get; set; }
        //Additional Attribute
        public string CreatedBy { get; set; }
        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public bool Active { get; set; }
        public virtual Group Group { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
    }
}