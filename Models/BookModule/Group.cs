using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookModule
{
    public class Group
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }

        [Required]
        [StringLength(145)]
        public string GroupName { get; set; }
        public int? PrimaryId { get; set; }
        public int ParentId { get; set; }
        public bool IsParent { get; set; }
        public string GroupIdStr { get; set; }

        //[Required]
        //public int ShowRoomId { get; set; }
        public bool TrialBalance { get; set; }
        public bool Provision { get; set; }

        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public virtual Primary Primary { get; set; }

        public List<Group> Childrens { get; internal set; }
        //public virtual ShowRoom ShowRoom { get; set; }
    }
}