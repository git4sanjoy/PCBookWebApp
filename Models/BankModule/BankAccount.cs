using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using PCBookWebApp.Models.BookModule;

namespace PCBookWebApp.Models.BankModule
{
    public class BankAccount
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BankAccountId { get; set; }

        [Required]
        public int BankId { get; set; }
        [Required]
        public int? LedgerId { get; set; }
        [Required]
        public int? GroupId { get; set; }

        [Required]
        public int ShowRoomId { get; set; }

        [Required]
        [StringLength(240)]
        public string BankAccountNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AccountOpenDate { get; set; }

        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }


        public virtual Bank Bank { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
        public virtual Ledger Ledger { get; set; }
        public virtual Group Group { get; set; }

        public virtual ICollection<Check> Checks { get; set; }
        public virtual ICollection<CheckBook> CheckBooks { get; set; }
    }
}