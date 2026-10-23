using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.DB
{
    public class PoezdkaDTO
    {
        public int OrderId { get; set; }

        public int DriverId { get; set; }

        public int VehicleId { get; set; }

        public string VehicleGosNumber { get; set; }
        
        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public decimal DriverEarning { get; set; }

        public decimal OrderEarning { get; set; }

        public decimal Earnings => OrderEarning - DriverEarning;



    }
}
