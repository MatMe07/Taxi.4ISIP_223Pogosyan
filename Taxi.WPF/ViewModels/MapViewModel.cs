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
using Taxi.src.Taxi.Map.Helpers;
using Taxi.src.Taxi.Map.Services;

namespace Taxi.WPF.ViewModels
{
    internal class MapViewModel
    {
        private string DefaultFromAddress = "Москва, Льва Толстого, 16";
        private string DefaultToAddress = "Москва, Льва Толстого, 10";
        private double DeliveryTariff = 20;
        private double MinimumCost = 500;

        private GeocodeService geocodeService;
        private WebViewBridge webViewBridge;
        private RoutingService routingService;
        private string TypeRoute;
        private HttpClient httpClient;

        private RoutePoint _from;
        private RoutePoint _to;
        private WebView2 MyWebView;
        public MapViewModel(WebView2 webView)
        {
            MyWebView = webView;
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5274")
            };
            geocodeService = new GeocodeService(httpClient);
            webViewBridge = new WebViewBridge(MyWebView);
            routingService = new RoutingService(httpClient);
            TypeRoute = Environment.GetEnvironmentVariable("TypeRoute");
            _from = new RoutePoint();
            _to = new RoutePoint();
        }



        private double Calculate(double routeLengthKm)
        {
            return Math.Max(routeLengthKm * DeliveryTariff, MinimumCost);
        }



        public async Task SetDefaultFromAsync()
        {
            await SetDefaultPointAsync("from", DefaultFromAddress);
            await SetDefaultPointAsync("to", DefaultToAddress);

            await UpdateRouteAsync();
        }

        public async Task SetDefaultPointAsync(string which, string address)
        {
            RoutePoint point = await geocodeService.GeocodeAsync(address);
            if (point == null)
            {
                Debug.WriteLine("Точка по умолчанию не найдена: " + address);
                return;
            }

            await webViewBridge.SetPointAsync(which, point, ref _from, ref _to);

        }

        private async Task UpdateRouteAsync()
        {
            if (!RoutingService.IsValidPoint(_from) || !RoutingService.IsValidPoint(_to))
            {
                await webViewBridge.ShowNoticeAsync("Сначала выберите обе точки");
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

            await webViewBridge.CallJsAsync("showRoute", args);

        }

        public async void ButtonClickAsync()
        {
            Debug.WriteLine("hello________________________________");
            var pos = await GeolocationService.GetLocationAsync();
            double lat = pos.Coordinate.Point.Position.Latitude;
            double lon = pos.Coordinate.Point.Position.Longitude;

            if (lat == 0 && lon == 0)
            {
                await webViewBridge.ShowNoticeAsync("Местоположение недоступно");
                return;
            }

            await webViewBridge.SetPointAsync("from", new RoutePoint
            {
                Lat = lat,
                Lon = lon,
                Address = "Моё местоположение"
            }, ref _from, ref _to);

            await UpdateRouteAsync();
        }
        public async Task SelectKudaAsync(string address)
        {
            try
            {
                RoutePoint point = await geocodeService.GeocodeAsync(address);
                if (point == null)
                {
                    await webViewBridge.ShowNoticeAsync("Адрес не найден");
                    return;
                }

                await webViewBridge.SetPointAsync("to", point, ref _from, ref _to);
                await UpdateRouteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка геокодера: " + ex);
            }
        }

    }

}