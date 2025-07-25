﻿using FMS.Data;
using Megatech.Nafsc.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Megatech.FMS.WebAPI.Models
{
    public class AirlineViewModel
    {
        public int Id { get; set; }
       

        public string Code { get; set; }
      
        public string Name { get; set; }

        public string TaxCode { get; set; }

        public string Address { get; set; }

        public decimal Price { get; set; }

        public Currency Currency { get; set; }

        public Vendor Vendor { get; set; }

        public int? VendorModelId { get; set; }
        public string VendorModelCode { get; set; }

        public Unit Unit { get; set; }

        public string ProductName { get; set; }

        public string InvoiceName { get; set; }

        public string InvoiceTaxCode { get; set; }

        public string InvoiceAddress { get; set; }

    }

    public enum Vendor
    {

        SKYPEC,
        PA,
        TAPETCO

    }

  
    
}