using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBooking.Models
{
    public class IndexViewModel
    {
        public IEnumerable<GymClass> GymClasses { get; set; }
        public bool History { get; set; }
    }
}
