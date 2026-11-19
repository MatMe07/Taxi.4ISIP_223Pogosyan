using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.SharedHub;

namespace Taxi.WPF.Services.HubServices
{
    public  class HubConnect : IAsyncDisposable
    {
        private HubConnection _hubConnection;
        public IRideContract rideContract { get;  }
        public event Action<string>? OnMessageReceived;

        public HubConnect(string hubUrl)
        {
            _hubConnection = new HubConnectionBuilder().WithUrl($"{hubUrl.TrimEnd('/')}/hubs/ride").WithAutomaticReconnect().Build();

            //rideContract = _hubConnection.CreateHubProxy<IRideContract>();
            _hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine($"[Клиент получил]: {message}");
                OnMessageReceived?.Invoke(message);
            });
            _hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine($"[Клиент получил]: {message}");
                OnMessageReceived?.Invoke(message);
            });
        }

        public async Task SendMessageAsync(string message)
        {
            await _hubConnection.InvokeAsync("SendMessage", message);
        }

        public async Task StartAsync()
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                Debug.WriteLine("CONNECTTING!!!");
            }
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
