using Microsoft.EntityFrameworkCore;
using Taxi.API.Server.Models;
using Taxi.Core.Models.DB;
using Taxi.Core.Models.Map;

namespace Taxi.API.Server.Services
{
    public class TariffService : BaseService
    {
        private readonly TariffCalculator _tariffCalculator;
        public TariffService(TaxiBaseContext context, TariffCalculator tariffCalculator):base(context)
        {
            _tariffCalculator = tariffCalculator;
        }

        public async Task<IEnumerable<TariffDTO>> GetAllCurrentTariffsAsync(RouteInfo routeInfo, bool isRoute)
        {
            
            var tariffs = _context.Tariffs.Select(
                t =>
                    new TariffDTO
                    {
                     TariffId= t.Id,   
                        ClassId = t.ClassId,
                        ClassPathVehicle = t.Class.PathVehicleClass,
                        ClassName = t.Class.Name,
                         BasePrice = t.BasePrice,
                         TotalTime = routeInfo.DurationSeconds,
                         IsActive = t.IsActive,
                         PricePerKm = t.PricePerKm,
                         PricePerMinute = t.PricePerMinute,
                         ValidFrom = t.ValidFrom,
                         ValidTo = t.ValidTo,
                    }
                ).ToList();

            foreach( var t in tariffs.ToList() )
            {
                //if (routeInfo.DistanceMeters == -1) t.TotalPrice = -1;
                t.TotalPrice = _tariffCalculator.Calculate(routeInfo.DurationSeconds,routeInfo.DistanceMeters, isRoute, t.BasePrice, t.PricePerKm, t.PricePerMinute);
                
            }

            return tariffs;
        }
    }
}
