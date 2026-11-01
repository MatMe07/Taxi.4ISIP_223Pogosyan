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
using Taxi.Core.Models.Map;
using Taxi.src.Taxi.Map.Helpers;
using Taxi.src.Services;

namespace Taxi.WPF.ViewModels.UsersViewModels
{
    public class MapViewModel : ViewModelBase
    {
        private string DefaultFromAddress = "Москва, Льва Толстого, 16";
        private string DefaultToAddress = "Москва, Льва Толстого, 10";
        private double DeliveryTariff = 20;
        private double MinimumCost = 500;

        private GeocodeService geocodeService;
        private RoutingService routingService;
        private string TypeRoute;
        private HttpClient httpClient;

        public Func<string, string, Task<string>> CallJsAsync {  get; set; }

        private RoutePoint _from;
        private RoutePoint _to;
        public WebView2 MyWebView {  get; set; }
        public MapViewModel(HttpClient http)
        {
            //MyWebView = webView;
            var baseAddress = Environment.GetEnvironmentVariable("BaseAddress");
            httpClient = http;
            geocodeService = new GeocodeService(httpClient);
            routingService = new RoutingService(httpClient);
            TypeRoute = Environment.GetEnvironmentVariable("TypeRoute");
            _from = new RoutePoint();
            _to = new RoutePoint();
        }

        public async Task ShowNoticeAsync(string message)
        {
            await CallJsAsync("showNotice", JsHelper.JsStr(message));
        }

        public async Task SetPointAsync(string which, RoutePoint point, bool addressName = false)
        {
            if (which == "from")
                _from = point;
            else
                _to = point;
            var jsArgs = $"{JsHelper.JsStr(which)},{JsHelper.JsNum(point.Lat)},{JsHelper.JsNum(point.Lon)},{JsHelper.JsStr(point.Address)}";
            await CallJsAsync( addressName? "setAddressOtkuda" : "setPoint", jsArgs);
        }


        private double Calculate(double routeLengthKm)
        {
            return Math.Max(routeLengthKm * DeliveryTariff, MinimumCost);
        }



        //public async Task SetDefaultFromAsync()
        //{
        //    await SetDefaultPointAsync("from", DefaultFromAddress);
        //    await SetDefaultPointAsync("to", DefaultToAddress);

        //    await UpdateRouteAsync();
        //}

        //public async Task SetDefaultPointAsync(string which, string address)
        //{
        //    RoutePoint point = await geocodeService.GeocodeAsync(address);
        //    if (point == null)
        //    {
        //        Debug.WriteLine("Точка по умолчанию не найдена: " + address);
        //        return;
        //    }

        //    await SetPointAsync(which, point, ref _from, ref _to);

        //}

        private async Task UpdateRouteAsync()
        {
            if (!RoutingService.IsValidPoint(_from) || !RoutingService.IsValidPoint(_to))
            {
                await ShowNoticeAsync("Сначала выберите обе точки");
                return;
            }
            RouteInfo route = new RouteInfo();
            switch (TypeRoute)
            {
                case "Yandex":
                    {
                        route = await routingService.GetRouteAsync(_from, _to);

                        break;
                    }
                case "Mapbox":
                    {
                        route = await routingService.GetRouteAsync(_from, _to, useMapbox: true);
                        break;
                    }
            }
            double price = Calculate(route.DistanceMeters / 1000.0);

            var args = string.Join(",",
                JsonSerializer.Serialize(route.Coordinates, JsHelper.JsonOptions),
                JsonSerializer.Serialize(new[] { _from.Lat, _from.Lon }, JsHelper.JsonOptions),
                JsonSerializer.Serialize(new[] { _to.Lat, _to.Lon }, JsHelper.JsonOptions));

            await CallJsAsync("showRoute", args);

        }

        public async void ButtonClickAsync()
        {
            Debug.WriteLine("hello________________________________");
            var pos = await GeolocationService.GetLocationAsync();
            double lat = pos.Coordinate.Point.Position.Latitude;
            double lon = pos.Coordinate.Point.Position.Longitude;

            if (lat == 0 && lon == 0)
            {
                await ShowNoticeAsync("Местоположение недоступно");
                return;
            }

            await SetPointAsync("from", new RoutePoint
            {
                Lat = lat,
                Lon = lon,
                Address = "Моё местоположение"
            });

            await UpdateRouteAsync();
        }


        public async Task SelectKudaAsync(object address, string napravlenie, bool IsCoord = false)
        {
            try
            {
                RoutePoint point = await geocodeService.GeocodeAddressAsync(address);
                if (point == null)
                {
                    await ShowNoticeAsync("Адрес не найден");
                    return;
                }

                await SetPointAsync(napravlenie, point, IsCoord);
                await UpdateRouteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка геокодера: " + ex);
            }
        }

    
        //public async Task SelectKudaAsync(List<double> address, string napravlenie, bool IsCoord = false)
        //{
        //    try
        //    {
        //        RoutePoint point = await geocodeService.GeocodeAddressAsync(address);
        //        if (point == null)
        //        {
        //            await ShowNoticeAsync("Адрес не найден");
        //            return;
        //        }

        //        await SetPointAsync(napravlenie, point);
        //        await UpdateRouteAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Ошибка геокодера: " + ex);
        //    }
        //}

    }

}