using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Taxi.API.Server.Models;
using Taxi.Core.SharedHub;
using Taxi.Core.Models.DB;
using Taxi.API.Server.Services;

namespace Taxi.API.Server.Hubs
{
    public class RideHub : Hub<IRideContract> 
    {
        private readonly TaxiBaseContext _context;

        public RideHub(TaxiBaseContext context) 
        {
            _context = context;
        }
        
        public async Task ReceiveMessage(string message) {
            Console.WriteLine($"[Сервер получил]: {message}");
            await this.Clients.All.ReceiveMessage("ReceiveMessage", $"Привет от сервера! Вы прислали: '{message}'");
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Клиент подключился: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Клиент отключился: {Context.ConnectionId}");
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
    }
}
