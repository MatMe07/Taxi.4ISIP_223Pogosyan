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

        [HttpPut("updateVehicle")]
        [Authorize]
        public IActionResult UpdateVehicle(VehicleDTO vehicle)
        {
            return Ok(vehicle);
        }


        [HttpPost("addVehicle")]
        [Authorize]
        public IActionResult AddVehicle(VehicleDTO vehicle)
        {
            return Ok(vehicle);
        }


        [HttpDelete("deleteVehicle")]
        [Authorize]
        public IActionResult DeleteVehicle(VehicleDTO vehicle)
        {
            return Ok(vehicle);
        }
    }
}
