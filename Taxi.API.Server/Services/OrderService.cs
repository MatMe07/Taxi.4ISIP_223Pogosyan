using Microsoft.EntityFrameworkCore;
using Taxi.API.Server.Models;

namespace Taxi.API.Server.Services
{
    public class OrderService : BaseService
    {
        public OrderService(TaxiBaseContext context) : base(context)
        {
        }

        public async Task<(bool, int?)> CreateNewOrderAsync(
            int clientId, int tariffId, string AddressOtkuda, string AddressKuda, int PaymentMethodId, decimal PaymentAmount
            )
        {
            
            var newOrder = new Order
            {
                ClientId = clientId,
                StatusId = 1,
                TariffId = tariffId,
                OtkudaAddress = AddressOtkuda,
                KudaAddress = AddressKuda,
                PaymentMethodId= PaymentMethodId,
                PaymentAmount = PaymentAmount,
                CreatedAt = DateTime.Now,
            };
            try
            {
                await _context.Orders.AddAsync( newOrder );
                await _context.SaveChangesAsync();
                //int id = await _context.Orders
                return (true, newOrder.Id);
            }
            catch ( Exception ex )
            {
                return (false, null);
            }
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
