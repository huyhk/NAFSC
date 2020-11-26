using System;
using System.Collections.Generic;
using System.Text;

namespace FMS.Data
{
    public class Order:BaseEntity
    {
        public int FlightId { get; set; }
        public int VehicleId { get; set; }
        public int UserId { get; set; }


    }
}
