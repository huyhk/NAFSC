using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FMS.Data
{
    public class User: BaseEntity
    {

        public User()
        {
            Permission = USER_PERMISSION.NONE;
        }
        
        public string UserName { get; set; }
        [Required]
        //[StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        public string FullName { get; set; }

        //[Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime? LastLogin { get; set; }

        public int? AirportId { get; set; }

        public Airport Airport { get; set; }
        public string DisplayName { get; set; }

        public ICollection<Airport> Airports { get; set; }

        public USER_PERMISSION Permission { get; set; }
    }

    [Flags]
    public enum USER_PERMISSION
    {
        NONE,
        CREATE_REFUEL = 1,
        CREATE_EXTRACT = 2,
        CREATE_CUSTOMER = 4
    }
}
