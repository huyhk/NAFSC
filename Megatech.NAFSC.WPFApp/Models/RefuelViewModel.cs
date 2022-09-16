using FMS.Data;
using Megatech.NAFSC.WPFApp.Global;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Megatech.FMS.WebAPI.Models
{
    public class RefuelViewModel:INotifyPropertyChanged
    {
        public RefuelViewModel()
        {
            StartTime = EndTime = DateTime.Now;
            RefuelItemType = REFUEL_ITEM_TYPE.REFUEL;
            Others = new List<RefuelViewModel>();
        }
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string  FlightCode { get; set; }
        public FlightType FlightType { get; set; }
        public decimal EstimateAmount { get; set; }
        public string AircraftCode { get; set; }
        public string AircraftType { get; set; }
        public string ParkingLot { get; set; }
        public ValvePit ValvePit { get; set; }
        public string RouteName { get; set; }
        public string TruckNo { get; set; }
        public int TruckId { get; set; }
        
        public decimal RealAmount
        {
            get;
            set;
               
            
        }

        public int? DriverId { get; set; }

        public int? OperatorId { get; set; }

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

        
        public decimal Weight
        {
            get;
            set;
            
        }

        public decimal Volume
        {
            get
            {
                return RealAmount - (Extract??0);// Math.Round(AppSetting.GALLON_TO_LITTRE * RealAmount, 4);
            }
        }

        public decimal SubTotal { get; set; }

        public int AirlineId { get; set; }
        public AirlineViewModel Airline { get; set; }
        public string QualityNo { get;  set; }

        public List<RefuelViewModel> Others { get; set; }

        public FlightStatus FlightStatus { get; set; }
        public decimal? Gallon
        {
            get;
            set;
        }

        public REFUEL_ITEM_TYPE RefuelItemType { get; set; }

        public bool IsDone
        {
            get { return Status == REFUEL_ITEM_STATUS.DONE; }
        }

        internal void CalculateWeight()
        {
            this.Weight = Math.Round(this.Volume * this.Density, 0, MidpointRounding.AwayFromZero);
            decimal galToLit = AppSetting.GALLON_TO_LITTRE;
            if (this.Airline != null && this.Airline.Vendor == Vendor.PA)
                galToLit = AppSetting.GALLON_TO_LITTRE_PA;
            this.Gallon = Math.Round(this.Volume / galToLit, 0, MidpointRounding.AwayFromZero);
        }

        public decimal? Extract { get; set; }

        public bool HasExtract { get { return Extract > 0; } }

        public string DriverName { get; set; }

        public string OperatorName { get; set; }

        [DoNotNotify]
        public int InvoiceId { get; set; }
        [DoNotNotify]
        public bool Printed { get; set; }

        [DoNotNotify]
        public Guid InvoiceGuid { get; set; }

        [DoNotNotify]
        public Guid LocalGuid { get; set; }

        public string InvoiceNumber { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsReadOnly { get; set; }

        public bool IsDeleted { get; set; }

        public RefuelViewModel Copy()
        {
            var model =  JsonConvert.DeserializeObject<RefuelViewModel>(JsonConvert.SerializeObject(this));
            model.Id = 0;
            model.Status = REFUEL_ITEM_STATUS.NONE;
            model.Printed = false;
            model.LocalGuid = Guid.Empty;

            return model;
        }
        
    }
}