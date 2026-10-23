using Taxi.API.Server.Models;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Services
{
    public class PoesdkaService(TaxiBaseContext context)
    {
        private readonly TaxiBaseContext _context = context;


        public async Task<List<PoezdkaDTO>> GetAllPoezdkasAsync()
        {
            var lstClasses =
                _context.Poezdkas.Select(vc => new PoezdkaDTO
                {
                    OrderId = vc.OrderId,
                    DriverId = vc.DriverId,
                    VehicleId = vc.VehicleId,
                    VehicleGosNumber = vc.Vehicle.GovNumber,
                    StartedAt = vc.StartedAt,
                    FinishedAt = vc.FinishedAt,
                    DriverEarning= vc.DriverEarning,
                    VehicleClassName=vc.Vehicle.Class.Name,
                    OrderEarning=vc.Order.PaymentAmount
                }).ToList();
            ;
            return lstClasses;
        }
    }
}
