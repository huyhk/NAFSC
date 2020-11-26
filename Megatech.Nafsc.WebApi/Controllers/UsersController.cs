using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Megatech.FMS.WebAPI.Controllers
{
    public class UsersController : ApiController
    {
        private DataContext db = new DataContext();
        public IQueryable<UserViewModel> GetUsers()
        {
            return db.Users.Select(u=>new UserViewModel { Id = u.Id, Name = u.FullName}).AsQueryable();
        }
    }
}
