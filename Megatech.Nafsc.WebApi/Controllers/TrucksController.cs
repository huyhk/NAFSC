using FMS.Data;
using Megatech.FMS.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace Megatech.FMS.WebAPI.Controllers
{
    public class TrucksController : ApiController
    {
        private DataContext db = new DataContext();
        [Route("api/trucks/amount")]
        public IHttpActionResult PostAmount(TruckViewModel truck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = db.Trucks.FirstOrDefault(r => r.Code == truck.Code);
            if (model != null)
            {
                model.CurrentAmount = truck.CurrentAmount;
                db.SaveChanges();
            }
            return Ok();
        }
        [ResponseType(typeof(TruckViewModel))]
        public IHttpActionResult GetTrucks(string truckNo)
        {
            string denyNewRefuel = ConfigurationManager.AppSettings["DenyNewRefuel"];
            var model = db.Trucks.Select(t=>new TruckViewModel { Id = t.Id,
                Code = truckNo,
                CurrentAmount = t.CurrentAmount,
                AirportId = t.AirportId.Value,
                AirportCode = t.CurrentAirport.Code,
                AllowNewRefuel = t.CurrentAirport.Code != denyNewRefuel
            }).FirstOrDefault(r => r.Code == truckNo);
            return Ok(model);
        }
        [Authorize]
        public IEnumerable<TruckViewModel> GetTrucks()
        {

            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var userName = ClaimsPrincipal.Current.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == userName);

            var airportId = user != null ? user.AirportId : 0;

            return db.Trucks.Where(t => t.AirportId == airportId).Select(t => new TruckViewModel { Id = t.Id,TruckNo = t.Code, Code = t.Code, CurrentAmount = t.CurrentAmount }).ToList();

        }

        [ResponseType(typeof(TruckViewModel))]
        public IHttpActionResult PostTruck(TruckViewModel model)
        {

            var truck = db.Trucks.Where(t => t.Code == model.TruckNo).FirstOrDefault();
            if (truck != null)
            {
                truck.DeviceSerial = model.DeviceSerial;
                truck.TabletSerial = model.TabletSerial;
                truck.DeviceIP = model.DeviceIP;
                truck.PrinterIP = model.PrinterIP;

                db.SaveChanges();

                model.CurrentAmount = truck.CurrentAmount;
                model.Id = truck.Id;
                
            }
            

            return Ok(model);

        }
    }
}
