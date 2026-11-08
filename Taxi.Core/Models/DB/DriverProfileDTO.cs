using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.DB
{
    public class DriverProfileDTO
    {
        public string PassportSeries { get; set; }

        public string PassportNumber { get; set; }

        public string LicenseNumber { get; set; }

        public string LicenseCategories { get; set; }

        public int ExperienceYears { get; set; }

        public DateOnly DateNaima { get; set; }

        public bool IsActive { get; set; }

        public decimal SalaryRate { get; set; }

        public decimal Rating { get; set; }
    }
}
