using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Geolocation;

namespace Taxi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static HttpClient httpClient = new HttpClient();
        string token = "809be99a95fa7638881e09e8f6c4220e6e9cfd43";
        private const string URL = "https://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/address";
        public class RoutePoint
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
            public string Address { get; set; }
        }

        // В MainWindow
        private RoutePoint _from;
        private RoutePoint _to;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MyWebView.WebMessageReceived += WebView_WebMessageReceived;
            _from = new RoutePoint();
            _to = new RoutePoint();
        }

        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Получаем данные в формате строки или JSON
            string messageFromWeb = e.TryGetWebMessageAsString();

            await SelectKudaAsync(messageFromWeb);

        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await MyWebView.EnsureCoreWebView2Async(null);

            string htmlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html");

            MyWebView.CoreWebView2.Navigate($"file:///{htmlPath}");
            Console.WriteLine( htmlPath );

        }


        private async Task<Geoposition> GetLocationAsync()
        {
            var geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            return await geolocator.GetGeopositionAsync();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var pos = await GetLocationAsync();
            var lat = pos.Coordinate.Point.Position.Latitude;
            var lon = pos.Coordinate.Point.Position.Longitude;
            await MyWebView.CoreWebView2.ExecuteScriptAsync(
                $"setUserLocation({lat}, {lon})"
            );

            _from.Lat = lat;
            _from.Lon = lon;
            _from.Address = "Mine";

        }

        public async Task SelectKudaAsync(string adress)
        {
            string adressI = adress.Replace(", ", "").Replace(' ', '+');
            var req = await httpClient.GetAsync($"https://geocode-maps.yandex.ru/1.x/?apikey=5a39211b-dd9d-482d-abf7-95a52bd60110&geocode={adressI}&format=json");
            Console.WriteLine(req.StatusCode);
            if (!req.IsSuccessStatusCode)
            {
                Console.WriteLine("Error");
                return;
            }

            string responseText = await req.Content.ReadAsStringAsync();
            var content = JsonDocument.Parse(responseText).RootElement.GetProperty("response").GetProperty("GeoObjectCollection").GetProperty("featureMember");
            var obj = content[0].GetProperty("GeoObject");

            string[] lat_lon = obj.GetProperty("Point").GetProperty("pos").ToString().Split();
            double lon = double.Parse(lat_lon[0]);
            double lat = double.Parse(lat_lon[1]);

            //var fullAdress = obj.GetProperty("metaDataProperty").GetProperty("GeocoderMetaData").GetProperty("text");
            //string addrEscaped = fullAdress.ToString().Replace("\\", "\\\\").Replace("'", "\\'");
            //Console.WriteLine(fullAdress);
            string latStr = lat.ToString();
            string lonStr = lon.ToString();

            await MyWebView.CoreWebView2.ExecuteScriptAsync(

                $"showMarkC({latStr}, {lonStr}, '{adress}')"

            );
            _to.Lat = lat;
            _to.Lon = lon;
            _to.Address = adress;

            await MyWebView.CoreWebView2.ExecuteScriptAsync(
                $"buildRoute()"
                );
        }


        public async Task PoiskAsync()

        {
            var body = new { query = "москва хабар" };

            var request = new HttpRequestMessage(HttpMethod.Post, URL)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json")
            };
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("Authorization", "Token " + token);

            var response = await httpClient.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            Console.WriteLine((int)response.StatusCode);
            Console.WriteLine(responseText);
        }
    }
}
