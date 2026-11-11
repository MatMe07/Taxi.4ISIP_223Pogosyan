using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Order
{
    public class OrderRequest
    {
        public int ClientId { get; set; }
        public int TariffId { get; set; }
        public string AddressOtkuda { get; set; }
        public string AddressKuda { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal PaymentAmount { get; set; }

    }
}
