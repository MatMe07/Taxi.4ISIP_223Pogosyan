using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Taxi.API.Server.Models;
using Taxi.Core.Models;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Services
{
    public class PoesdkaService : BaseService

    {
        public PoesdkaService(TaxiBaseContext context) : base(context)
        {

        }

        public async Task<RideResponse> TryToConnectAsync(int userID)
        {
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userID);
            if (driver == null) return null;
            Debug.WriteLine(driver.Id);
            var waybill = await _context.Waybills.FirstOrDefaultAsync(w => w.DriverId == driver.Id);
            if (waybill == null || waybill?.ReturnTime != null)
                return new RideResponse
            {
                message = "Нет актуального путевого листа",
                isAllowed = false
            };
            DateTime endWaybill = waybill.StartedAt.Add(TimeSpan.FromHours(24)) ;
            if (DateTime.Now >= endWaybill)
            {
                waybill.ReturnTime = endWaybill;
                await _context.SaveChangesAsync();
                return new RideResponse
            {
                message = "Путевой лист истек. Создайте новый путевой лист",
                isAllowed = false
            };

            }
            
            if (!waybill.TechStateOk)
                return new RideResponse
            {
                message = "Не пройден техосмотр",
                isAllowed = false
            };
            if (waybill.MedcheckResult != "Допущен")
                return new RideResponse
            {
                message = "Не пройден медосмотр",
                isAllowed = false
            };
            return new RideResponse
            {
                message = "Ok",
                isAllowed = true
            };
        }

        public async Task<List<PoezdkaDTO>> GetAllPoezdkasAsync()
        {
            var lstClasses =
                _context.Poezdkas.Include(o=>o.Order).Select(vc => new PoezdkaDTO
                {
                    OrderId = vc.OrderId,
                    //DriverId = vc.Order.DriverId,
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
