using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.DB
{
    public class ClientProfileDTO
    {
        public int Id { get; set; }

        public int? DefaultPaymentMethodId { get; set; }
        public string? DefaultPaymentMethodName { get; set; }

        public decimal Rating { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
