using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;
using Taxi.Core.Models.Map;

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

        [HttpPost("GetAllCurrentTariffs")]
        public async Task<IActionResult> GetAllCurrentTariffsAsync(TariffRequest tariffRequest)
        {
            return Ok( await _tariffService.GetAllCurrentTariffsAsync(tariffRequest.routeInfo, tariffRequest.IsRoute));
        }

    }

}
