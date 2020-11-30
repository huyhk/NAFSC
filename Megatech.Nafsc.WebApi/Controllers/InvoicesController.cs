using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using EntityFramework.DynamicFilters;

namespace Megatech.FMS.WebAPI.Controllers
{
    public class InvoicesController : ApiController
    {
        private DataContext db = new DataContext();
        [Authorize]
        // POST: api/Refuels
        [ResponseType(typeof(InvoiceViewModel))]
        public IHttpActionResult PostInvoice(InvoiceViewModel model)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var userName = ClaimsPrincipal.Current.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == userName);

            var userId = user != null ? user.Id : 0;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = JsonConvert.DeserializeObject<Invoice>(JsonConvert.SerializeObject(model));
            
            db.DisableFilter("IsNotDeleted");

            db.Invoices.Add(invoice);
            db.SaveChanges();
            
            return Ok(invoice);
        }
    }
}
