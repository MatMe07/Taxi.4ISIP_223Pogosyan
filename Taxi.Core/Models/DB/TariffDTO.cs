using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.DB
{
    public class TariffDTO
    {
        public int TariffId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public string ClassPathVehicle { get; set; }
        public decimal BasePrice { get; set; }

        public decimal TotalPrice { get; set; }
        public double TotalTime { get; set; }
        public string DisplayTotalTime {
            get{
                var tim = TimeSpan.FromSeconds(TotalTime);
                return $"{(tim.Days > 0 ? $"{tim.Days} дн " : "")}" +
                    $"{(tim.Hours > 0 ? $"{tim.Hours} ч " : "")}" +
                    $"{(tim.Minutes > 0 ? $"{tim.Minutes} мин" : "")}";
            
            }
        }
            

        public decimal PricePerKm { get; set; }

        public decimal PricePerMinute { get; set; }

        public DateOnly ValidFrom { get; set; }

        public DateOnly? ValidTo { get; set; }

        public bool? IsActive { get; set; }
    }
}
