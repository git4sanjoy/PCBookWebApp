using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.SalesModule
{
    public class UnitManager
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UnitManagerId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Unit Name")]
        public int UnitId { get; set; }

        public string UnitManagerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public virtual Unit Unit { get; set; }
    }
}