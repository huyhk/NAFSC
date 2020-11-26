using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FMS.Data
{
    public class Airport:BaseEntity
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }

        public string Address { get; set; }

        public ICollection<User> Users { get; set; }

        //public BRANCHES Branch { get; set; }
    }

    public enum BRANCHES
    {
        MB,
        MT,
        MN
    }
}
