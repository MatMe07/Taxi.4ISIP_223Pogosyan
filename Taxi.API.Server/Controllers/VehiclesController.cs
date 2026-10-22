using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehiclesController : ControllerBase
    {

        private readonly VehiclesServices services;
        public VehiclesController(VehiclesServices services)
        {
            this.services = services;
        }


        [HttpGet("getAllVehicles")]
        [Authorize]
        public async Task<IActionResult> GetAllVehiclesAsync()
        {
            List<VehicleDTO> lst = await services.GetAllVehiclesAsync();
            return Ok(lst);
        }

        [HttpGet("getAllClasses")]
        [Authorize]
        public async Task<IActionResult> GetAllVehicleClassesAsync()
        {
            var lst = await services.GetAllVehicleClassesAsync();
            return Ok(lst);
        }

        [HttpPatch("updateVehicle")]
        [Authorize]
        public async Task<IActionResult> UpdateVehicle(VehicleDTO vehicle)
        {
            var (message, isUpdate) = await services.UpdateVehicleAsync(vehicle);
            if (!isUpdate)
            {
                return BadRequest(new VehicleUpdateResponse
                {
                    message = message,
                    IsUpdateded = isUpdate
                });
            }
            return Ok(new VehicleUpdateResponse
            {
                message = message,
                IsUpdateded = isUpdate
            });
        }


        [HttpPost("addVehicle")]
        [Authorize]
        public async Task<IActionResult> AddVehicleAsync(VehicleDTO vehicle)
        {
            var (message, isAdd) = await services.AddVehicleAsync(vehicle);
            if (!isAdd)
            {
                return BadRequest(new VehicleAddResponse
                {
                    message = message,
                    IsAdded = isAdd
                });
            }
            return Ok(new VehicleAddResponse
            {
                message=message,
                IsAdded=isAdd
            });
        }


        [HttpDelete("deleteVehicle/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicleAsync(int id)
        {
            var (message, isDelete) = await services.DeleteVehicleAsync(id);
            if (!isDelete)
            {
                return BadRequest(new VehicleDeleteResponse
                {
                    message = message,
                    IsDeleted = isDelete
                });
            }
            return Ok(new VehicleDeleteResponse
            {
                message = message,
                IsDeleted = isDelete
            });
        }
    }
}
