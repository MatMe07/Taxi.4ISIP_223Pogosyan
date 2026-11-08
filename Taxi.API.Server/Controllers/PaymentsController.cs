using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentsService paymentsService;
        public PaymentsController(PaymentsService paymentsService)
        {
            this.paymentsService = paymentsService;
        }

        [HttpGet("allPaymentsMehtods")]
        public IActionResult GetallPaymentsMehtods()
        {
            return Ok(paymentsService.GetPaymentMethods());
        }
    }
}
