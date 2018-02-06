using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class UnitRole
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UnitRoleId { get; set; }

        [StringLength(145)]
        [Required(ErrorMessage = "Unit Type Name is required.")]
        [MaxLength(50), MinLength(2)]
        public string UnitRoleName { get; set; }
       



        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        [Required(ErrorMessage = "Show Room Name is required.")]
        public int ShowRoomId { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
    }
}