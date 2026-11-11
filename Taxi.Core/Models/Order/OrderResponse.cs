using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Order
{
    public class OrderResponse
    {
        public bool isCreated { get; set; }
        public int? orderId { get; set; }
    }
}
