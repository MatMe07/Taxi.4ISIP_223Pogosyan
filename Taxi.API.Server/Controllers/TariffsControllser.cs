using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/tariff")]
    [Authorize]
    public class TariffsControllser : ControllerBase
    {
        private readonly TariffService _tariffService;
        public TariffsControllser(TariffService tariffService)
        {
            _tariffService = tariffService;
        }

        [HttpGet("GetAllCurrentTariffs")]
        public async Task<IActionResult> GetAllCurrentTariffsAsync()
        {
            return Ok( await _tariffService.GetAllCurrentTariffsAsync());
        }

    }

}
