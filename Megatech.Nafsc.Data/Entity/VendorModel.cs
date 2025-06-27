using FMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.Nafsc.Data.Entity
{
    [Table("OilCompanyTemplates")]
    public class VendorModel:BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
