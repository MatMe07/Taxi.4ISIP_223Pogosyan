using dotenv.net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using static System.Net.WebRequestMethods;

namespace Taxi.src.Taxi.Map.Services
{
    internal class RoutingService
    {
        static HttpClient httpClient;

        private string RoutingApiKey = "";
        private string RoutingPickPointApiKey = "";
        private string RoutingMapboxToken = "";
        private const string RoutingUrl = "https://api.routing.yandex.net/v1/route/";
        private const string RoutingPickPointUrl = "https://api.pickpoint.io/v2/route";
        private const string RoutingMapboxUrl = "https://api.mapbox.com/directions/v5/mapbox/cycling/";
        private const string RoutingMode = "driving";
        private readonly WpfMemoryCache _cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);


        public RoutingService(HttpClient http)
        {
            //DotEnv.Load()
            httpClient = http;
            _cache = AppCache.Cache;

            RoutingApiKey = Environment.GetEnvironmentVariable("RoutingApiKey");
            RoutingPickPointApiKey = Environment.GetEnvironmentVariable("RoutingPickPointApiKey");
            RoutingMapboxToken = Environment.GetEnvironmentVariable("RoutingMapboxToken");
        }

        

        public class LOC
        {
            public List<RoutePoint> locations {  get; set; }
            public string language { get; set; }
            public string units { get; set; }
            public string costing { get; set; }
        }
        public async Task<RouteInfo> GetRouteAsync(
            RoutePoint from,
            RoutePoint to,
            string mode = RoutingMode,
            bool usePickPoint = false)
        {
            if (!IsValidPoint(from) || !IsValidPoint(to))
                throw new ArgumentException("Не заданы обе точки маршрута");

            string waypoints = string.Join("|", FormatCoord(from), FormatCoord(to));
            string cacheKey = $"route:{waypoints}".ToLowerInvariant();

            if (_cache.TryGet(cacheKey, out RouteInfo cachedResult))
            {
                Debug.WriteLine($"Использовал кэш {cacheKey}");
                return cachedResult;
            }

            RouteInfo info = usePickPoint
                ? await BuildRouteViaPickPointAsync(from, to)
                : await BuildRouteViaStandardApiAsync(waypoints, mode);

            _cache.Set(
                key: cacheKey,
                value: info,
                CacheExpiration
            );

            Debug.WriteLine($"Кэшировал - {info}");
            Debug.WriteLine(
                $"Маршрут: {info.DistanceMeters} м, {info.DurationSeconds} с, точек геометрии: {info.Coordinates.Count}");

            return info;
        }

        private async Task<RouteInfo> BuildRouteViaPickPointAsync(RoutePoint from, RoutePoint to)
        {

            
            string coords = $"{ from.Lon.ToString(CultureInfo.InvariantCulture)},{from.Lat.ToString(CultureInfo.InvariantCulture)};" +
                    $"{to.Lon.ToString(CultureInfo.InvariantCulture)},{to.Lat.ToString(CultureInfo.InvariantCulture)}";

            string url = $"{RoutingMapboxUrl}{coords}?geometries=geojson&access_token={RoutingMapboxToken}";

            using var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"HTTP {(int)response.StatusCode}");

            var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            JsonElement root = result.RootElement;

            //if (!root.TryGetProperty("trip", out var route) ||
            //    !route.TryGetProperty("legs", out var legs) ||
            //    !legs[0].TryGetProperty("shape", out var shapes) ||
            //    !legs[0].TryGetProperty("summary", out var summary))
            if (!root.TryGetProperty("routes", out var route) ||
                !route[0].TryGetProperty("geometry", out var geometry) ||
                !geometry.TryGetProperty("coordinates", out var coordinates))
                throw new InvalidOperationException("Маршрут не найден");

            if (!double.TryParse(route[0].GetProperty("duration").ToString(), out double durSec))
                throw new InvalidOperationException("time Error");

            if (!double.TryParse(route[0].GetProperty("distance").ToString(), out double durMet))
                throw new InvalidOperationException("cost Error");

            Debug.WriteLine("cost = " + durSec);
            Debug.WriteLine("time = " + durMet);

            var info = new RouteInfo
            {
                DurationSeconds = durSec,
                DistanceMeters = durMet
            };

            var shapesToList = JsonSerializer.Deserialize<List<List<double>>>(coordinates);
            foreach (List<double> point in shapesToList)
            {
                double lat = point[1];
                double lon = point[0];

                int last = info.Coordinates.Count - 1;
                if (last >= 0 && info.Coordinates[last][0] == lat && info.Coordinates[last][1] == lon)
                    continue;

                info.Coordinates.Add(new[] { lat, lon });
            }
            //info.Coordinates = shapesToList;

            return info;

        }

        private async Task<RouteInfo> BuildRouteViaStandardApiAsync(string waypoints, string mode)
        {
            string url = RoutingUrl
                + "?apikey=" + Uri.EscapeDataString(RoutingApiKey)
                + "&waypoints=" + Uri.EscapeDataString(waypoints)
                + "&mode=" + Uri.EscapeDataString(mode);

            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {



                JsonElement root = doc.RootElement;

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"HTTP {(int)response.StatusCode}");

                if (!root.TryGetProperty("route", out var route) || !route.TryGetProperty("legs", out var legs))
                    throw new InvalidOperationException("Маршрут не найден");

                if (root.TryGetProperty("traffic_type", out var traffic))
                    Debug.WriteLine($"traffic_type: {traffic.GetString()}");

                var info = new RouteInfo();
                var list_arr = legs[0];
                var points2 = list_arr.GetProperty("steps").ToString();

                List<RouteSegment> segments = JsonSerializer.Deserialize<List<RouteSegment>>(points2);

                foreach (RouteSegment step in segments)
                {
                    info.DistanceMeters += step.Length;
                    info.DurationSeconds += step.Duration;

                    if (step.Polyline?.Points == null)
                        continue;

                    foreach (List<double> point in step.Polyline.Points)
                    {
                        if (point.Count < 2)
                            continue;

                        double lat = point[0];
                        double lon = point[1];

                        int last = info.Coordinates.Count - 1;
                        if (last >= 0 && info.Coordinates[last][0] == lat && info.Coordinates[last][1] == lon)
                            continue;

                        info.Coordinates.Add(new[] { lat, lon });
                    }
                }

                return info;
            }
        }


        public static bool IsValidPoint(RoutePoint point)
        {
            return point != null && (point.Lat != 0 || point.Lon != 0);
        }


        public static string FormatCoord(RoutePoint point)
        {
            return point.Lat.ToString(CultureInfo.InvariantCulture) + "," + point.Lon.ToString(CultureInfo.InvariantCulture);
        }

    }
}
