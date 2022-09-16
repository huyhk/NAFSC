using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using FMS.Data;
using System.Data.Entity.Core.Objects;
using Megatech.FMS.WebAPI.Models;

namespace Megatech.FMS.WebAPI.Controllers
{
    public class AirlinesController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Airlines
        public IQueryable<AirlineViewModel> GetAirlines()
        {
            var today = DateTime.Today;
            //select general price
            var gprice = db.ProductPrices.Include(p => p.Product).FirstOrDefault(p => p.StartDate <= today && p.EndDate >= today && p.Customer == null);
            if (gprice == null) gprice = new ProductPrice { Price = 0, Product = new Product { Name = "" }, Currency = global::FMS.Data.Currency.VND };
           
            var prices = db.ProductPrices.Where(p => p.StartDate <= today && p.EndDate >= today).Include(p=>p.Product).OrderByDescending(p=>p.StartDate);
            var list = from a in db.Airlines
                       let  p = prices.Where(p => p.CustomerId == a.Id).FirstOrDefault()
                       //join p in prices
                       //on a.Id equals p.CustomerId into hasPrice
                       //from hs in hasPrice.DefaultIfEmpty()
                       select new AirlineViewModel
                       {
                           Id = a.Id,
                           Name = a.Name,
                           Code = a.Code,
                           TaxCode = a.TaxCode,
                           Address = a.Address,
                           Price = p ==null? gprice.Price: p.Price,
                           ProductName = p == null ? gprice.Product.Name: p.Product.Name,
                           InvoiceAddress = a.InvoiceAddress,
                           InvoiceName = p == null ? "" : p.Agency.Name ,
                           InvoiceTaxCode = a.InvoiceTaxCode ,
                           Currency = p == null? Currency.VND: p.Currency,
                           Vendor = p == null ? Vendor.SKYPEC: (Vendor)p.OilCompany,
                           Unit = p==null? Unit.KG: p.Unit

                       };
            return list;
                       
        }

        // GET: api/Airlines/5
        [ResponseType(typeof(Airline))]
        public IHttpActionResult GetAirline(int id)
        {
            Airline airline = db.Airlines.Find(id);
            if (airline == null)
            {
                return NotFound();
            }

            return Ok(airline);
        }

        // PUT: api/Airlines/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAirline(int id, Airline airline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != airline.Id)
            {
                return BadRequest();
            }

            db.Entry(airline).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirlineExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Airlines
        [ResponseType(typeof(AirlineViewModel))]
        public IHttpActionResult PostAirline(AirlineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airline = db.Airlines.FirstOrDefault(a => a.Code.Equals(model.Code, StringComparison.InvariantCultureIgnoreCase));
            if (airline == null)
            {
                airline = new Airline { Code = model.Code, Name = model.Name, Address = model.Address, TaxCode = model.TaxCode };
                var product = db.Products.FirstOrDefault();
                var price = new ProductPrice { StartDate = DateTime.Today, EndDate = DateTime.Today.AddYears(1), Customer = airline, Product = product, Price = model.Price };
                db.ProductPrices.Add(price);
                db.SaveChanges();
                model.Id = airline.Id;
                model.ProductName = product.Name;
                
            }
            return Ok(model);
        }

        // DELETE: api/Airlines/5
        [ResponseType(typeof(Airline))]
        public IHttpActionResult DeleteAirline(int id)
        {
            Airline airline = db.Airlines.Find(id);
            if (airline == null)
            {
                return NotFound();
            }

            db.Airlines.Remove(airline);
            db.SaveChanges();

            return Ok(airline);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AirlineExists(int id)
        {
            return db.Airlines.Count(e => e.Id == id) > 0;
        }
    }
}