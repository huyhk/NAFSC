using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FMS.Data
{
    public class RefuelItem:BaseEntity
    {
        public RefuelItem()
        {
            RefuelItemType = REFUEL_ITEM_TYPE.REFUEL;
        }


        public int TruckId { get; set; }

        public Truck Truck { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? RefuelTime { get; set; }

        public decimal Amount { get; set; }

        public decimal Temperature { get; set; }

        public decimal StartNumber { get; set; }

        public decimal EndNumber { get; set; }

        public bool Completed { get; set; }

        public bool Printed { get; set; }

        

        public int? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public User Driver { get; set; }

        public int? OperatorId { get; set; }
        [ForeignKey("OperatorId")]
        public User Operator { get; set; }


        public DateTime? DeviceStartTime { get; set; }

        public DateTime? DeviceEndTime { get; set; }               

        //public int RefuelId { get; set; }
        //[ForeignKey("RefuelId")]
        //public virtual Refuel Refuel { get; set; }

        public REFUEL_ITEM_STATUS Status { get; set; }

        //public ITEM_PRINT_STATUS PrintStatus { get; set; }

        public decimal ManualTemperature { get; set; }
    
        public decimal Density { get; set; }

        public int FlightId { get; set; }
        [ForeignKey("FlightId")]
        public Flight Flight { get; set; }

        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public Unit Unit { get; set; }

        public string QCNo { get; set; }

        public decimal TaxRate { get; set; }

        public int? ApprovalUserId { get; set; }

        public ITEM_APPROVE_STATUS ApprovalStatus { get; set; }

        public string ApprovalNote { get; set; }

        public ITEM_CREATED_LOCATION CreatedLocation { get; set; }

        public REFUEL_ITEM_TYPE RefuelItemType { get; set; }

        
        public decimal? Volume {
            get;
            set;
        }

        public decimal? Volume15
        {
            get;
            set;
        }
        public decimal? Weight
        {
            get;set;
        }

        public decimal? Gallon { get; set; }
        
        public decimal? Extract { get; set; }



        public decimal SalesAmount
        {
            get
            {
                return (decimal)this.Weight * this.Price;
            }
        }
        public decimal VATAmount
        {
            get
            {
                return this.SalesAmount * this.TaxRate;
            }
        }

        public decimal TotalSalesAmount
        {
            get
            {
                return this.SalesAmount * (1+this.TaxRate);
            }
        }
        [NotMapped]
        public string InvoiceNumber
        {
            get { return this.Invoice == null ? "" : this.Invoice.InvoiceNumber; }
            private set { }
        }

        public int? InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

    }

    public enum REFUEL_ITEM_STATUS
    {
        [Description("Chưa tra nạp")]
        NONE,
        [Description("Đang tra nạp")]
        PROCESSING,
        [Description("Tạm ngưng tra nạp")]
        PAUSED,
        [Description("Đã tra nạp")]
        DONE,
        [Description("Tra nạp lỗi")]
        ERROR
            
    }

    public enum ITEM_PRINT_STATUS
    {
        NONE,
        SUCCESS,
        ERROR
    }

    public enum ITEM_APPROVE_STATUS
    {
        NONE,
        APPROVED,
        REJECTED
    }

    public enum ITEM_CREATED_LOCATION
    {
        IMPORT,
        WEB,
        COPY,
        APP
    }

    public enum REFUEL_ITEM_TYPE
    {
        REFUEL,
        EXTRACT,
        TEST
    }

    public enum REFUEL_UNIT
    {
        KG,
        GALLON
    }

    public enum REFUEL_CURRENCY
    {
        USD,
        VND
    }
}