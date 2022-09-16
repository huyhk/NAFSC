using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Data
{
    public class LocalRefuel:BaseLocalEntity
    {             
       

        public string Date { get; set; }

        public Guid InvoiceGuid { get; set; }

    }
}
