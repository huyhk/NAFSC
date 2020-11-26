using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using FMS.Data;
using System.IO;
using System.Data.Entity.Migrations;
using System.ComponentModel.DataAnnotations;

namespace FMS.Data
{
    public class Flight : BaseEntity
    {
        [Required(ErrorMessage = "Vui lòng nhập dữ liệu.")]
        public string Code { get; set; }
        public DateTime? DepartureScheduledTime { get; set; }
        public DateTime? DepartuteTime { get; set; }
        public DateTime? ArrivalScheduledTime { get; set; }
        public DateTime? ArrivalTime { get; set; }

        public DateTime? FlightTime { get; set; }
        //public Route Route { get; set; }
        public string RouteName { get; set; }
        //public int? DepartureId { get; set; }
        //public Airport Departure { get; set; }
        //public int? ArrivalId { get; set; }
        //public Airport Arrival { get; set; }

        public int? AirlineId { get; set; }
        public Airline Airline { get; set; }

        public int? AircraftId { get; set; }
        public Aircraft Aircraft { get; set; }

        public string AircraftCode { get; set; }
        public string AircraftType { get; set; }

        //[Required(ErrorMessage = "Vui lòng nhập dữ liệu.")]
        //[DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal EstimateAmount { get; set; }
        public DateTime? RefuelScheduledTime { get; set; }
        public DateTime? RefuelScheduledHours { get; set; }
        public DateTime? RefuelTime { get; set; }

        public int? ParkingLotId { get; set; }
        public ParkingLot ParkingLot { get; set; }
        public string Parking { get; set; }
        public ValvePit ValvePit { get; set; }
        //public int? RefuelId { get; set; }

        //public virtual  Refuel Refuel { get; set; }

        public List<RefuelItem> RefuelItems { get; set; }

        public string DriverName { get; set; }
        public string TechnicalerName { get; set; }
        public string Shift { get; set; }
        public DateTime? ShiftStartTime { get; set; }
        public DateTime? ShiftEndTime { get; set; }
        public string AirportName { get; set; }
        public string TruckName { get; set; }
        public FlightStatus Status { get; set; }

        public int? AirportId { get; set; }
        
        public Airport Airport { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Price { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        public string Note { get; set; }

        public FLIGHT_CREATED_LOCATION CreatedLocation { get; set; }

        //public REFUEL_STATUS Status { get; set; }

        //[NotMapped]
        //public Refuel Refuel
        //{
        //    get
        //    {
        //        var refs = Refuels.FirstOrDefault(r => r.FlightId == this.Id);
        //        if (refs != null)
        //            return refs;
        //        else
        //            return null;
        //    }
        //}

        public FlightType FlightType { get; set; }

        public void RepairDateTime()
        {
            if (ArrivalScheduledTime == null && DepartureScheduledTime == null)
                ImportError = true;
            else if (RefuelScheduledTime == null)
            {
                if (ArrivalScheduledTime == null)
                    ArrivalScheduledTime = DepartureScheduledTime.Value.AddHours(-1);
                RefuelScheduledTime = ArrivalScheduledTime.Value.AddMinutes(5);
            }
        }
        [NotMapped]
        public bool ImportError { get; set; }
    }
    public enum FlightStatus
    {
        NONE,
        ASSIGNED,
        REFUELING,
        REFUELED,
        CANCELED
    }

    public enum FlightSortOrder
    {
        SORT_ORDER,
        DATE_CREATED,
        ArrivalScheduledTime,
        DepartureScheduledTime,
        RefuelScheduledTime
    }

    public enum SortDirection
    {
        ASCENDING,
        DESCENDING
    }
    [Flags]
    public enum FlightType
    {
        DOMESTIC, // quoc noi
        OVERSEA // quoc ngoai
    }

    public enum FLIGHT_CREATED_LOCATION
    {
        IMPORT,
        WEB,
        COPY,
        APP
    }

    public enum ValvePit
    {
        L,
        R
    }
}
