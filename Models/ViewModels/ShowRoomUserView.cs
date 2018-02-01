using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCBookWebApp.Models.ViewModels
{
    public class ShowRoomUserView
    {
        public int ShowRoomUserId { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int ShowRoomId { get; set; }
        public string ShowRoomName { get; set; }
        public string OfficerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }

    }
}