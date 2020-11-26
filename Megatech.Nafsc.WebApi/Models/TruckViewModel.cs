using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Megatech.FMS.WebAPI.Models
{
    public class TruckViewModel
    {

        public int Id { get; set; }
        public string Code { get; set; }
        public string TruckNo { get; set; }

        public decimal CurrentAmount { get; set; }

        public string TabletSerial { get; set; }

        public string DeviceSerial { get; set; }

        public string DeviceIP { get; set; }

        public string PrinterIP { get; set; }

        public int AirportId { get; set; }

        public string  AirportCode { get; set; }

        public bool AllowNewRefuel { get; set; }
    }
}