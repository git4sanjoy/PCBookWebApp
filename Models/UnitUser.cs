﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models
{
    public class UnitUser
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UnitUserId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Unit Name")]
        public int UnitId { get; set; }

        //Additional Attribute
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateUpdated { get; set; }

        public virtual Unit Unit { get; set; }
        //public virtual ApplicationUser ApplicationUser { get; set; }
    }
}