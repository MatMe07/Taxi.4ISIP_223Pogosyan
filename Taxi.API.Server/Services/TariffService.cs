using Microsoft.EntityFrameworkCore;
using Taxi.API.Server.Models;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Services
{
    public class TariffService
    {
        private readonly TaxiBaseContext _context;
        public TariffService(TaxiBaseContext context)
        {
            _context = context;
        }

        public async Task<List<TariffDTO>> GetAllCurrentTariffsAsync()
        {
            var tariffs = _context.Tariffs.Include(c => c.ClassId).Select(
                t =>
                    new TariffDTO
                    {
                        ClassId = t.ClassId,
                        ClassName = t.Class.Name,
                         BasePrice = t.BasePrice,
                         IsActive = t.IsActive,
                         PricePerKm = t.PricePerKm,
                         PricePerMinute = t.PricePerMinute,
                         ValidFrom = t.ValidFrom,
                         ValidTo = t.ValidTo,
                    }
                ).ToList();

            return tariffs;
        }
    }
}
