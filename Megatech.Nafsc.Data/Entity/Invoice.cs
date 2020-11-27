using System;
using System.Collections.Generic;
using System.Text;

namespace FMS.Data
{
    public class Invoice: BaseEntity
    {
        public int FlightId { get; set; }
        public int? PrimaryInvoiceId { get; set; }
        public Invoice PrimaryInvoice { get; set; }


        public string InvoiceNumber { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string TaxCode { get; set; }

        public string Address { get; set; }

        public decimal Volume { get; set; }

        public decimal Weight { get; set; }

        public decimal Temperature { get; set; }

        public decimal Density { get; set; }
        
        public decimal Price { get; set; }

        public decimal TaxRate { get; set; }

        public decimal SubTotal
        {
            get
            {
                return Weight * Price;
            }
        }

        public decimal Tax
        {
            get { return TaxRate * SubTotal; }
        }

        public decimal TotalAmount
        {
            get { return SubTotal + Tax; }
        }

    }

    public class InvoiceItem:BaseEntity
    {
        public int RefuelId { get; set; }

        public decimal StartNumner { get; set; }

        public decimal EndNumber { get; set; }

        public decimal Volume { get; set; }
    }
}
