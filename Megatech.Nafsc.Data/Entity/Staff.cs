using System;
using System.Collections.Generic;
using System.Text;

namespace FMS.Data
{
    public class Staff:BaseEntity
    {
        public string Name { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }


    }
}
