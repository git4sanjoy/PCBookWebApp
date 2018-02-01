using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BankModule
{
    public class CheckBook
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CheckBookId { get; set; }

        [Required]
        public int BankAccountId { get; set; }

        [Required]
        [StringLength(45)]
        public string CheckBookNo { get; set; }


        public string StartSuffices { get; set; }
        public double StartNo { get; set; }
        public double EndNo { get; set; }


        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual ICollection<CheckBookPage> CheckBookPages { get; set; }

    }
}