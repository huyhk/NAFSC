﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Megatech.FMS.WebAPI.Models
{
    public class PermissionViewModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public bool AllowNewRefuel { get; set; }
    }

    
}