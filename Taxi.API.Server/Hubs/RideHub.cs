using Microsoft.AspNetCore.SignalR;
using Taxi.Core.SharedHub;

namespace Taxi.API.Server.Hubs
{
    public class RideHub : Hub<IRideContract>
    {
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
    }
}
