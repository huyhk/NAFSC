using FMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Megatech.FMS.WebAPI.Models
{
    public class RefuelViewModel
    {
        public RefuelViewModel()
        {
            StartTime = EndTime = DateTime.Now;
            RefuelItemType = REFUEL_ITEM_TYPE.REFUEL;
        }
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string  FlightCode { get; set; }
        public decimal EstimateAmount { get; set; }
        public string AircraftCode { get; set; }
        public string AircraftType { get; set; }
        public string ParkingLot { get; set; }
        public ValvePit ValvePit { get; set; }
        public string RouteName { get; set; }
        public string TruckNo { get; set; }
        public int TruckId { get; set; }
        public decimal RealAmount { get; set; }

        public decimal Temperature { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime? DeviceStartTime { get; set; }

        public DateTime? DeviceEndTime { get; set; }

        public REFUEL_ITEM_STATUS Status { get; set; }

        public DateTime ArrivalTime { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime? RefuelTime { get; set; }

        public decimal StartNumber { get; set; }
        public decimal EndNumber { get; set; }
        public decimal ManualTemperature { get; set; }
        public decimal Density { get; set; }

        public decimal Price { get; set; }

        public decimal TaxRate { get; set; }

        public decimal Weight { get; set; }

        public decimal Volume { get; set; }
        public decimal Extract { get; set; }
        public int AirlineId { get; set; }
        public AirlineViewModel Airline { get; set; }

        public string QualityNo { get;  set; }

        public List<RefuelViewModel> Others { get; set; }

        public FlightStatus FlightStatus { get; set; }
        public decimal Gallon { get;  set; }

        public REFUEL_ITEM_TYPE RefuelItemType { get; set; }

        public ITEM_PRINT_STATUS PrintStatus { get; set; }

        public int? DriverId { get; set; }

        public int? OperatorId { get; set; }

        public string DriverName { get; set; }

        public string OperatorName { get; set; }
    }
}