using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Data
{
    public class ProductPrice: BaseEntity
    {

        public int? CustomerId { get; set; }

        public Airline Customer { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal Price { get; set; }
        
        public OilCompany OilCompany { get; set; }

        public Currency Currency { get; set; }

        public int? AgencyId { get; set; }
        public Agency Agency  { get; set; }

        public int? AirportId { get; set; }

        public Unit Unit { get; set; }

    }
    public enum OilCompany
    {
        SKYPEC,
        PA
    }
    public enum Currency
    {
        USD,
        VND
    }

    public enum Unit
    {
        KG,
        GALLON
    }

}
