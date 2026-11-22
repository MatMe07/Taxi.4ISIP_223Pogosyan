using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Map
{
    public class RouteData
    {
        public double[] StartPoint { get; set; }
        public double[] EndPoint { get; set; }

        public RouteInfo RouteInfo { get; set; }
    }
}
