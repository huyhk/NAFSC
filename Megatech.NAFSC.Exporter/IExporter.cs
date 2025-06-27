using Megatech.NAFSC.DataExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.Exporter
{
    public interface IExporter
    {
        IEnumerable<ExportResult> Export();
        ExportResult Export(int id);

        string VendorModelCode { get; set; }
    }
}
