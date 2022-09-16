
using Megatech.NAFSC.WPFApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMS.Data;
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
        public InvoiceViewModel(List<RefuelViewModel> refuels, InvoiceOption option)
        {
            refuels.OrderBy(r => r.EndTime).FirstOrDefault();
        }
        public InvoiceViewModel(RefuelViewModel refuel, InvoiceOption option):this()
        {
            var model = refuel.Copy();
            model.Id = refuel.Id;
            var autoNum = refuel.TruckNo.Substring(refuel.TruckNo.Length - 2) + refuel.EndTime.ToString("yyMMddHHmm1");
            var autoNum2 = refuel.TruckNo.Substring(refuel.TruckNo.Length - 2) + refuel.EndTime.ToString("yyMMddHHmm2");
            InvoiceNumber = option.AutoNumber? autoNum: option.InvoiceNumber;
            if (option !=null && option.Split)
            {
                var amount = model.RealAmount;
                
                model.RealAmount -= option.SplitAmount;
                model.StartNumber = model.StartNumber + option.SplitAmount;
                model.CalculateWeight();
                InvoiceViewModel child = new InvoiceViewModel(model, new InvoiceOption { InvoiceNumber = option.AutoNumber ? autoNum2: option.InvoiceNumber2});
                //child.InvoiceNumber = option.InvoiceNumber2;                
                model.RealAmount = option.SplitAmount;
                model.CalculateWeight();
                model.StartNumber = model.StartNumber - option.SplitAmount;
                model.EndNumber = model.StartNumber + option.SplitAmount;
                ChildInvoice = child;
                
            }
            
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
            TaxCode = item.Airline.TaxCode;
            Address = item.Airline.Address;
            ProductName = item.Airline.ProductName;
            Currency = item.Airline.Currency;
            Unit = item.Airline.Unit;
            Vendor = item.Airline.Vendor;
            StartTime = item.StartTime;
            EndTime = item.EndTime;
            QualityNo = item.QualityNo;
            
            TaxRate = item.TaxRate;
            IsInternational = item.FlightType == FlightType.OVERSEA;

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

        public bool IsInternational { get; set; }

        public int? ParentId { get; set; }
        public virtual InvoiceViewModel ParentInvoice { get; set; }

        public int? ChildId { get; set; }
        public virtual InvoiceViewModel ChildInvoice { get; set; }

        public IList<InvoiceItemModel> Items { get; set; }


        public string ImagePath { get; set; }

        public string ImageString { get; set; }

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
                    return (decimal)Items.Sum(r=> (r.Density * (decimal) r.Volume)) / Items.Sum(r => r.Volume);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public decimal SumDensity
        {
            get
            {
                return (decimal)Items.Sum(r => (r.Density * (decimal)r.Volume));
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

        public Unit Unit { get; set; }
        public Currency Currency { get; set; }
        public Vendor Vendor { get; set; }

        public decimal Subtotal {
            get {
                return Math.Round(Price * (decimal)(Unit == Unit.KG?Weight: Gallon), Currency == Currency.USD ? 2 : 0, MidpointRounding.AwayFromZero);
            }
        }
        
        public decimal TaxRate { get; set; }
        public decimal Tax
        {
            get
            {
                return Math.Round(Subtotal * TaxRate, Currency== Currency.USD ? 2 : 0, MidpointRounding.AwayFromZero);
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
        public Guid LocalGuid { get; set; }


        public bool IsUSD
        {
            get { return Currency == Currency.USD; }
        }

        public string[] Names
        {
            get { return Split(CustomerName, 50); }
        }

        public string[] Addresses
        {
            get { return Split(Address, 50); }
        }

        public string SellerName { get; internal set; } = "NORTHERN REGION BRANCH OF VIETNAM AIR PETROL \nCOMPANY LIMITED (SKYPEC)";
        public string SellerTaxCode { get; internal set; } = "0100107638-001";
        public string SellerAddress { get; internal set; } = "Hà Nội";

        public bool Exported { get; set; }
        public bool CanSync
        {
            get { return Vendor == Vendor.PA || !string.IsNullOrEmpty(ImagePath) ; }
        }

        public bool CanExport
        {
            get { return !Exported && !string.IsNullOrEmpty(ImagePath); }
        }

        private string[] Split(string text, int length)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= length)
                return new string[] { text };
            else
            {
                var words = text.Split(new char[] { ' ' });
                var s = new string[2] { "", "" };
                foreach (var w in words)
                {
                    if (s[0].Length + w.Length < length)
                        s[0] += (string.IsNullOrEmpty(s[0]) ? "" : " ") + w;
                    else
                        s[1] += (string.IsNullOrEmpty(s[1]) ? "" : " ") + w;

                }
                return s;
            }
            
        }
    }

    public class InvoiceItemModel 
    {
        public InvoiceItemModel()
        { }
        public InvoiceItemModel(RefuelViewModel item)
        {
            this.RealAmount = Math.Round(item.RealAmount,0, MidpointRounding.AwayFromZero);
            this.Volume = Math.Round(item.Volume, 0, MidpointRounding.AwayFromZero);
            this.Weight =  Math.Round(item.Weight,0, MidpointRounding.AwayFromZero);
            this.Gallon = Math.Round((decimal)item.Gallon,0, MidpointRounding.AwayFromZero);
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