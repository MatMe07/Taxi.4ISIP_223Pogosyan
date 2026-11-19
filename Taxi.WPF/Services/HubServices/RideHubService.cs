using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.SharedHub;

namespace Taxi.WPF.Services.HubServices
{
    public class RideHubService 
    {
        private HubConnection _hubConnection;

        public event Action<string>? OnMessageReceived;

        public RideHubService(string hubUrl)
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).WithStatefulReconnect().Build();
            _hubConnection.On<string>("", (message) =>
            {

            });
            _hubConnection.On<string>("", (message) =>
            {

            });
        }
    }
}
