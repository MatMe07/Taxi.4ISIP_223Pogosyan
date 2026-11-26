using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Taxi.API.Server.Hubs;
using Taxi.API.Server.Models;
using Taxi.Core.Models.Order;
using Taxi.Core.SharedHub;

namespace Taxi.API.Server.Services
{
    public class OrderService : BaseService
    {
        private readonly IHubContext<RideHub, IRideContract> _hubContext;
        private readonly IMemoryCache _cache;
        public OrderService(TaxiBaseContext context, IHubContext<RideHub, IRideContract> hubContext, IMemoryCache cache) : base(context)
        {
            _hubContext = hubContext;
            _cache = cache;
        }

        public int GetFreeDriver(int orderId)
        {
            //var driverIds =  _context.DriverProfiles.Where(dr=>dr.Orders.Select(o=>o.Poezdkas.Where(p=>p.FinishedAt != null)).Count() == 0).OrderBy(dr=>dr.Rating).Select(dr=>dr.Id).ToList();
            var driverIds = _context.DriverProfiles.Where(
                dr => dr.IsActive 
                && dr.Waybills.Any(w=>w.ReturnTime == null && w.StartedAt.AddHours(24)> DateTime.UtcNow)
                && !dr.Orders.Any(o => o.Poezdkas.Any(p=>p.FinishedAt==null) || o.StatusId == 2 || o.StatusId == 3)
            ).OrderByDescending(dr=>dr.Rating).Select(dr=>dr.Id);

            _cache.TryGetValue($"CancelingDrivers:{orderId}", out List<int> cancelingDrivers);

            var driverIdWithoutCanceling= driverIds.Union(cancelingDrivers ?? []);
            if (driverIdWithoutCanceling.Any())
            {
                return driverIdWithoutCanceling.First();
            }
            if (driverIds.Any()) 
                return driverIds.First();
            else return 0;
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
            _cache.Set($"order:{orderId}", driverRequest);
            int driverId = GetFreeDriver(orderId);
            var userId = await _context.Users.FirstOrDefaultAsync(u=>u.DriverProfile.Id == driverId);

            await _hubContext.Clients.User(userId.Id.ToString()).NewOrder(driverRequest);
            //await _hubContext.Groups.AddToGroupAsync()
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

        public async Task TakeOrderAsync(int orderId, int driverId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o=>o.Id == orderId);
            order.DriverId = driverId;
            await _context.SaveChangesAsync();
            _cache.TryGetValue($"order:{order.Id}", out var orderRequest);
            //await _hubContext.Clients.Client
        }
    }
}
