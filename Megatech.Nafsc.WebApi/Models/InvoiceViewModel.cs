
using FMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Megatech.FMS.WebAPI.Models
{
    public class InvoiceViewModel
    {

        public InvoiceViewModel()
        {
            Items = new List<InvoiceItemModel>();
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
            CustomerId = item.AirlineId;
            ProductName = item.Airline.ProductName;
            Currency = item.Airline.Currency;
            Vendor = item.Airline.Vendor;
            Unit = item.Airline.Unit;
            StartTime = item.StartTime;
            EndTime = item.EndTime;
            QualityNo = item.QualityNo;
            TaxRate = item.TaxRate;
        }

       
        public int Id { get; set; }

        public int? RefuelItemId { get; set; }

        public int FlightId { get; set; }

        public string FlightCode { get; set; }

        public string AircraftCode { get; set; }

        public string AircraftType { get; set; }

        public string ParkingLot { get; set; }

        public string RouteName { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string TaxCode { get; set; }

        public string Address { get; set; }

        public DateTime? RefuelTime { get; set; }

        public string QualityNo { get; set; }

        public string OperatorName { get; set; }

        public string InvoiceNumber { get; set; }

        public int? ParentId { get; set; }
        public virtual InvoiceViewModel ParentInvoice { get; set; }

        public int? ChildId { get; set; }
        public virtual InvoiceViewModel ChildInvoice { get; set; }

        public IList<InvoiceItemModel> Items { get; set; }

        //Gallons

        public decimal RealAmount
        {
            get
            {
                return Items.Sum(r => r.RealAmount);
            }
        }
        //Littres

        public decimal Volume
        {
            get
            {
                return Items.Sum(r => (decimal)r.Volume);
            }
        }

        //kg

        public decimal Weight
        {
            get
            {
                return Items.Sum(r => (decimal)r.Weight);
            }
        }

        public decimal Gallon
        {
            get
            {
                return Items.Sum(r => (decimal)r.Gallon);
            }
        }

        public decimal Density
        {
            get
            {
                try
                {
                    return Items.Sum(r => (decimal)(r.Density * r.Volume)) / Items.Sum(r => (decimal)r.Volume);
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
                    return Items.Sum(r => r.Temperature * r.RealAmount) / Items.Sum(r => r.RealAmount);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public string ProductName { get; set; }

        public decimal Price { get; set; }


        public Currency Currency { get; set; }
        public Vendor Vendor { get; set; }
        public Unit Unit { get; set; }

        public bool IsInternational { get; set; }

        public decimal Subtotal
        {
            get;
            set;
        }

        public decimal TaxRate { get; set; }
        public decimal Tax
        {
            get;set;
        }

        public decimal TotalAmount
        {
            get { return Subtotal + Tax; }
        }

        public decimal SaleAmount { get; set; }

        

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get;  set; }

        //base64 image
        public string ImageString { get; set; }
    }

    public class InvoiceItemModel
    {
        public InvoiceItemModel()
        { }
        public InvoiceItemModel(RefuelViewModel item)
        {
            this.RealAmount = item.RealAmount;
            this.Volume = item.Volume;
            this.Weight = item.Weight;
            this.Gallon = item.Gallon;
            this.Density = item.Density;
            this.Temperature = item.Temperature;
            this.StartNumber = item.StartNumber;
            this.EndNumber = item.EndNumber;
            this.TruckNo = item.TruckNo;
        }


        public decimal RealAmount { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Gallon { get; set; }
        public decimal Density { get; set; }
        public decimal Temperature { get; set; }

        public string TruckNo { get; set; }

        public decimal StartNumber { get; set; }
        public decimal EndNumber { get; set; }

       
    }
}