using Megatech.FMS.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FMS.Data;
using System.Security.Claims;

namespace Megatech.FMS.WebAPI.Controllers
{
    public class ShiftController : ApiController
    {
        private DataContext db = new DataContext();
        public IHttpActionResult GetShift()
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var userName = ClaimsPrincipal.Current.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == userName);

            var airportId = user != null ? user.AirportId : 0;

            var now = DateTime.Now.TimeOfDay;// DbFunctions.CreateTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);


            var qshift = db.Shifts.Where(s => (s.StartTime < s.EndTime && DbFunctions.CreateTime(s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) <= now && DbFunctions.CreateTime(s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) >= now)
                                            || (s.StartTime > s.EndTime && (DbFunctions.CreateTime(s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) <= now && DbFunctions.CreateTime(s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) <= now
                                                                            || DbFunctions.CreateTime(s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) >= now && DbFunctions.CreateTime(s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) >= now)))

                .Where(s => s.AirportId == airportId);

            var shift = qshift.FirstOrDefault();
            var start = DateTime.Today;
            var end = DateTime.Today;
            string name = "N/A";
            if (shift != null)
            {
                start = start.Add(shift.StartTime.TimeOfDay);
                end = end.Add(shift.EndTime.TimeOfDay);
                if (end < start)
                    end = end.AddDays(1);
                name = shift.Name;
            }
            else
                end = end.AddDays(1).AddSeconds(-1);
            var shiftModel = new ShiftViewModel {
                Name = name,
                StartTime = start,
                EndTime = end,
                AiportId = airportId,
                Date = DateTime.Today

            };
            return Ok(shiftModel);
        }
    }
}
