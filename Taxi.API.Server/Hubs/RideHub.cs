using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Taxi.API.Server.Models;
using Taxi.Core.SharedHub;
using Taxi.Core.Models.DB;
using Taxi.API.Server.Services;
using Microsoft.AspNetCore.Authorization;

namespace Taxi.API.Server.Hubs
{
    [Authorize]
    public class RideHub : Hub<IRideContract> 
    {
        private readonly TaxiBaseContext _context;
        private readonly ILogger<RideHub> _logger;

        public RideHub(TaxiBaseContext context, ILogger<RideHub> logger) 
        {
            _context = context;
            _logger = logger;
        }
        
        public override async Task OnConnectedAsync()
        {
            var userIdentifier = Context.UserIdentifier;
            var connectionId = Context.ConnectionId;
            var isAuthenticated = Context.User?.Identity?.IsAuthenticated ?? false;

            // 2. Все клеймы из токена/куки (поможет, если UserIdentifier оказался пустым)
            var claims = Context.User?.Claims
                .Select(c => $"{c.Type} = {c.Value}")
                .ToList() ?? new List<string>();

            _logger.LogInformation(
                "SignalR Connect | ConnectionId: {ConnectionId} | IsAuth: {IsAuth} | UserIdentifier: '{UserId}' | Claims: [{Claims}]",
                connectionId,
                isAuthenticated,
                userIdentifier ?? "NULL",
                string.Join(", ", claims)
            );

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation(
                "SignalR Disconnect | ConnectionId: {ConnectionId} | UserIdentifier: '{UserId}' | Error: {Error}",
                Context.ConnectionId,
                Context.UserIdentifier ?? "NULL",
                exception?.Message ?? "None"
            );

            await base.OnDisconnectedAsync(exception);
        }



        public async Task GetDriver(int orderId)
        {
            Console.WriteLine($"[Сервер получил]: {orderId}");
            //var order = await _context.Orders.FirstOrDefaultAsync(o=>o.Id ==  orderId);
            //var clientDto = new ClientProfileDTO
            //{
            //    Id = order.ClientId,
            //    DefaultPaymentMethodId = order.Client.DefaultPaymentMethodId,
            //    DefaultPaymentMethodName = order.Client.DefaultPaymentMethod.Name,
            //    Rating = order.Client.Rating,
            //    CreatedAt = order.Client.CreatedAt,
            //};
            //var tariffDto = new TariffDTO
            //{

            //};
            //await Clients.All.NewOrder(new OrderDTO
            //{
            //    Id  = orderId,
            //    Client = clientDto,
            //    Status = order.Status.Name,
            //    Tariff = tariffDto,
            //    OtkudaAddress = order.OtkudaAddress,
            //    KudaAddress = order.KudaAddress,
            //    PaymentMethod = order.PaymentMethod.Name,
            //    PaymentAmount = order.PaymentAmount,
            //    PaidAt = order.PaidAt,
            //    CreatedAt = order.CreatedAt,
            //    CancelledAt = order.CancelledAt,
            //    CancelReason   = order.CancelReason,
            //});
        }



        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        }
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        }
    }
}
