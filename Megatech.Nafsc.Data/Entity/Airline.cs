using System;
using System.Collections.Generic;
using System.Text;

namespace FMS.Data
{
    public class Airline: Company
    {
        public IList<Aircraft> Aircrafts { get; set; }
        public IList<Flight> Flights { get; set; }

        //public int FlightCodePattern { get; set; }

        
        
    }
}
