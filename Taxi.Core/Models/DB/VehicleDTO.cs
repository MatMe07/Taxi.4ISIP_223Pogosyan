using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.DB
{
    public class VehicleDTO
    {
        public int Id { get; set; }

        public string GovNumber { get; set; }

        public string Model { get; set; }

        public string Brand { get; set; }

        public int ClassId { get; set; }
        //public string ClassName { get; set; }

        public int Year { get; set; }

        public bool IsActive { get; set; }
    }
}
