using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Map;

namespace Taxi.Core.Models.Order
{
    public class GetDriverRequest
    {
        public int OrderId { get; set; }
        public OrderRequest orderRequest { get; set; }

        public decimal Rating { get; set; }
        public string? PaymentMethodName { get; set; }
        public string TariffName { get; set; }
        public string ClientName { get; set; }


    }
}
