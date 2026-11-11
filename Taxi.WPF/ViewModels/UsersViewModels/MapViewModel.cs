using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Taxi.Core.Models.DB;
using Taxi.Core.Models.Map;
using Taxi.src.Taxi.Map.Helpers;
using Taxi.WPF.Models;
using Taxi.WPF.Services;

namespace Taxi.WPF.ViewModels.UsersViewModels
{
    public class MapViewModel : ViewModelBase
    {

        private readonly GeocodeService geocodeService;
        private readonly RoutingService routingService;
        private readonly TariffService tariffService;
        private readonly PaymentMethodService paymentMethodService;
        private readonly ProfileService profileService;
        private readonly OrderService orderService;
        private string TypeRoute;

        public Action<string, object> CallJsAsync {  get; set; }

        private RoutePoint _from;
        private RoutePoint _to;
        private RouteInfo _routeInfeFromTo;

        public WebView2 MyWebView {  get; set; }
        public MapViewModel(GeocodeService _geocodeService, RoutingService _routingService, 
            TariffService _tariffService, PaymentMethodService _paymentMethod, ProfileService _profileSerivce, OrderService orderService)
        {
            //MyWebView = webView;
            var baseAddress = Environment.GetEnvironmentVariable("BaseAddress");
            geocodeService = _geocodeService;
            routingService = _routingService;
            tariffService = _tariffService;
            paymentMethodService = _paymentMethod;
            profileService = _profileSerivce;
            this.orderService = orderService;
            TypeRoute = Environment.GetEnvironmentVariable("TypeRoute");
            _from = new RoutePoint();
            _to = new RoutePoint();
            _routeInfeFromTo = new();
            AppSettings.Instance.PropertyChanged += OnAppSettingsChanged; ;
        }

        private void OnAppSettingsChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.Instance.Theme))
            {
                CallJsAsync("changeTheme", new { Theme = AppSettings.Instance.Theme });
            }
        }

        public async Task ShowNoticeAsync(string message)
        {
            
            CallJsAsync("showNotice", new { Text = JsHelper.JsStr(message) });
        }

        public async Task<IEnumerable<TariffDTO>> GetTariffDTOs()
        {
            return await tariffService.GetAllTariffsAsync(_routeInfeFromTo, _to.Lat!= 0.0); 
        }

        private async Task UpdateTariff()
        {
            var tariffs_ = await GetTariffDTOs();
            var messageData = new
            {
                tariffs = tariffs_.ToList(),
                routeInfo = _routeInfeFromTo
            };
            CallJsAsync("updateTariff", messageData);

        }

        public async Task SetPointAsync(string which, RoutePoint point, bool addressName = false)
        {


            if (which == "from")
                _from = point;
            else
                _to = point;

            var tariffs_ = await GetTariffDTOs();
            
            var messageData = new
            {
                Which = which,
                Lat = point.Lat,
                Lon = point.Lon,
                Address = point.Address,
                tariffs = tariffs_.ToList(),
                routeInfo = _routeInfeFromTo
            };
            //var jsArgs = $"{JsHelper.JsStr(which)},{JsHelper.JsNum(point.Lat)},{JsHelper.JsNum(point.Lon)},{JsHelper.JsStr(point.Address)}";
            CallJsAsync( "setPoint", messageData);
        }

        public async Task WebMessage(JsonElement root)
        {
            if (!root.TryGetProperty("type", out JsonElement typeElement))
                return;

            switch (typeElement.GetString())
            {
                case "ready":
                    //await viewModel.SetDefaultFromAsync();
                    await SentDefaultSettings();
                    break;

                case "destinationSelected":
                    await SelectPointAsync(root.GetProperty("address").GetString(), "to", root.GetProperty("uri").GetString());
                    break;


                case "destinationSelectedKuda":
                    await SelectPointAsync(root.GetProperty("address").GetString(), "to", root.GetProperty("uri").GetString());
                    break;


                case "destinationSelectedOtkuda":
                    await SelectPointAsync(root.GetProperty("address").GetString() ?? "", "from", root.GetProperty("uri").GetString());
                    break;

                case "destinationOtkudaCoord":
                    await SelectPointAsync(root.GetProperty("address_coord").Deserialize<List<double>>() ?? [], "from", "", IsCoord: true);
                    break;

                case "changePaymentMethod":
                    string rawValue = root.GetProperty("selectedPaymentMethodId").GetString();

                    if (int.TryParse(rawValue, out int paymentMethodId))
                    {
                        await profileService.ChangePaymentMethod(AppSettings.Instance.userSession.User.ClientProfile?.Id ?? 0, paymentMethodId);
                    }
                    break;
                case "getUserLocation":
                    //await UserCurrentPointAsync();
                    break;
                case "createNewOrder":
                    string tariff = root.GetProperty("selectTariff").GetString();
                    string payment = root.GetProperty("paymentId").GetString();
                    string paymentAmountStr = root.GetProperty("paymentAmount").GetString();
                    if (int.TryParse(tariff, out int tariffId) &&
                        int.TryParse(payment, out int paymentId) &&
                        decimal.TryParse(paymentAmountStr, out decimal paymentAmount))
                    {
                        await CreateNewOrder(tariffId, paymentId, paymentAmount);
                    }
                    break;
                case "cancelOrder":
                    string orderIdStr = root.GetProperty("orderId").GetString();
                    if (int.TryParse(orderIdStr, out int orderId))
                        await CancelOrderAsync(orderId);
                    break;
                case "updateTariffs":
                    await UpdateTariff();
                    break;
            }
            //await UpdateRouteAsync();
        }



        //public async Task SetDefaultFromAsync()
        //{
        //    await SetDefaultPointAsync("from", DefaultFromAddress);
        //    await SetDefaultPointAsync("to", DefaultToAddress);

        //    await UpdateRouteAsync();
        //}

        //public async Task SetDefaultPointAsync(string which, string address)
        //{
        //RoutePoint point = await geocodeService.GeocodeAsync(address);
        //    if (point == null)
        //    {
        //        Debug.WriteLine("Точка по умолчанию не найдена: " + address);
        //        return;
        //    }

        //    await SetPointAsync(which, point, ref _from, ref _to);

        //}
        private async Task CancelOrderAsync(int orderId)
        {
            await orderService.CancelOrderAsync(orderId);
        }

        private async Task CreateNewOrder(int tariffId, int paymentId, decimal paymentAmount)
        {
            var (isCreated, orderId) = await orderService.CreateNewOrderAsync(
                AppSettings.Instance.userSession.User.ClientProfile.Id,
                tariffId,
                _from.Address,
                _to.Address,
                paymentId,
                paymentAmount
                );
            if ( isCreated )
            {
                var messageData = new
                {
                    isCreated,
                    orderId
                };
                CallJsAsync("GoToWaitingDriver", messageData);
            }

        }
        private async Task SentDefaultSettings()
        {
            var allPaymentsMethod = await paymentMethodService.GetPaymentMethods();
            var dataMess = new
            {
                allPaymentsMethod = allPaymentsMethod.ToList(),
                //coutnPaymet = allPaymentsMethod.Count(),
                selectPaymentMethod = AppSettings.Instance.userSession.User.ClientProfile?.DefaultPaymentMethodId,
            };
            CallJsAsync("SetDefaultSet", dataMess);
        }

        private async Task UpdateRouteAsync()
        {
            if (!RoutingService.IsValidPoint(_from) || !RoutingService.IsValidPoint(_to))
            {
                await ShowNoticeAsync("Сначала выберите обе точки");

                return;
            }
            _routeInfeFromTo = new RouteInfo();
            switch (TypeRoute)
            {
                case "Yandex":
                    {
                        _routeInfeFromTo = await routingService.GetRouteAsync(_from, _to);

                        break;
                    }
                case "Mapbox":
                    {
                        _routeInfeFromTo = await routingService.GetRouteAsync(_from, _to, useMapbox: true);
                        break;
                    }
            }

            var tariffs_ = await GetTariffDTOs();
            var routeData = new
            {
                _routeInfeFromTo.Coordinates,     
                StartPoint = new[] { _from.Lat, _from.Lon },
                EndPoint = new[] { _to.Lat, _to.Lon } ,
                tariffs = tariffs_.ToList(),
                routeInfo = _routeInfeFromTo
            };



            CallJsAsync("showRoute", routeData);
            //Debug.WriteLine($"showRoute = {routeData}");

        }

        //public async Task UserCurrentPointAsync()
        //{
        //    Debug.WriteLine("hello________________________________");
        //    var pos = await GeolocationService.GetLocationAsync();
        //    double lat = pos.Coordinate.Point.Position.Latitude;
        //    double lon = pos.Coordinate.Point.Position.Longitude;

        //    if (lat == 0 && lon == 0)
        //    {
        //        await ShowNoticeAsync("Местоположение недоступно");
        //        return;
        //    }
        //    RoutePoint pointUser = await geocodeService.GeocodeAddressAsync(new List<double>
        //    {
        //        lat,lon
        //    });
        //    await SetPointAsync("from", pointUser, true);

        //    await UpdateRouteAsync();
        //}


        public async Task SelectPointAsync(object address, string napravlenie, string uri, bool IsCoord = false)
        {
            try
            {
                RoutePoint point = await geocodeService.GeocodeAddressAsync(address, uri);
                if (point == null)
                {
                    await ShowNoticeAsync("Адрес не найден");
                    return;
                }

                await SetPointAsync(napravlenie, point, IsCoord);

                //await 

                await UpdateRouteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка геокодера: " + ex);
            }
        }





    }

}