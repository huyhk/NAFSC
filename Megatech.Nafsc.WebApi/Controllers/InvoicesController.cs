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
    [RoutePrefix("api/invoices")]
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

            //var invoiceItem = JsonConvert.DeserializeObject<InvoiceItem>(JsonConvert.SerializeObject(model.Items[0]));
            //invoice.AddItem(invoiceItem);
            invoice.FlightId = refuel.FlightId;
            invoice.CustomerId = refuel.Flight.AirlineId??0;
            invoice.UserCreatedId = invoice.UserUpdatedId = user.Id;
            var child = invoice.ChildInvoice;
            if (child != null) {                
                invoice.ChildInvoice = null;
                child.ParentInvoice = invoice;
                db.Invoices.Add(child);
            }
            else
                db.Invoices.Add(invoice); 
            db.DisableFilter("IsNotDeleted");
            
            db.SaveChanges();
            if (child != null)
            {

                invoice.ChildId = child.Id;
            }
            if (refuel != null)
            {
                refuel.InvoiceId = invoice.Id;
                refuel.Printed = true;                
            }
            db.SaveChanges();
            invoice.Flight.RefuelItems = null;
            return Ok(invoice);
        }

        [HttpPost]
        [Authorize]
        [Route("cancel")]
        [ResponseType(typeof(InvoiceViewModel))]
        public IHttpActionResult PostCancel([FromBody] int id)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var userName = ClaimsPrincipal.Current.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == userName);

            var userId = user != null ? user.Id : 0;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var refuel = db.RefuelItems.Include(r => r.Flight).FirstOrDefault(r => r.InvoiceId == id);
            var invoice = db.Invoices.FirstOrDefault(r => r.Id == id);
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
                       
            db.SaveChanges();
            return Ok(invoice);
        }


        public IHttpActionResult Get(int id)
        {
            
            var model = db.Invoices.AsNoTracking()      
                .Include(inv=>inv.Flight)          
                .Include(inv=>inv.Items)
                .Include(inv => inv.ChildInvoice.Items)                
                .FirstOrDefault(inv => inv.Id == id);
                      

            var resp = JsonConvert.DeserializeObject<InvoiceViewModel>(JsonConvert.SerializeObject(model));

            resp.FlightCode = model.Flight.Code;
            resp.AircraftCode = model.Flight.AircraftCode;
            resp.AircraftType = model.Flight.AircraftType;
            resp.RouteName = model.Flight.RouteName;
            resp.ParkingLot = model.Flight.Parking + "/" + model.Flight.ValvePit.ToString();

            if (resp.ChildInvoice != null)
            {
                resp.ChildInvoice.FlightCode = model.Flight.Code;
                resp.ChildInvoice.AircraftCode = model.Flight.AircraftCode;
                resp.ChildInvoice.AircraftType = model.Flight.AircraftType;
                resp.ChildInvoice.RouteName = model.Flight.RouteName;
                resp.ChildInvoice.ParkingLot = model.Flight.Parking + "/" + model.Flight.ValvePit.ToString();

            }
            
            resp.OperatorName = (db.RefuelItems.Include(r => r.Operator).Where(re => re.InvoiceId == id).Select(re => re.Operator).FirstOrDefault() ?? new User()).FullName;


            return Ok(resp);
        }
    }
}
