using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FMS.Data
{
    public class Product:BaseEntity
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
