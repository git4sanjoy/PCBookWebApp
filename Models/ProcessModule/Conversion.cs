﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class Conversion
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ConversionId { get; set; }
        [Required]
        [StringLength(100)]
        public string ConversionName { get; set; }
        public Nullable<int> PurchaseProductId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConversionDetail> ConversionDetails { get; set; }
        public virtual PurchasedProduct PurchaseProduct { get; set; }

        public int? MatricId1 { get; set; }
        public int? MatricId2 { get; set; }

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