using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.BookModule
{
    public class CostCenter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CostCenterId { get; set; }
        public string CostCenterName { get; set; }
        [Required(ErrorMessage = "Ledger Name is required.")]
        public int LedgerId { get; set; }
        [Required]
        public int ShowRoomId { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }
    }
}