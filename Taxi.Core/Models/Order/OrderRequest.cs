using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Map;

namespace Taxi.Core.Models.Order
{
    public class OrderRequest
    {
        public int ClientId { get; set; }
        public int TariffId { get; set; }
        public RoutePoint AddressOtkuda { get; set; }
        public RoutePoint AddressKuda { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal PaymentAmount { get; set; }

        public RouteInfo RouteInfo { get; set; }


    }
}
