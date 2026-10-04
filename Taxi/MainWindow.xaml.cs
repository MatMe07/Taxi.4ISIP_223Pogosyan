using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using Taxi.src.Taxi.Map;
using Taxi.src.Taxi.Map.Helpers;
using Taxi.src.Taxi.Map.Services;
using dotenv.net;


namespace Taxi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string DefaultFromAddress = "Москва, Льва Толстого, 16";
        private string DefaultToAddress = "Москва, Льва Толстого, 10";
        private double DeliveryTariff = 20;
        private double MinimumCost = 500;

        private GeocodeService geocodeService;
        private WebViewBridge webViewBridge;
        private RoutingService routingService;

        private HttpClient httpClient = new HttpClient();

        private RoutePoint _from;
        private RoutePoint _to;

        public MainWindow()
        {
            DotEnv.Load();
            InitializeComponent();
            geocodeService = new GeocodeService(httpClient);
            webViewBridge = new WebViewBridge(MyWebView);
            routingService = new RoutingService(httpClient);
            _from = new RoutePoint();
            _to = new RoutePoint();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await AppCache.Cache.LoadAsync();

            await MyWebView.EnsureCoreWebView2Async(null);
            MyWebView.WebMessageReceived += WebView_WebMessageReceived;

            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html");
            Console.WriteLine(htmlPath);

            MyWebView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using (JsonDocument doc = JsonDocument.Parse(e.TryGetWebMessageAsString()))
            {
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("type", out JsonElement typeElement))
                    return;

                switch (typeElement.GetString())
                {
                    case "ready":
                        await SetDefaultFromAsync();
                        break;

                    case "destinationSelected":
                        await SelectKudaAsync(root.GetProperty("address").GetString());
                        break;
                }
            }
        }


        private double Calculate(double routeLengthKm)
        {
            return Math.Max(routeLengthKm * DeliveryTariff, MinimumCost);
        }



        private async Task SetDefaultFromAsync()
        {
            await SetDefaultPointAsync("from", DefaultFromAddress);
            await SetDefaultPointAsync("to", DefaultToAddress);

            await UpdateRouteAsync();
        }

        private async Task SetDefaultPointAsync(string which, string address)
        {
            RoutePoint point = await geocodeService.GeocodeAsync(address);
            if (point == null)
            {
                Console.WriteLine("Точка по умолчанию не найдена: " + address);
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

            RouteInfo route = await routingService.GetRouteAsync(_from, _to);
            double price = Calculate(route.DistanceMeters / 1000.0);

            var args = string.Join(",",
                JsonSerializer.Serialize(route.Coordinates, JsHelper.JsonOptions),
                JsonSerializer.Serialize(new[] { _from.Lat, _from.Lon }, JsHelper.JsonOptions),
                JsonSerializer.Serialize(new[] { _to.Lat, _to.Lon }, JsHelper.JsonOptions));

            await webViewBridge.CallJsAsync("showRoute", args);

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            var pos = await GeolocationService.GetLocationAsync();
            double lat = pos.Coordinate.Point.Position.Latitude;
            double lon = pos.Coordinate.Point.Position.Longitude;

            if (lat == 0 && lon == 0)
            {
                await webViewBridge.ShowNoticeAsync("Местоположение недоступно");
                return;
            }



            await  webViewBridge.SetPointAsync("from", new RoutePoint
            {
                Lat = lat,
                Lon = lon,
                Address = "Моё местоположение"
            }, ref _from, ref _to);

            await UpdateRouteAsync();
        }

        private async Task SelectKudaAsync(string address)
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
                Console.WriteLine("Ошибка геокодера: " + ex);
            }
        }

        //public async Task PoiskAsync()
        //{
        //    var body = new { query = "москва хабар" };

        //    var request = new HttpRequestMessage(HttpMethod.Post, URL)
        //    {
        //        Content = new StringContent(
        //            JsonSerializer.Serialize(body),
        //            Encoding.UTF8,
        //            "application/json")
        //    };
        //    request.Headers.TryAddWithoutValidation("Accept", "application/json");
        //    request.Headers.TryAddWithoutValidation("Authorization", "Token " + token);

        //    var response = await httpClient.SendAsync(request);
        //    string responseText = await response.Content.ReadAsStringAsync();

        //    Console.WriteLine((int)response.StatusCode);
        //    Console.WriteLine(responseText);
        //}

        private async void Window_Closed(object sender, EventArgs e)
        {

            await AppCache.Cache.SaveAsync();
        }
    }
}