using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Models;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Services
{
    public class VehiclesServices(TaxiBaseContext context)
    {
        private readonly TaxiBaseContext _context = context;

        public async Task<List<VehicleDTO>> GetAllVehiclesAsync()
        {
            var lstVehiclesDTO = new List<VehicleDTO>();
            foreach(var v in _context.Vehicles)
            {
                lstVehiclesDTO.Add(new VehicleDTO {
                    Id=v.Id,
                    GovNumber=v.GovNumber,
                    Model=v.Model,
                        Brand= v.Brand,
                        ClassId=v.ClassId,
                        Year=v.Year,
                        IsActive=v.IsActive,
                });
            }

            return lstVehiclesDTO;
        }


    }
}
