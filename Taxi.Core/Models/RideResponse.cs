using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models
{
    public class RideResponse
    {
        public string message { get; set; }
        public bool isAllowed { get; set; }
    }
}
