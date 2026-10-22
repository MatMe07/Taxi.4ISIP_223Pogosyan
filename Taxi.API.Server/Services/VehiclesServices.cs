using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var docsType = _context.VehicleDocumentTypes.ToList();

            var vehicles = _context.Vehicles.Include(v => v.Maintenances).Include(v => v.VehicleDocuments).ThenInclude(d=>d.DocType).ToList();
            foreach (var v in _context.Vehicles.Select(v=> new {
            v.Id,
            Class = v.Class.Name,
            v.GovNumber,
            v.Brand,
            v.Year,
            v.IsActive,
            v.Model,
            v.Maintenances,
            v.VehicleDocuments,
            DocTypes = v.VehicleDocuments.Where(v=>v.IsVerified).Select(d=>d.DocType)
            }))
            {
                var dop = v.DocTypes.Union(docsType).FirstOrDefault();
                var TO = v.Maintenances.FirstOrDefault();
                //string dopusk = "";
                //if (!v.IsActive) dopusk = "Архив";
                //else if (dop == null)
                //{
                //    if (!(TO?.IsDone ?? true))
                //    {
                //        dopusk = "На ТО";
                //    }
                //}
                //else
                //{
                //    dopusk = dop.Name;
                //}
                lstVehiclesDTO.Add(new VehicleDTO
                {
                    Id = v.Id,
                    GovNumber = v.GovNumber,
                    Model = v.Model,
                    Brand = v.Brand,
                    Year = v.Year,
                    ClassName = v.Class,
                    IsActive = v.IsActive,
                    //dopuskText = string.IsNullOrWhiteSpace(dopusk) ? "Готово" : dopusk,
                    HasActiveMaintenance = TO?.IsDone ?? false,
                    HasOsago = (dop?.Id ?? -1) == 3,
                    HasTechCheck= (dop?.Id ?? -1) == 4,
                    HasPermit= (dop?.Id ?? -1) == 5

                });
            }

            return lstVehiclesDTO;
        }


        public async Task<List<VehicleClassDTO>> GetAllVehicleClassesAsync()
        {
            var lstClasses =
                _context.VehicleClasses.Select(vc => new VehicleClassDTO
                {
                    Id = vc.Id,
                    Name = vc.Name,
                }).ToList();
                ;
            return lstClasses;
        }

        public async Task<(string, bool)> AddVehicleAsync(VehicleDTO veh)
        {
            var findVehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.GovNumber == veh.GovNumber);
            if (findVehicle != null) return ("Машина с такими номерами уже есть",  false);

            await _context.Vehicles.AddAsync(new Vehicle
            {
                Id = veh.Id,
                GovNumber = veh.GovNumber,
                Model = veh.Model,
                Brand = veh.Brand,
                ClassId = veh.ClassId,
                Year = veh.Year,
                IsActive = veh.IsActive,
            });
            await _context.SaveChangesAsync();
            return ("Успешно!", true);
        }

    }
}
