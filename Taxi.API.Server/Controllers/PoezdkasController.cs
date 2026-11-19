using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taxi.API.Server.Services;
using Taxi.Core.Models;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/poezdka")]
    [Authorize]
    public class PoezdkasController : ControllerBase
    {

        private PoesdkaService poesdkaService;

        public PoezdkasController(PoesdkaService service)
        {
            poesdkaService = service;
        }

        [HttpGet("getAllPoezdka")]
        public async Task<IActionResult> GetAllPoezdkasAsync()
        {
            List<PoezdkaDTO> lst = await poesdkaService.GetAllPoezdkasAsync();
            return Ok(lst);
        }

        [HttpGet("tryToConnect")]
        public async Task<IActionResult> tryToConnect()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var response = await poesdkaService.TryToConnectAsync(userId);
                return Ok(response);
            }
            return BadRequest();
        }




    }
}
