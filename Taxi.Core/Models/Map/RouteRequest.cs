using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Map
{
    public class RouteRequest
    {
        public RoutePoint from { get; set; }
        public RoutePoint to { get; set; }
        public bool useMapbox { get; set; } = false;
    }
}
