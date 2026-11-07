using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Map
{
    public class TariffRequest
    {
        public bool IsRoute { get; set; } = false;
        public RouteInfo routeInfo { get; set; }
    }
}
