using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.DB
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public ClientProfileDTO Client { get; set; }

        public string Status { get; set; }

        public TariffDTO Tariff { get; set; }

        public string OtkudaAddress { get; set; }

        public string KudaAddress { get; set; }

        public string PaymentMethod { get; set; }

        public decimal PaymentAmount { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public string CancelReason { get; set; }

    }
}
