using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.DB
{
    public class TariffDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public decimal BasePrice { get; set; }

        public decimal PricePerKm { get; set; }

        public decimal PricePerMinute { get; set; }

        public DateOnly ValidFrom { get; set; }

        public DateOnly ValidTo { get; set; }

        public bool? IsActive { get; set; }
    }
}
