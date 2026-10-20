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
    }
}
