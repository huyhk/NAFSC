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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Hosting;
using Megatech.NAFSC.DataExport;
using System.Threading;
using System.Threading.Tasks;

namespace Megatech.FMS.WebAPI.Controllers
{
    [RoutePrefix("api/invoices")]
    public class InvoicesController : ApiController
    {
        public InvoicesController()        
        {
            Logging.Logger.SetPath(HostingEnvironment.MapPath("~/Logs"));

            Task.Run(() => {
                var exporter = new Exporter();
                exporter.Export();
            });
        }
        private DataContext db = new DataContext();

        [Authorize]        
        // POST: api/Invoices
        [ResponseType(typeof(InvoiceViewModel))]
        public IHttpActionResult PostInvoice(InvoiceViewModel model)
        {
            Logging.Logger.SetPath(HostingEnvironment.MapPath("~/Logs"));
            Logging.Logger.AppendLog("INVOICE", $"Invoice Number: {model.InvoiceNumber}", "invoices");
            var folderPath = HostingEnvironment.MapPath("~/receipts");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var userName = ClaimsPrincipal.Current.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == userName);

            var userId = user != null ? user.Id : 0;

            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState.Values)
                {

                    Logging.Logger.AppendLog("INVOICE-ERROR", item.Errors.FirstOrDefault().ErrorMessage , "invoices");
                }
               
                return BadRequest(ModelState);
            }
            try
            {

                var refuel = db.RefuelItems.Include(r => r.Flight).FirstOrDefault(r => r.Id == model.RefuelItemId);
                if (refuel == null)
                {
                    Logging.Logger.AppendLog("BAD-REQUEST", $"No refuel item for id {model.RefuelItemId}", "invoices");
                    return BadRequest(ModelState);
                }
                var invoice = JsonConvert.DeserializeObject<Invoice>(JsonConvert.SerializeObject(model));
                var existed = db.Invoices.FirstOrDefault(inv => inv.InvoiceNumber == invoice.InvoiceNumber
                && (inv.Flight.Id == refuel.FlightId) && (inv.Vendor == invoice.Vendor));
                if (existed != null)
                    return Ok(existed);
                invoice.FlightId = refuel.FlightId;
                invoice.CustomerId = refuel.Flight.AirlineId ?? 0;
                invoice.UserCreatedId = invoice.UserUpdatedId = user.Id;
                if (model.ImageString != null)
                {
                    var fileName = model.InvoiceNumber + ".jpg";
                    SaveImage(model.ImageString, fileName, folderPath);
                    invoice.ImagePath = fileName;
                }
                else
                    Logging.Logger.AppendLog("INVOICE", "NO image string found", "invoices");
                if (invoice.Price == 0)
                {
                    var price = db.ProductPrices.Include(p => p.Agency).Include(p => p.Customer).OrderByDescending(p => p.StartDate)
                           .Where(p => p.StartDate <= refuel.RefuelTime)
                           .Where(p => p.CustomerId == invoice.CustomerId)
                           .FirstOrDefault();
                    if (price == null)
                        price = db.ProductPrices.Include(p => p.Product).FirstOrDefault(p => p.StartDate <= refuel.RefuelTime && p.EndDate >= refuel.RefuelTime && p.Customer == null);
                    //var invoiceItem = JsonConvert.DeserializeObject<InvoiceItem>(JsonConvert.SerializeObject(model.Items[0]));
                    //invoice.AddItem(invoiceItem);

                    if (price != null)
                    {
                        refuel.Price = invoice.Price = price.Price;
                        refuel.Currency = invoice.Currency = price.Currency;
                        refuel.Unit = invoice.Unit = price.Unit;
                        invoice.CustomerName = (price.Agency == null ? "" : price.Agency.Name + " - ") + price.Customer.Name;
                    }
                }
                var child = invoice.ChildInvoice;
                if (child != null)
                {
                    child.CustomerName = invoice.CustomerName;
                    invoice.ChildInvoice = null;
                    child.ParentInvoice = invoice;
                    db.Invoices.Add(child);
                    if (model.ChildInvoice.ImageString != null)
                    {
                        var fileName = model.ChildInvoice.InvoiceNumber + ".jpg";
                        SaveImage(model.ChildInvoice.ImageString, fileName, folderPath);
                        child.ImagePath = fileName;
                    }
                }
                else
                    db.Invoices.Add(invoice);
                db.DisableFilter("IsNotDeleted");


                db.SaveChanges();
                Logging.Logger.AppendLog("INVOICE", $" Save OK Invoice Number: {model.InvoiceNumber}", "invoices");
                if (child != null)
                {

                    invoice.ChildId = child.Id;
                }
                if (refuel != null)
                {
                    refuel.InvoiceId = invoice.Id;
                    refuel.Printed = true;
                    refuel.Price = invoice.Price;
                    refuel.Currency = invoice.Currency;
                    refuel.Unit = invoice.Unit;
                }

                db.SaveChanges();
                if (model.Vendor == Vendor.SKYPEC)
                {
                    Export(invoice);
                    if (child != null)
                        Export(child);
                }
                db.SaveChanges();
                invoice.Flight.RefuelItems = null;
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                Logging.Logger.AppendLog("INVOICE-ERROR", ex.Message, "invoices");
                Logging.Logger.LogException(ex, "invoices-ex");
                throw;
            }
        }
        private void Export(Invoice invoice)
        {
            if (!string.IsNullOrEmpty(invoice.ImagePath))
            {
                var exporter = new Exporter();
                var result = exporter.Export(invoice.Id);
                invoice.ExportedResult = (int)result.Result ;
                invoice.DateExported = DateTime.Now;

                invoice.ExportError = result.Message.ToString();
            }

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
                var child = db.Invoices.FirstOrDefault(inv => inv.Id == invoice.ChildId);
                if (child != null)
                {
                    child.UserDeletedId = user.Id;
                    child.DateDeleted = DateTime.Now;
                    child.IsDeleted = true;
                }
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
                      
            

            var resp = JsonConvert.DeserializeObject<InvoiceViewModel>(JsonConvert.SerializeObject(model,new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));

            resp.FlightCode = model.Flight.Code;
            resp.AircraftCode = model.Flight.AircraftCode;
            resp.AircraftType = model.Flight.AircraftType;
            resp.IsInternational = model.Flight.FlightType == FlightType.OVERSEA;
            resp.RouteName = model.Flight.RouteName;
            resp.ParkingLot = model.Flight.Parking + "/" + model.Flight.ValvePit.ToString();

            if (resp.ChildInvoice != null)
            {
                resp.ChildInvoice.FlightCode = model.Flight.Code;
                resp.ChildInvoice.AircraftCode = model.Flight.AircraftCode;
                resp.ChildInvoice.AircraftType = model.Flight.AircraftType;
                resp.ChildInvoice.IsInternational = model.Flight.FlightType == FlightType.OVERSEA;
                resp.ChildInvoice.RouteName = model.Flight.RouteName;
                resp.ChildInvoice.ParkingLot = model.Flight.Parking + "/" + model.Flight.ValvePit.ToString();

            }
            
            resp.OperatorName = (db.RefuelItems.Include(r => r.Operator).Where(re => re.InvoiceId == id).Select(re => re.Operator).FirstOrDefault() ?? new User()).FullName;


            return Ok(resp);
        }

        private void SaveImage(string base64String, string fileName, string folderPath)
        {
            Logging.Logger.AppendLog("IMAGE", $"Save image to path {folderPath}/{fileName}", "invoices");
            SaveImage(Convert.FromBase64String(base64String), fileName, folderPath);
        }
        private void SaveImage(byte[] bytes, string fileName, string folderPath)
        {
            //Logger.AppendLog("RECEIPT", "Save " + fileName, "receipt");
            //var fs = new BinaryWriter(new FileStream(Path.Combine(folderPath, fileName), FileMode.Append, FileAccess.Write));
            //fs.Write(bytes);
            //fs.Close();
            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    Image img = Image.FromStream(ms);
                    img.Save(Path.Combine(folderPath, fileName), ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex, "invoice");
            }
        }

    }
}
