using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Models
{
    public class InvoiceOption
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceNumber2 { get; set; }
        public bool Split { get; set; }
        public decimal SplitAmount { get; set; }
        public decimal MaxSplit { get; set; }
        
    }
}
