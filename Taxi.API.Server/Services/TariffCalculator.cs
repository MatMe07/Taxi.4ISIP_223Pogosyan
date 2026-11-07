using Microsoft.Data.SqlClient;
using Taxi.Core.Models.DB;
using Taxi.Core.Models.Map;

namespace Taxi.API.Server.Services
{
    public class TariffCalculator
    {


        public decimal Calculate(double durSeconds, double durMeters, bool isRoute, decimal BasePrice, decimal PricePerKm, decimal PricePerMinute)
        {
            if (!isRoute || durMeters ==-1)
            {
                return BasePrice;
            }
                
            decimal durMin = (decimal)durSeconds / 60.0m;
            decimal durKm = (decimal)durMeters / 1000.0m;
            return BasePrice + (durMin * PricePerMinute) + (durKm * PricePerKm);
        }
    }
}
