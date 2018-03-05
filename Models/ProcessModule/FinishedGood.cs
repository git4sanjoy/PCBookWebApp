using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class FinishedGood
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int FinishedGoodId { get; set; }

        [StringLength(150)]
        [Required(ErrorMessage = "Finished Goods Name is required.")]
        [MaxLength(150), MinLength(2)]
        public string FinishedGoodName { get; set; }

        public string DesignNo { get; set; }
        public int ProductTypeId { get; set; }
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
        public virtual ProductType ProductType { get; set; }
        public virtual ICollection<FinishedGoodImage> FinishedGoodImages { get; set; }
        public virtual ICollection<FinishedGoodStock> FinishedGoodStocks { get; set; }

    }
}