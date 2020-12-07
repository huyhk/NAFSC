using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
namespace Megatech.FMS.WebAPI.Controllers
{
    public class UsersController : ApiController
    {
        private DataContext db = new DataContext();
        public IQueryable<UserViewModel> GetUsers()
        {
            string[] techRole = new string[] { "DRIVER", "OPERATOR" };
            var ctx = ApplicationDbContext.Create();
            var userRoles =  ctx.Roles
                .Where(r => techRole.Contains(r.Code))
                .Select(r => r.Users.FirstOrDefault())
                .ToList().Select(u=>u.UserId).ToArray();
            var users = ctx.Users.Where(u => userRoles.Contains(u.Id)).Select(u => u.UserId).ToArray();

            return db.Users.Where(u=>users.Contains(u.Id)).Select(u=>new UserViewModel { Id = u.Id, Name = u.FullName}).AsQueryable();
        }
    }
}
