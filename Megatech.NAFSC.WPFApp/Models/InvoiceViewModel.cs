using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Megatech.FMS.WebAPI.Models
{
    public class FlightViewModel
    {
        public FlightViewModel()
        {
            RefuelItems = new List<RefuelViewModel>();
            Currency = Models.Currency.VND.ToString();
            Vendor = Vendor.SKYPEC;
        }
        public FlightViewModel(RefuelViewModel model)
        {
            RefuelItems = new List<RefuelViewModel> { model };
            foreach (var item in model.Others)
            {
                RefuelItems.Add(item);
            }
            FillFlightData(model);


        }

        private void FillFlightData(RefuelViewModel item)
        {
            FlightCode = item.FlightCode;
            AircraftCode = item.AircraftCode;
            AircraftType = item.AircraftType;
            ParkingLot = item.ParkingLot;
            RouteName = item.RouteName;
            RefuelTime = item.RefuelTime.Value;
            Price = item.Price;
            CustomerName = item.Airline.InvoiceName + " " + item.Airline.Name;
            ProductName = item.Airline.ProductName;
            Currency = item.Airline.Currency.ToString();
            Vendor = item.Airline.Vendor;
            StartTime = item.StartTime;
            EndTime = item.EndTime;
            QualityNo = item.QualityNo;
            TaxRate = item.TaxRate;
        }

        public FlightViewModel(IList<RefuelViewModel> items)
        {
            RefuelItems = items;
            if (items.Count > 0)
            {
                FillFlightData(items[0]);
            }
        }
        public int Id { get; set; }

        public string FlightCode { get; set; }

        public string AircraftCode { get; set; }

        public string AircraftType { get; set; }

        public string ParkingLot  { get; set; }

        public string  RouteName { get; set; }      
       
        public string CustomerName { get; set; }

        public string TaxCode { get; set; }

        public string Address { get; set; }

        public DateTime RefuelTime { get; set; }

        public string QualityNo { get; set; }

        public int MyProperty { get; set; }

        public IList<RefuelViewModel> RefuelItems { get; set; }

        //Gallons

        public decimal RealAmount
        {
            get
            {
                return RefuelItems.Sum(r => r.RealAmount);
            }
        }
        //Littres

        public decimal Volume {
            get
            {
                return RefuelItems.Sum(r => r.Volume);
            }
        }

        //kg

        public decimal Weight {
            get
            {
                return RefuelItems.Sum(r => r.Weight);
            }
        }

        public decimal Gallon
        {
            get
            {
                return RefuelItems.Sum(r => r.Gallon);
            }
        }

        public decimal Density {
            get
            {
                try
                {
                    return RefuelItems.Sum(r =>r.Weight) / RefuelItems.Sum(r => r.Volume);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public decimal Temperature
        {
            get
            {
                try
                {
                    return RefuelItems.Sum(r => r.Temperature * r.RealAmount) / RefuelItems.Sum(r => r.RealAmount);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public string ProductName { get; set; }

        public decimal Price { get; set; }


        public string Currency { get; set; }
        public Vendor Vendor { get; set; }

        public decimal Subtotal {
            get {
                return Math.Round(Price * RealAmount, Currency == "USD" ? 2 : 0, MidpointRounding.AwayFromZero);
            }
        }
        

        public decimal TaxRate { get; set; }
        public decimal Tax
        {
            get
            {
                return Math.Round(Subtotal * TaxRate, Currency == "USD" ? 2 : 0, MidpointRounding.AwayFromZero);
            }
        }

        public decimal TotalAmount
        {
            get { return Subtotal + Tax; }
        }

        public decimal SaleAmount { get; set; }

        public string InWords {

        get {
                return VNS.Utils.NumberConvert.NumberToSentence(TotalAmount, false, this.Currency);
            }
        }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
    }
}