﻿using Megatech.Nafsc.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FMS.Data
{
    public class Invoice: BaseEntity
    {
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        public int? ParentId { get; set; }
        public Invoice ParentInvoice { get; set; }

        public int? ChildId{ get; set; }
        public Invoice ChildInvoice { get; set; }



        public string InvoiceNumber { get; set; }

        public int CustomerId { get; set; }

        public Airline Customer { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string TaxCode { get; set; }

        public string Address { get; set; }

        public decimal Volume { get; set; }

        public decimal Weight { get; set; }

        public decimal Gallon { get; set; }

        public decimal Temperature { get; set; }

        

        public decimal Density { get; set; }

        public string QualityNo { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public decimal TaxRate { get; set; }

        public DateTime?  RefuelTime { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public List<InvoiceItem> Items { get; set; }

        //public FLIGHT_CREATED_LOCATION? CreatedLocation { get; set; }

        public decimal SubTotal
        {
            get;
            set;
        }

        public decimal Tax
        {
            get;
            set;
        }

        public decimal TotalAmount
        {
            get { return SubTotal + Tax; }
        }

        public void AddItem(InvoiceItem invoiceItem)
        {
            if (this.Items == null)
                this.Items = new List<InvoiceItem>();
            this.Items.Add(invoiceItem);
        }

        public OilCompany Vendor { get; set; }

        [NotMapped]
        public int? VendorModelId { get { return Flight?.VendorModelId; } }

        [NotMapped]
        public VendorModel  VendorModel { get { return Flight?.VendorModel; } }
        [NotMapped]
        public string VendorModelCode { get { return VendorModel?.Code; } }

        public Currency Currency { get; set; }

        public Unit  Unit { get; set; }


        public int? ExportedResult { get; set; }
        public bool Exported { get { return (ExportedResult??-1) == 0; } }

        public DateTime? DateExported { get; set; }

        public string ExportError { get; set; }

        public string ImagePath { get; set; }

        public Guid? UniqueId { get; set; } = Guid.NewGuid();
    }

    public class InvoiceItem:BaseEntity
    {

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public decimal RealAmount { get; set; }
        public decimal Volume { get; set; }
        public decimal Weight { get; set; }
        public decimal Gallon { get; set; }
        public decimal Density { get; set; }
        public decimal Temperature { get; set; }

        public string TruckNo { get; set; }

        public decimal StartNumber { get; set; }
        public decimal EndNumber { get; set; }
    }
}
