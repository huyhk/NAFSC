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
using System.Data.Entity;

namespace Megatech.FMS.WebAPI.Controllers
{
    
    public class InvoicesController : ApiController
    {
        private DataContext db = new DataContext();
        [Authorize]    
        // POST: api/Invoices
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
            var refuel = db.RefuelItems.Include(r=>r.Flight).FirstOrDefault(r => r.Id == model.RefuelItemId);
            var invoice = JsonConvert.DeserializeObject<Invoice>(JsonConvert.SerializeObject(model));

            var invoiceItem = JsonConvert.DeserializeObject<InvoiceItem>(JsonConvert.SerializeObject(model.InvoiceItems[0]));
            invoice.AddItem(invoiceItem);
            invoice.FlightId = refuel.FlightId;
            invoice.CustomerId = refuel.Flight.AirlineId??0;
            invoice.UserCreatedId = invoice.UserUpdatedId = user.Id;
            if (invoice.ChildInvoice != null) {
                var child = invoice.ChildInvoice;
                invoice.ChildInvoice = null;
                child.ParentInvoice = invoice;
                db.Invoices.Add(child);
            }
            else
                db.Invoices.Add(invoice); 
            db.DisableFilter("IsNotDeleted");
            
            db.SaveChanges();

            if (refuel != null)
            {
                refuel.InvoiceId = invoice.Id;
                refuel.Printed = true;                
            }
            db.SaveChanges();
            return Ok(invoice);
        }

        [HttpPost]
        [Authorize]
        [Route("api/invoices/cancel")]
        [ResponseType(typeof(InvoiceViewModel))]
        public IHttpActionResult PostCancel(InvoiceViewModel model)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var userName = ClaimsPrincipal.Current.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == userName);

            var userId = user != null ? user.Id : 0;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var refuel = db.RefuelItems.Include(r => r.Flight).FirstOrDefault(r => r.InvoiceId == model.Id);
            var invoice = db.Invoices.FirstOrDefault(r => r.Id== model.Id);
            if (invoice != null)
            {
                invoice.UserDeletedId = user.Id;
                invoice.DateDeleted = DateTime.Now;
                invoice.IsDeleted = true;
            }
            if (refuel != null)
            {
                refuel.UserUpdatedId = user.Id;                
                refuel.InvoiceId = null;
                refuel.Printed = false;
            }
            db.DisableFilter("IsNotDeleted");


            
            db.SaveChanges();
            return Ok(invoice);
        }
    }
}
