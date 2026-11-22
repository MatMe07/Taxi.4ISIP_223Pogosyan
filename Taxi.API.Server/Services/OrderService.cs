using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Taxi.API.Server.Hubs;
using Taxi.API.Server.Models;
using Taxi.Core.Models.Order;
using Taxi.Core.SharedHub;

namespace Taxi.API.Server.Services
{
    public class OrderService : BaseService
    {
        private readonly IHubContext<RideHub, IRideContract> _hubContext;
        public OrderService(TaxiBaseContext context, IHubContext<RideHub, IRideContract> hubContext) : base(context)
        {
            _hubContext = hubContext;
        }

        public async Task SendDriversNewOrder(OrderRequest orderRequest, int orderId, string clientName, string paymentMethodName, decimal rating, string tariffName)
        {
            var driverRequest = new GetDriverRequest
            {
                OrderId = orderId,
                ClientName = clientName,
                PaymentMethodName = paymentMethodName,
                Rating = rating,
                TariffName = tariffName,
                orderRequest = orderRequest
            };
            await _hubContext.Clients.All.NewOrder(driverRequest);

        }

        public async Task<(bool, int?)> CreateNewOrderAsync(OrderRequest orderRequest)
        {
            
            var newOrder = new Order
            {
                ClientId = orderRequest.ClientId,
                StatusId = 1,
                TariffId = orderRequest.TariffId,
                OtkudaAddress = orderRequest.AddressOtkuda.Address,
                KudaAddress = orderRequest.AddressKuda.Address,
                PaymentMethodId= orderRequest.PaymentMethodId,
                PaymentAmount = orderRequest.PaymentAmount,
                CreatedAt = DateTime.Now,
            };
            try
            {
                await _context.Orders.AddAsync( newOrder );
                await _context.SaveChangesAsync();

            }
            catch ( Exception ex )
            {
                return (false, null);
            }
            var order = await _context.Orders.Include(p=>p.PaymentMethod).Include(t=>t.Tariff).ThenInclude(c=>c.Class).Include(c=>c.Client).ThenInclude(u=>u.User).FirstOrDefaultAsync(o => o.Id == newOrder.Id);
            var client = order.Client;
            await SendDriversNewOrder(orderRequest, order.Id, 
               client.User.FirstName, order.PaymentMethod.Name, client.Rating, order.Tariff.Class.Name);

            return (true, order.Id);
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o=>o.Id == orderId);
            if (order != null)
            {
                order.CancelledAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
