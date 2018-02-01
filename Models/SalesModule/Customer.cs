using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule
{
    public class Customer
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }
        public int SalesManId { get; set; }
        public int UpazilaId { get; set; }
        [Required]
        public int ShowRoomId { get; set; }
        [Required]
        [StringLength(145)]
        public string CustomerName { get; set; }
        [Required]
        [StringLength(145)]
        public string CustomerNameBangla { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public string AddressBangla { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public double CreditLimit { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual Upazila Upazila { get; set; }
        public virtual SalesMan SalesMan { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
    }
}