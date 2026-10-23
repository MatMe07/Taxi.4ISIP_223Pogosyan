using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/poezdka")]
    public class PoezdkasController : ControllerBase
    {

        private PoesdkaService poesdkaService;

        public PoezdkasController(PoesdkaService service)
        {
            poesdkaService = service;
        }

        [HttpGet("getAllPoezdka")]
        [Authorize]
        public async Task<IActionResult> GetAllPoezdkasAsync()
        {
            List<PoezdkaDTO> lst = await poesdkaService.GetAllPoezdkasAsync();
            return Ok(lst);
        }




    }
}
