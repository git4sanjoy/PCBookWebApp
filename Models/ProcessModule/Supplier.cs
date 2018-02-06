using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class Supplier
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SupplierId { get; set; }

        [StringLength(145)]
        [Required(ErrorMessage = "Supplier Name is required.")]
        [MaxLength(50), MinLength(2)]
        public string SupplierName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }



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