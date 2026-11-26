using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Map;

namespace Taxi.Core.Models.Order
{
    public class SendClientAcceptOrder
    {
        public int OrderId { get; set; }
        public RoutePoint AddressOtkuda { get; set; }
        public RouteInfo RouteInfoToClient { get; set; }
        public decimal RatingDriver { get; set; }
        public string DriverName { get; set; }

        public int DriverPoezdok { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleColor { get; set; }


    }
}
