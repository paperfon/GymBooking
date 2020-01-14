using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBooking.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
    }
}
