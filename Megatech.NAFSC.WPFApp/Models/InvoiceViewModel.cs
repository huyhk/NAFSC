
using Megatech.NAFSC.WPFApp.Models;
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
            Currency = Models.Currency.VND;
            Vendor = Vendor.SKYPEC;
        }
        public InvoiceViewModel(RefuelViewModel model, InvoiceOption option):this()
        {
            if (option !=null && option.Split)
            {
                var amount = model.RealAmount;
                model.RealAmount -= option.SplitAmount;
                InvoiceViewModel child = new InvoiceViewModel(model, new InvoiceOption { InvoiceNumber = option.InvoiceNumber2});
                //child.InvoiceNumber = option.InvoiceNumber2;                
                model.RealAmount = option.SplitAmount;
                
                ChildInvoice = child;
                
            }
            InvoiceNumber = option.InvoiceNumber;
            Items.Add(new InvoiceItemModel(model));

            RefuelItemId = model.Id;
            //foreach (var item in model.Others)
            //{
            //    InvoiceItemModel invoiceItem = new InvoiceItemModel(item);
            //    InvoiceItems.Add(invoiceItem);
            //}
           
            FillFlightData(model);


        }

        private void FillFlightData(RefuelViewModel item)
        {
            FlightId = item.FlightId;
            FlightCode = item.FlightCode;
            AircraftCode = item.AircraftCode;
            AircraftType = item.AircraftType;
            ParkingLot = item.ParkingLot;
            RouteName = item.RouteName;
            RefuelTime = item.RefuelTime.Value;
            Price = item.Price;
            CustomerId = item.AirlineId;
            CustomerName = item.Airline.InvoiceName + " " + item.Airline.Name;
            ProductName = item.Airline.ProductName;
            Currency = item.Airline.Currency;
            Vendor = item.Airline.Vendor;
            StartTime = item.StartTime;
            EndTime = item.EndTime;
            QualityNo = item.QualityNo;
            TaxRate = item.TaxRate;
        }

        public InvoiceViewModel(IList<RefuelViewModel> items)
        {
            Items = InvoiceItemModel.CreateList(items);
            if (items.Count > 0)
            {
                FillFlightData(items[0]);
            }
        }
        public int Id { get; set; }
        public int? RefuelItemId { get; set; }
        public int FlightId { get; set; }
        public string FlightCode { get; set; }

        public string AircraftCode { get; set; }

        public string AircraftType { get; set; }

        public string ParkingLot  { get; set; }

        public string  RouteName { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string TaxCode { get; set; }

        public string Address { get; set; }

        public DateTime RefuelTime { get; set; }

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

        public decimal? Volume {
            get
            {
                return Items.Sum(r => r.Volume);
            }
        }

        //kg

        public decimal? Weight {
            get
            {
                return Items.Sum(r => r.Weight);
            }
        }

        public decimal? Gallon
        {
            get
            {
                return Items.Sum(r => r.Gallon);
            }
        }

        public decimal? Density {
            get
            {
                try
                {
                    return Items.Sum(r=>r.Weight) / Items.Sum(r => r.Volume);
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

        public decimal Subtotal {
            get {
                return Math.Round(Price * RealAmount, Currency == Currency.USD ? 2 : 0, MidpointRounding.AwayFromZero);
            }
        }
        
        public decimal TaxRate { get; set; }
        public decimal Tax
        {
            get
            {
                return Math.Round(Subtotal * TaxRate, Currency.ToString() == "USD" ? 2 : 0, MidpointRounding.AwayFromZero);
            }
        }

        public decimal TotalAmount
        {
            get { return Subtotal + Tax; }
        }

        public decimal SaleAmount { get; set; }

        public string InWords {

        get {
                return VNS.Utils.NumberConvert.NumberToSentence(TotalAmount, false, this.Currency.ToString());
            }
        }

        public DateTime StartTime { get;  set; }
        public DateTime EndTime { get; set; }

    }

    public class InvoiceItemModel 
    {
        public InvoiceItemModel()
        { }
        public InvoiceItemModel(RefuelViewModel item)
        {
            this.RealAmount = item.RealAmount;
            this.Volume = item.Volume;
            this.Weight= item.Weight;
            this.Gallon = item.Gallon;
            this.Density = item.Density;
            this.Temperature = item.Temperature;
            this.StartNumber= item.StartNumber;
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

        internal static IList<InvoiceItemModel> CreateList(IList<RefuelViewModel> items)
        {
            var list = new List<InvoiceItemModel>();
            foreach (var item in items)
            {
                list.Add(new InvoiceItemModel(item));
            }
            return list;
        }
    }
}