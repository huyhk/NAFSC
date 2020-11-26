using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Megatech.FMS.WebAPI.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static List<UserViewModel> CreateList()
        {
            var list = new List<UserViewModel>();
            list.Add(new UserViewModel { Id = 1, Name = "Admin" });
            list.Add(new UserViewModel { Id = 2, Name = "Moderator" });
            list.Add(new UserViewModel { Id = 3, Name = "Driver" });
            list.Add(new UserViewModel { Id = 4, Name = "Operator" });
            return list;
        }
    }


}