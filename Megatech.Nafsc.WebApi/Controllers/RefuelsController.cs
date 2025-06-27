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
using Megatech.FMS.WebAPI.Models;
using System.Security.Claims;
using EntityFramework.DynamicFilters;
using System.Globalization;
using Megatech.FMS.WebAPI.App_Start;
using Newtonsoft.Json;

namespace Megatech.FMS.WebAPI.Controllers
{
    public class RefuelsController : ApiController
    {
        public enum TIME_RANGE
        {
            SHIFT,
            TODAY,
            ALL
        }

        private DataContext db = new DataContext();

        [Authorize]

        // GET: api/Refuels
        public IEnumerable<RefuelViewModel> GetRefuels(string sdate = "", int p = 1, int ps=99, bool includeDeleted = false)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var userName = ClaimsPrincipal.Current.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == userName);

            var airportId = (int)(user != null ? user.AirportId : 0);

            var start = DateTime.Today;
            var end = DateTime.Today.AddDays(1);

            if (!string.IsNullOrEmpty(sdate))
            {
                var date = DateTime.Today;
                if (DateTime.TryParseExact(sdate, "yyyyMMdd", new CultureInfo("en-US"), DateTimeStyles.None, out date))
                {
                    start = date; end = date.AddDays(1);
                }
            }

            /*
            var shift = GetCurrentShift(airportId);                        
            
            if (range == TIME_RANGE.SHIFT && shift != null)
            {
                start = start.Add(shift.StartTime.TimeOfDay);
                end = end.Add(shift.EndTime.TimeOfDay);
                if (end < start)
                    end = end.AddDays(1);
            }
            if (range == TIME_RANGE.TODAY)
                end = end.AddDays(1);
            if (range == TIME_RANGE.ALL)
            {
                start = DateTime.MinValue;
                end = DateTime.MaxValue;
            }
            */

            //Get default QCNo
            var qcNo = (db.QCNoHistory.OrderByDescending(q=>q.StartDate).FirstOrDefault(q => q.StartDate <= DateTime.Now) ?? new QCNoHistory()).QCNo;

            if (includeDeleted)
                db.DisableFilter("IsNotDeleted");

            db.Configuration.ProxyCreationEnabled = false;
            var query = db.RefuelItems.Include(r => r.Flight.Airline).Include(r => r.Truck).Include(r=>r.Driver).Include(r=>r.Operator);
            query = query
                    .Where(r => r.Flight.RefuelScheduledTime.Value >= start)
                    .Where(r => r.Flight.RefuelScheduledTime.Value <= end);
            //  .Where(r => r.RefuelItemType == type);
            //if (airportId != 0)
            //    query = query.Where(r => r.Flight.AirportId == airportId);
            /*
                        if (o == 1)
                            query = query.Where(r => r.Truck.Code == truckNo);
                        else
                            query = query.Where(r => r.Truck.Code != truckNo);
            */

            Logger.AppendLog("query",query.ToString(),"query");
            var list = query.OrderBy(r=>r.Flight.RefuelScheduledTime)
                .Select(r => new RefuelViewModel
                {
                    FlightStatus = r.Flight.Status,
                    FlightId = r.FlightId,
                    FlightCode = r.Flight.Code,
                    FlightType = r.Flight.FlightType,
                    EstimateAmount = r.Flight.EstimateAmount,
                    Id = r.Id,
                    AircraftType = r.Flight.AircraftType,
                    AircraftCode = r.Flight.AircraftCode,
                    ParkingLot = r.Flight.Parking,
                    ValvePit = r.Flight.ValvePit,
                    RouteName = r.Flight.RouteName,
                    Status = r.Status,
                    ArrivalTime = r.Flight.ArrivalScheduledTime ?? DateTime.MinValue,
                    DepartureTime = r.Flight.DepartureScheduledTime ?? DateTime.MinValue,
                    RefuelTime = r.Status == REFUEL_ITEM_STATUS.DONE ? (r.RefuelTime?? (r.Flight.VendorModel.Code == "PA"? r.StartTime: r.EndTime)) : r.Flight.RefuelScheduledTime,
                    //RefuelTime =  r.Flight.RefuelScheduledTime,
                    RealAmount = r.Amount,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime ?? DateTime.MinValue,
                    StartNumber = r.StartNumber,
                    EndNumber = r.EndNumber,
                    DeviceEndTime = r.DeviceEndTime,
                    DeviceStartTime = r.DeviceStartTime,
                    Density = r.Density,
                    ManualTemperature = r.ManualTemperature,
                    Temperature = r.Temperature,
                    Price = r.Price,
                    Currency = r.Currency,
                    Unit = r.Unit,
                    QualityNo = r.QCNo ?? qcNo,
                    TaxRate = r.TaxRate,
                    TruckNo = r.Truck.Code,
                    Gallon = r.Gallon,
                    AirlineId = r.Flight.AirlineId ?? 0,
                    Airline = new AirlineViewModel { Id = r.Flight.AirlineId??0, Name = r.Flight.Airline.Name, InvoiceName = r.Flight.Airline.InvoiceName, VendorModelId = r.Flight.VendorModelId, VendorModelCode = r.Flight.VendorModel.Code },
                    RefuelItemType = r.RefuelItemType,
                    DriverId = r.DriverId,
                    OperatorId = r.OperatorId,
                    DriverName = r.Driver == null ? "" : r.Driver.FullName,
                    OperatorName = r.Operator == null ? "" : r.Operator.FullName,
                    Printed = r.Printed,
                    Extract = r.Extract,
                    Volume = r.Volume,
                    Weight = r.Weight, 
                    IsDeleted = r.IsDeleted,
                    VendorModelId = r.Flight.VendorModelId,
                    VendorModelCode = r.Flight.VendorModel.Code


                }).Skip((p-1)*ps).Take(ps).ToList();//.OrderBy(r => r.Status).ThenByDescending(r => r.RefuelTime);

            var flights = list.Select(item => item.FlightId).ToArray();
            
            return list;
        }

        private Shift GetCurrentShift(int airportId)
        {
            var now = DateTime.Now.TimeOfDay;


            var qshift = db.Shifts.Where(s => (s.StartTime < s.EndTime && DbFunctions.CreateTime(s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) <= now && DbFunctions.CreateTime(s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) >= now)
                                            || (s.StartTime > s.EndTime && (DbFunctions.CreateTime(s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) <= now && DbFunctions.CreateTime(s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) <= now
                                                                            || DbFunctions.CreateTime(s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) >= now && DbFunctions.CreateTime(s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) >= now)))

                .Where(s => s.AirportId == airportId);
            return qshift.FirstOrDefault();
        }

        // GET: api/Refuels/5
        [ResponseType(typeof(RefuelViewModel))]
        public IHttpActionResult GetRefuel(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var refuel = db.RefuelItems.Include(r=>r.Flight).Select(r => new RefuelViewModel
            {
                FlightStatus = r.Flight.Status,
                FlightId = r.FlightId,
                FlightCode = r.Flight.Code,
                FlightType = r.Flight.FlightType,
                EstimateAmount = r.Flight.EstimateAmount,
                Id = r.Id,
                AircraftType = r.Flight.AircraftType,
                AircraftCode = r.Flight.AircraftCode,
                ParkingLot = r.Flight.Parking,
                ValvePit = r.Flight.ValvePit,
                RouteName = r.Flight.RouteName,
                Status = r.Status,
                ArrivalTime = r.Flight.ArrivalScheduledTime ?? DateTime.MinValue,
                DepartureTime = r.Flight.DepartureScheduledTime ?? DateTime.MinValue,
                RefuelTime = r.Status == REFUEL_ITEM_STATUS.DONE? r.RefuelTime : r.Flight.RefuelScheduledTime,
                RealAmount = r.Amount,
                StartTime = r.StartTime,
                EndTime = r.EndTime ?? DateTime.MinValue,
                StartNumber = r.StartNumber,
                EndNumber = r.EndNumber,
                DeviceEndTime = r.DeviceEndTime,
                DeviceStartTime = r.DeviceStartTime,
                Density = r.Density,
                ManualTemperature = r.ManualTemperature,
                Temperature = r.Temperature,
                QualityNo = r.QCNo,
                TaxRate = r.TaxRate,
                Price = r.Price,
                Currency = r.Currency,
                Unit = r.Unit,
                TruckNo = r.Truck.Code,
                Gallon = r.Gallon,
                AirlineId = r.Flight.AirlineId ?? 0,
                RefuelItemType = r.RefuelItemType,
                DriverId = r.DriverId,
                OperatorId = r.OperatorId,
                Printed = r.Printed,
                InvoiceId = r.InvoiceId ?? 0,
                InvoiceNumber = r.Invoice ==null?"" : r.Invoice.InvoiceNumber,
                Extract = r.Extract,
                Volume = r.Volume,
                Weight = r.Weight, 


            }).FirstOrDefault(r => r.Id == id);
            if (refuel == null)
            {
                return NotFound();
            }
            var today = DateTime.Today;

            if (refuel.Status == REFUEL_ITEM_STATUS.DONE)
                today = refuel.RefuelTime.Value;

            var qcNo = (db.QCNoHistory.OrderByDescending(q=>q.StartDate).FirstOrDefault(q => q.StartDate <= DateTime.Now) ?? new QCNoHistory()).QCNo;



            var airline = GetAirline(refuel);
            if (airline != null)
            {
                refuel.AirlineId = airline.Id;
                refuel.Airline = airline;
                if (refuel.Status != REFUEL_ITEM_STATUS.DONE)
                    refuel.Price = airline.Price;
                refuel.Currency = airline.Currency;
                refuel.Unit = airline.Unit;
                refuel.Vendor = airline.Vendor;
                refuel.VendorModelId = airline.VendorModelId;
                refuel.VendorModelCode = airline.VendorModelCode;
            }

            //if (refuel.FlightStatus == FlightStatus.REFUELED)
            //{
            refuel.Others = db.RefuelItems.Where(r => r.FlightId == refuel.FlightId && r.Id != refuel.Id)
                   .Select(r => new RefuelViewModel
                   {
                       FlightStatus = refuel.FlightStatus,
                       FlightId = r.FlightId,
                       FlightCode = r.Flight.Code,
                       FlightType = r.Flight.FlightType,
                       EstimateAmount = r.Flight.EstimateAmount,
                       Id = r.Id,
                       AircraftType = r.Flight.AircraftType,
                       AircraftCode = r.Flight.AircraftCode,
                       ParkingLot = r.Flight.Parking,
                       ValvePit = r.Flight.ValvePit,
                       RouteName = r.Flight.RouteName,
                       Status = r.Status,
                       ArrivalTime = r.Flight.ArrivalScheduledTime ?? DateTime.MinValue,
                       DepartureTime = r.Flight.DepartureScheduledTime ?? DateTime.MinValue,
                       RefuelTime = r.Status == REFUEL_ITEM_STATUS.DONE ? r.RefuelTime : r.Flight.RefuelScheduledTime,
                       RealAmount = r.Amount,
                       StartTime = r.StartTime,
                       EndTime = r.EndTime ?? DateTime.MinValue,
                       StartNumber = r.StartNumber,
                       EndNumber = r.EndNumber,
                       DeviceEndTime = r.DeviceEndTime,
                       DeviceStartTime = r.DeviceStartTime,
                       Density = r.Density,
                       ManualTemperature = r.ManualTemperature,
                       Temperature = r.Temperature,
                       QualityNo = r.QCNo??qcNo,
                       TaxRate = r.TaxRate,
                       TruckNo = r.Truck.Code,
                       Gallon = r.Gallon,
                       AirlineId = r.Flight.AirlineId ?? 0,
                       RefuelItemType = r.RefuelItemType,
                       Printed = r.Printed,
                       InvoiceId = r.InvoiceId ?? 0,
                       InvoiceNumber = r.Invoice ==null?"" :r.Invoice.InvoiceNumber,
                       Extract = r.Extract,
                       Volume = r.Volume,
                       Weight = r.Weight,
                       DriverId = r.DriverId,
                       OperatorId = r.OperatorId,
                       Currency = r.Currency,
                       Unit = r.Unit,
                       Price = r.Price, 
                       VendorModelId = refuel.VendorModelId,
                       VendorModelCode = refuel.VendorModelCode,


                   }).ToList();
            //}


            return Ok(refuel);
        }

        // PUT: api/Refuels/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRefuel(int id, Refuel refuel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != refuel.Id)
            {
                return BadRequest();
            }

            db.Entry(refuel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefuelExists(id))
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
        [Authorize]
        // POST: api/Refuels
        [ResponseType(typeof(RefuelViewModel))]
        public IHttpActionResult PostRefuel(RefuelViewModel refuel)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var userName = ClaimsPrincipal.Current.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == userName);

            var userId = user != null ? user.Id : 0;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var transaction = db.Database.BeginTransaction();
            Logger.AppendLog("REFUEL", "Start post " + refuel.FlightCode);
            Logger.AppendLog("REFUEL", JsonConvert.SerializeObject(refuel),"post-data");
            try
            {
                db.DisableFilter("IsNotDeleted");
                if (refuel.Id == 0)
                {
                    if (refuel.FlightId == 0)
                    {
                        //insert new flight
                        Flight fl = new Flight
                        {
                            Code = refuel.FlightCode,
                            AircraftCode = refuel.AircraftCode,
                            Parking = refuel.ParkingLot,
                            ValvePit = refuel.ValvePit,
                            RouteName = refuel.RouteName,
                            AircraftType = refuel.AircraftType,
                            ArrivalTime = DateTime.Today.Add(refuel.ArrivalTime.TimeOfDay),
                            DepartuteTime = DateTime.Today.Add(refuel.DepartureTime.TimeOfDay),
                            ArrivalScheduledTime = DateTime.Today.Add(refuel.ArrivalTime.TimeOfDay),
                            DepartureScheduledTime = DateTime.Today.Add(refuel.DepartureTime.TimeOfDay),
                            RefuelScheduledTime = DateTime.Today.Add(refuel.RefuelTime.Value.TimeOfDay),
                            EstimateAmount = refuel.EstimateAmount,
                            RefuelTime = refuel.RefuelTime,
                            StartTime = refuel.StartTime,
                            EndTime = refuel.EndTime,
                            AirportId = user.AirportId,
                            CreatedLocation = FLIGHT_CREATED_LOCATION.APP,
                            Status = FlightStatus.ASSIGNED,
                            UserCreatedId = user.Id,
                            VendorModelId = refuel.VendorModelId,

                        };

                        db.Flights.Add(fl);
                        db.SaveChanges();
                        refuel.FlightId = fl.Id;
                    }
                }

                var model = db.RefuelItems.Include(r => r.Flight).FirstOrDefault(r => r.Id == refuel.Id);
                var qcNo = (db.QCNoHistory.OrderByDescending(q => q.StartDate).FirstOrDefault(q => q.StartDate <= DateTime.Now) ?? new QCNoHistory()).QCNo;
                if (model == null)
                {
                    model = new RefuelItem
                    {
                        FlightId = refuel.FlightId,
                        UserCreatedId = user.Id,
                        CreatedLocation = ITEM_CREATED_LOCATION.APP
                    };

                    db.RefuelItems.Add(model);
                }
                if (model != null)
                {
                    var truck = db.Trucks.OrderBy(t => t.IsDeleted).FirstOrDefault(t => t.Code == refuel.TruckNo);
                    if (truck != null)
                    {
                        model.TruckId = truck.Id;
                    }

                    model.Amount = refuel.RealAmount;
                    model.Temperature = refuel.Temperature;
                    model.Status = refuel.Status;
                    model.EndTime = refuel.EndTime;
                    model.StartTime = refuel.StartTime;
                    model.EndNumber = refuel.EndNumber;
                    model.StartNumber = refuel.StartNumber;
                    model.StartTime = refuel.StartTime;
                    if (refuel.EndTime > DateTime.MinValue)
                        model.EndTime = refuel.EndTime;
                    else
                        model.EndTime = null;

                    model.ManualTemperature = refuel.Temperature;
                    model.Density = refuel.Density;
                    model.Price = refuel.Price;
                    model.Unit = refuel.Unit;
                    model.Currency = refuel.Currency;

                    model.QCNo = refuel.QualityNo ?? qcNo;
                    model.TaxRate = refuel.TaxRate;
                    model.Status = refuel.Status;
                    model.DeviceStartTime = refuel.DeviceStartTime;
                    model.DeviceEndTime = refuel.DeviceEndTime;
                    model.Gallon = refuel.Gallon;
                    model.Weight = refuel.Weight;
                    model.Volume = refuel.Volume;
                    //model.Volume15 = refuel.Volume;
                    model.Extract = refuel.Extract;
                    model.DateUpdated = DateTime.Now;
                    model.UserUpdatedId = userId;
                    model.RefuelItemType = refuel.RefuelItemType;
                    model.OperatorId = refuel.OperatorId;
                    model.DriverId = refuel.DriverId;


                    if (model.Status == REFUEL_ITEM_STATUS.DONE)
                        model.RefuelTime = refuel.RefuelTime;

                }
                Logger.AppendLog("REFUEL", "Save changes " + refuel.FlightCode);
                db.SaveChanges();


                var flight = db.Flights.Include(f => f.RefuelItems).FirstOrDefault(f => f.Id == model.FlightId);
                if (flight != null)
                {
                    flight.TotalAmount = flight.RefuelItems.Where(r => r.Status == REFUEL_ITEM_STATUS.DONE).Sum(r => r.Amount);

                    if (flight.RefuelItems.All(r => r.Status == REFUEL_ITEM_STATUS.DONE))
                        flight.Status = FlightStatus.REFUELED;
                    else if (flight.RefuelItems.Any(r => r.Status == REFUEL_ITEM_STATUS.DONE || r.Status == REFUEL_ITEM_STATUS.PROCESSING))
                        flight.Status = FlightStatus.REFUELING;
                    if (flight.Status == FlightStatus.REFUELED || flight.Status == FlightStatus.REFUELING)
                    {
                        if (flight.RefuelItems.Any(r => r.Status == REFUEL_ITEM_STATUS.DONE))
                        {
                            flight.StartTime = flight.RefuelItems.Where(r => r.Status == REFUEL_ITEM_STATUS.DONE).Min(r => r.StartTime);
                            flight.EndTime = flight.RefuelItems.Where(r => r.Status == REFUEL_ITEM_STATUS.DONE).Max(r => r.EndTime).Value;
                            flight.RefuelTime = flight.EndTime;
                        }
                    }
                    if (refuel.AirlineId > 0)
                        flight.AirlineId = refuel.AirlineId;
                    flight.AircraftCode = refuel.AircraftCode;
                    flight.AircraftType = refuel.AircraftType;
                    flight.Parking = refuel.ParkingLot;
                    flight.ValvePit = refuel.ValvePit;
                    flight.RouteName = refuel.RouteName;
                    flight.FlightType = refuel.FlightType;
                    flight.DateUpdated = DateTime.Now;
                    flight.UserUpdatedId = userId;
                    flight.VendorModelId = refuel.VendorModelId;
                    //get current price
                    if (refuel.Status == REFUEL_ITEM_STATUS.DONE)
                    {
                        var price = db.ProductPrices.OrderByDescending(p => p.StartDate)
                            .Where(p => p.StartDate <= flight.RefuelTime)
                            .Where(p => p.CustomerId == refuel.AirlineId)
                            .Where(p => !p.IsDeleted)
                            .FirstOrDefault();

                        if (price != null)
                        {
                            flight.OilCompany = price.OilCompany;
                            flight.VendorModelId = price.VendorModelId;
                            model.Price = price.Price;
                            model.Unit = price.Unit;
                            model.Currency = price.Currency;
                        }
                    }

                    db.SaveChanges();
                }

                var newItem = db.RefuelItems.Select(r => new RefuelViewModel
                {
                    FlightStatus = flight.Status,
                    FlightId = r.FlightId,
                    FlightCode = r.Flight.Code,
                    FlightType = r.Flight.FlightType,
                    EstimateAmount = r.Flight.EstimateAmount,
                    Id = r.Id,
                    AircraftType = r.Flight.AircraftType,
                    AircraftCode = r.Flight.AircraftCode,
                    ParkingLot = r.Flight.Parking,
                    ValvePit = r.Flight.ValvePit,
                    RouteName = r.Flight.RouteName,
                    Status = r.Status,
                    ArrivalTime = r.Flight.ArrivalScheduledTime ?? DateTime.MinValue,
                    DepartureTime = r.Flight.DepartureScheduledTime ?? DateTime.MinValue,
                    RefuelTime = r.Status == REFUEL_ITEM_STATUS.DONE ? r.RefuelTime : r.Flight.RefuelScheduledTime,
                    RealAmount = r.Amount,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime ?? DateTime.MinValue,
                    StartNumber = r.StartNumber,
                    EndNumber = r.EndNumber,
                    DeviceEndTime = r.DeviceEndTime,
                    DeviceStartTime = r.DeviceStartTime,
                    Density = r.Density,
                    ManualTemperature = r.ManualTemperature,
                    Temperature = r.Temperature,
                    QualityNo = r.QCNo ?? qcNo,
                    TaxRate = r.TaxRate,
                    Price = r.Price,
                    TruckNo = r.Truck.Code,
                    Gallon = r.Gallon,
                    AirlineId = r.Flight.AirlineId ?? 0,
                    RefuelItemType = r.RefuelItemType,
                    Extract = r.Extract,
                    Volume = r.Volume,
                    Weight = r.Weight,
                    DriverId = r.DriverId,
                    OperatorId = r.OperatorId,
                    Currency = r.Currency,
                    Unit = r.Unit,
                    InvoiceId = r.InvoiceId ?? 0,
                    InvoiceNumber = r.Invoice == null ? "" : r.Invoice.InvoiceNumber


                }).FirstOrDefault(r => r.Id == model.Id);

                var airline = GetAirline(newItem);
                if (airline != null)
                {
                    newItem.AirlineId = airline.Id;
                    newItem.Airline = airline;
                    //newItem.Price = airline.Price;
                    newItem.Currency = airline.Currency;
                    newItem.Unit = airline.Unit;
                    newItem.Vendor = airline.Vendor;
                    newItem.VendorModelId = airline.VendorModelId;
                    newItem.VendorModelCode = airline.VendorModelCode;
                }

                //if (flight.Status == FlightStatus.REFUELED)
                //{
                newItem.Others = db.RefuelItems.Where(r => r.FlightId == flight.Id && r.Id != model.Id)
                   .Select(r => new RefuelViewModel
                   {
                       FlightStatus = flight.Status,
                       FlightId = r.FlightId,
                       FlightCode = r.Flight.Code,
                       FlightType = r.Flight.FlightType,
                       EstimateAmount = r.Flight.EstimateAmount,
                       Id = r.Id,
                       AircraftType = r.Flight.AircraftType,
                       AircraftCode = r.Flight.AircraftCode,
                       ParkingLot = r.Flight.Parking,
                       ValvePit = r.Flight.ValvePit,
                       RouteName = r.Flight.RouteName,
                       Status = r.Status,
                       ArrivalTime = r.Flight.ArrivalScheduledTime ?? DateTime.MinValue,
                       DepartureTime = r.Flight.DepartureScheduledTime ?? DateTime.MinValue,
                       RefuelTime = r.Status == REFUEL_ITEM_STATUS.DONE ? r.RefuelTime : r.Flight.RefuelScheduledTime,
                       RealAmount = r.Amount,
                       StartTime = r.StartTime,
                       EndTime = r.EndTime ?? DateTime.MinValue,
                       StartNumber = r.StartNumber,
                       EndNumber = r.EndNumber,
                       DeviceEndTime = r.DeviceEndTime,
                       DeviceStartTime = r.DeviceStartTime,
                       Density = r.Density,
                       ManualTemperature = r.ManualTemperature,
                       Temperature = r.Temperature,
                       QualityNo = r.QCNo ?? qcNo,
                       TaxRate = r.TaxRate,
                       TruckNo = r.Truck.Code,
                       Gallon = r.Gallon,
                       AirlineId = r.Flight.AirlineId ?? 0,
                       RefuelItemType = r.RefuelItemType,
                       Extract = r.Extract,
                       Volume = r.Volume,
                       Weight = r.Weight,
                       DriverId = r.DriverId,
                       OperatorId = r.OperatorId,
                       Currency = r.Currency,
                       Unit = r.Unit,
                       Price = r.Price,
                       InvoiceId = r.InvoiceId ?? 0,
                       InvoiceNumber = r.Invoice == null ? "" : r.Invoice.InvoiceNumber


                   }).ToList();
                //}
                transaction.Commit();

                return Ok(newItem);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Logging.Logger.LogException(ex,"refuel");
                Logger.AppendLog("DATA", JsonConvert.SerializeObject(refuel),"data-error.json");
                return InternalServerError();
            }
        }

        private AirlineViewModel GetAirline(RefuelViewModel refuel)
        {
            var today = DateTime.Today;

            if (refuel.Status == REFUEL_ITEM_STATUS.DONE)
                today = refuel.RefuelTime.Value;

            

            //select general price
            var gprice = db.ProductPrices.Include(p => p.Product).Include(p=>p.VendorModel)
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.StartDate <= today && p.EndDate >= today && p.Customer == null);
            if (gprice == null) gprice = new ProductPrice { Price = 0, Product = new Product { Name = "" }, VendorModel = new Nafsc.Data.Entity.VendorModel { } };

            var prices = db.ProductPrices.Where(p => p.StartDate <= today && p.EndDate >= today).Where(p => !p.IsDeleted).Include(p => p.Product).OrderByDescending(p => p.StartDate);
            var airlinequery = (from a in db.Airlines.Where(a => a.Id == refuel.AirlineId)
                                let p = prices.Where(p => p.CustomerId == a.Id).FirstOrDefault()
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
                                    Price = p == null ? gprice.Price : p.Price,
                                    ProductName = p == null ? gprice.Product.Name : p.Product.Name,
                                    InvoiceAddress = a.InvoiceAddress,
                                    InvoiceName = p == null ? "" : p.Agency.Name ,
                                    InvoiceTaxCode = a.InvoiceTaxCode,
                                    
                                    Currency = p == null ? gprice.Currency : p.Currency,
                                    Vendor = (Vendor)(p == null ? gprice.OilCompany : p.OilCompany),
                                    VendorModelId = p.VendorModelId,
                                    VendorModelCode = p.VendorModel.Code,
                                    Unit = p == null ? gprice.Unit : p.Unit

                                });
            var airline = airlinequery.FirstOrDefault();
            return airline;
        }

        private AirlineViewModel GetAirline(DateTime today, int airlineId)
        {        


            //select general price
            var gprice = db.ProductPrices.Include(p => p.Product).Include(p => p.VendorModel)
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.StartDate <= today && p.EndDate >= today && p.Customer == null);
            if (gprice == null) gprice = new ProductPrice { Price = 0, Product = new Product { Name = "" }, VendorModel = new Nafsc.Data.Entity.VendorModel { } };

            var prices = db.ProductPrices.Where(p => p.StartDate <= today && p.EndDate >= today).Where(p => !p.IsDeleted).Include(p => p.Product).OrderByDescending(p => p.StartDate);
            var airlinequery = (from a in db.Airlines.Where(a => a.Id == airlineId)
                                let p = prices.Where(p => p.CustomerId == a.Id).FirstOrDefault()
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
                                    Price = p == null ? gprice.Price : p.Price,
                                    ProductName = p == null ? gprice.Product.Name : p.Product.Name,
                                    InvoiceAddress = a.InvoiceAddress,
                                    InvoiceName = p == null ? "" : p.Agency.Name,
                                    InvoiceTaxCode = a.InvoiceTaxCode,

                                    Currency = p == null ? gprice.Currency : p.Currency,
                                    Vendor = (Vendor)(p == null ? gprice.OilCompany : p.OilCompany),
                                    VendorModelId = p.VendorModelId,
                                    VendorModelCode = p.VendorModel.Code,
                                    Unit = p == null ? gprice.Unit : p.Unit

                                });
            var airline = airlinequery.FirstOrDefault();
            return airline;
        }
        // DELETE: api/Refuels/5
        [ResponseType(typeof(Refuel))]
        public IHttpActionResult DeleteRefuel(int id)
        {
            Refuel refuel = db.Refuels.Find(id);
            if (refuel == null)
            {
                return NotFound();
            }

            db.Refuels.Remove(refuel);
            db.SaveChanges();

            return Ok(refuel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RefuelExists(int id)
        {
            return db.Refuels.Count(e => e.Id == id) > 0;
        }
    }
}