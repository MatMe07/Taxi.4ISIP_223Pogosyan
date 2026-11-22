using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public List<IDisposable> _subscriptions = new();

        public RideHubService(string baseAddress)
        {
            var hubUrl = $"{baseAddress.TrimEnd('/')}/hubs/ride";

            _hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).WithStatefulReconnect().Build();
            //BindingAllMethodsInInterface<IRideContract>(this);
        }

        //public void AddNewFunc<T>(Func<T, Task> func)
        //{
        //    var funcName = func.Method.Name; 
        //    if (_subscriptions.TryGetValue(funcName, out var Sub))
        //    {
        //        Sub.Dispose();
        //    }

        //    var subscription = _hubConnection.On<T>(funcName, async (data) =>
        //    {
        //        await func(data);
        //    });

        //    _subscriptions.Add(funcName, subscription);
        //}


        public async Task GetDriver(int orderId)
        {
            await _hubConnection.InvokeAsync("GetDriver",  orderId);
        }




        public void BindingAllMethodsInInterface<TInterface>(TInterface realize) 
        {
            var methods = typeof(TInterface).GetMethods();
            //var df = typeof(TInterface);
            foreach(var method in methods)
            {
                var paramTypes = method.GetParameters().Select(p=>p.ParameterType).ToArray();
                var subscription = _hubConnection.On(method.Name, paramTypes, async args =>
                {
                    var result = method.Invoke(realize, args);
                    if (result is Task task)
                    {
                        await task;
                    }
                });
                _subscriptions.Add(subscription);
            }

        }



        public async Task StartAsync()
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }


    }
}
