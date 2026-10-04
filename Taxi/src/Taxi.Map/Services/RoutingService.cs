using dotenv.net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Taxi.src.Taxi.Map.Services
{
    internal class RoutingService
    {
        static HttpClient httpClient;

        private string RoutingApiKey = "";
        private const string RoutingUrl = "https://api.routing.yandex.net/v1/route/";
        private const string RoutingMode = "driving";
        private readonly WpfMemoryCache _cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);


        public RoutingService(HttpClient http)
        {
            //DotEnv.Load()
            httpClient = http;
            _cache = AppCache.Cache;

            RoutingApiKey = Environment.GetEnvironmentVariable("RoutingApiKey");
        }

        public async Task<RouteInfo> GetRouteAsync(RoutePoint from, RoutePoint to, string mode = RoutingMode)
        {
            if (!IsValidPoint(from) || !IsValidPoint(to))
                throw new ArgumentException("Не заданы обе точки маршрута");

            string waypoints = string.Join("|", FormatCoord(from), FormatCoord(to));


            if (_cache.TryGet($"route:{waypoints}", out RouteInfo cachedResult))
            {
                Console.WriteLine($"Использовал кэш route:{waypoints}");

                return cachedResult;
            }

            string url = RoutingUrl + "?apikey=" + Uri.EscapeDataString(RoutingApiKey) + "&waypoints=" + Uri.EscapeDataString(waypoints) + "&mode=" + Uri.EscapeDataString(mode);


            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("errors", out JsonElement errors))
                    throw new InvalidOperationException("Ошибки API");

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"HTTP {(int)response.StatusCode}");

                if (!root.TryGetProperty("route", out var route) || !route.TryGetProperty("legs", out var legs))
                    throw new InvalidOperationException("Маршрут не найден");

                if (root.TryGetProperty("traffic_type", out var traffic))
                    Console.WriteLine($"traffic_type: {traffic.GetString()}");

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

                var result = info;
                _cache.Set(
                    key: $"route:{waypoints}".ToLowerInvariant(),
                    value: info,
                    CacheExpiration
                );
                Console.WriteLine($"Кэшировал - {result.ToString()}");


                Console.WriteLine(
                    $"Маршрут: {info.DistanceMeters} м, {info.DurationSeconds} с, точек геометрии: {info.Coordinates.Count}");

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
