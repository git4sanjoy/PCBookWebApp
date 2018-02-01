using PCBookWebApp.Models.BookModule;
using PCBookWebApp.Models.SalesModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models
{
    public class ShowRoom
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ShowRoomId { get; set; }

        [Required]
        public int UnitId { get; set; }

        [Required]
        [StringLength(145)]
        public string ShowRoomName { get; set; }
        [Required]
        [StringLength(145)]
        public string ShowRoomNameBangla { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual Unit Unit { get; set; }
        public virtual ICollection<Ledger> Ledgers { get; set; }
        public virtual ICollection<MemoMaster> MemoMasters { get; set; }
    }
}