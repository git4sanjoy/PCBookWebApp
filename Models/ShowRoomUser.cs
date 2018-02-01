using PCBookWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models
{
    public class ShowRoomUser
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int  ShowRoomUserId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string Id { get; set; }
        
        [Required]
        [Display(Name = "Show Room Name")]
        public int ShowRoomId { get; set; }

        public string UserName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public virtual ShowRoom ShowRoom { get; set; }

    }
}