using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController:ControllerBase
    {

        private readonly ProfileService profileService;
        public ProfileController(ProfileService profileService)
        {
            this.profileService = profileService;
        }

        [HttpPatch("changePaymentMethod")]
        public async Task<IActionResult> ChangePaymentMethod([FromQuery] int idClientProfile, [FromQuery] int idPaymentMethod)
        {
            
            return Ok(await profileService.ChangePaymentMethod(idClientProfile, idPaymentMethod));
        }

    }
}
