using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ProcessModule
{
    public class FinishedGoodImage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int FinishedGoodImageId { get; set; }
        public int FinishedGoodId { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        //public string Title { get; set; }
        //public string Description { get; set; }
        //public HttpPostedFileBase Attachment { get; set; }
        public virtual FinishedGood FinishedGood { get; set; }
    }
}