using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Taxi.Core.Models.Map;
using Taxi.Core.Models.Order;
using Taxi.Core.SharedHub;
using Taxi.src.Taxi.Map.Helpers;
using Taxi.WPF.Models;
using Taxi.WPF.Services;
using Taxi.WPF.Services.HubServices;
using Windows.ApplicationModel.Activation;

namespace Taxi.WPF.ViewModels.UsersViewModels
{
    public class DriverViewModel : ViewModelBase, IDriverContract
    {
        private readonly RideHubService hubConnect;
        private readonly PoezdkaService poezdkaService;
        private readonly GeocodeService geocodeService;
        private readonly RoutingService routingService;

        private RoutePoint _driverAddress = new();
        private RouteInfo _routeInfoDriverToClent = new();
        private GetDriverRequest _request;
        private string TypeRoute;

        public Action<string, object> CallJsAsync { get; set; }

        public DriverViewModel(GeocodeService _geocodeService, RoutingService _routingService, RideHubService hubConnect, PoezdkaService poezdkaService) {
            this.hubConnect = hubConnect;
            this.poezdkaService = poezdkaService; 
            geocodeService = _geocodeService;
            routingService = _routingService;
            AppSettings.Instance.PropertyChanged += OnAppSettingsChanged;
            hubConnect.OnMessageReceived += (message) =>
            {
                Debug.WriteLine($"uS: {message}");
            };
            TypeRoute = Environment.GetEnvironmentVariable("TypeRoute");


            hubConnect.BindingAllMethodsInInterface<IDriverContract>(this);
            //this.hubConnect.Regi

        }

        private void OnAppSettingsChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.Instance.Theme))
            {
                CallJsAsync("changeTheme", new { Theme = AppSettings.Instance.Theme });
            }
        }

        //public async Task NewOrder(int )



        public async Task WebMessage(JsonElement root)
        {
            if (!root.TryGetProperty("type", out JsonElement typeElement))
                return;

            switch (typeElement.GetString())
            {
                case "ready":
                    {
                        var response = await poezdkaService.TryToConnectAsync();
                        CallJsAsync("tryToOnline", response);
                        await hubConnect.StartAsync();

                        break;
                    }
                case "driverAddressCoord":
                    {
                        await SelectPointAsync(root.GetProperty("address_coord").Deserialize<List<double>>() ?? [], "", IsCoord: true);

                        break;
                    }
                case "driverToOnline":
                    {
                        if (bool.TryParse( root.GetProperty("status").GetString(), out bool status))
                        {

                            try
                            {
                                //await hubConnect.StartAsync();

                                //await hubConnect.SendMessageAsync("message");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Ошибка подключения к серверу: {ex.Message}");
                            }

                        }
                        break;
                    }
                case "takeOrder":
                    {

                        await hubConnect.takeOrder(_request.OrderId);
                        break;
                    }
            }
        }

        private async Task UpdateRouteAsync()
        {
            var _from = _driverAddress;
            var _to = _request.orderRequest.AddressOtkuda;
            switch (TypeRoute)
            {
                case "Yandex":
                    {
                        _routeInfoDriverToClent = await routingService.GetRouteAsync(_from, _to);

                        break;
                    }
                case "Mapbox":
                    {
                        _routeInfoDriverToClent = await routingService.GetRouteAsync(_from, _to, useMapbox: true);
                        break;
                    }
            }

        }


        public async Task NewOrder(GetDriverRequest request)
        {
            _request = request;
            string js = JsonSerializer.Serialize(request, JsHelper.JsonOptions);
            Debug.WriteLine(js);
            await UpdateRouteAsync();

            var messageData = new
            {
                request,
                routeDriverToClent = _routeInfoDriverToClent
            };


            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CallJsAsync("newOrder", messageData);
            });
            //CallJsAsync("newOrder", messageData);
        }
        public async Task SetPointAsync( RoutePoint point, bool addressName = false)
        {


            var messageData = new
            {
                point,
                //routeInfo = _routeInfoDriverToClent
            };
            //var jsArgs = $"{JsHelper.JsStr(which)},{JsHelper.JsNum(point.Lat)},{JsHelper.JsNum(point.Lon)},{JsHelper.JsStr(point.Address)}";
            CallJsAsync("setPoint", messageData);
        }

        public async Task SelectPointAsync(object address, string uri, bool IsCoord = false)
        {
            try
            {
                RoutePoint point = await geocodeService.GeocodeAddressAsync(address, uri);
                //if (point == null)
                //{
                //    await ShowNoticeAsync("Адрес не найден");
                //    return;
                //}
                _driverAddress = point;

                await SetPointAsync(point, IsCoord);

                //await UpdateRouteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка геокодера: " + ex);
            }
        }

        public Task JoinOrderGroup(string orderId)
        {
            throw new NotImplementedException();
        }
    }
}
