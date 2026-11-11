using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Taxi.API.Server.Services;
using Taxi.Core.Models.Order;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("createNewOrder")]
        public async Task<IActionResult> PostCreateNewOrderAsync(OrderRequest orderRequest)
        {
            var (isCreated, orderId) = await _orderService.CreateNewOrderAsync(
                orderRequest.ClientId,
                orderRequest.TariffId,
                orderRequest.AddressOtkuda,
                orderRequest.AddressKuda,
                orderRequest.PaymentMethodId,
                orderRequest.PaymentAmount
                );
            return Ok(new OrderResponse
            {
                isCreated = isCreated,
                orderId = orderId
            });
        }

        [HttpPatch("closeOrder")]
        public async Task<IActionResult> PatchCancelOrderAsync([FromQuery] int orderId)
        {
            Debug.WriteLine(HttpContext.Request);
            await _orderService.CancelOrderAsync(orderId);
            return Ok();
        }
    }
}
