using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FMS.Data
{
    public class Truck : BaseEntity
    {
        public int? DeviceId { get; set; }

        public Device Device { get; set; }

        public string TabletId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public decimal MaxAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        public int? AirportId { get; set; }
         
        public string Unit { get; set; }
        public Airport CurrentAirport { get; set; }

        public string DeviceIP { get; set; }

        public string PrinterIP { get; set; }

        public string TabletSerial { get; set; }

        public string DeviceSerial { get; set; }

        //public int? CurrentDriverId { get; set; }
        //public virtual User CurrentDriver { get; set; }

        //public int? CurrentOperatorId { get; set; }
        //public virtual User CurrentOperator { get; set; }

    }
}
