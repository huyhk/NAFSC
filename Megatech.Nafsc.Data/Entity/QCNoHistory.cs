using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Data
{
    public class QCNoHistory: BaseEntity
    {
        public DateTime StartDate { get; set; }

        public string QCNo { get; set; }

        public int? ProductId { get; set; }

        public int? AirtportId { get; set; }

        public string FileUrl { get; set; }
    }
}
