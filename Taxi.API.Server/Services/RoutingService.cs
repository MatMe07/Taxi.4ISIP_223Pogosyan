using dotenv.net;
using Microsoft.AspNetCore.Mvc;
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
using Taxi.Core.Models.Map;
using static System.Net.WebRequestMethods;

namespace Taxi.API.Server.Services
{
    internal class RoutingService
    {
        static HttpClient httpClient;

        private string RoutingApiKey = "";
        private string RoutingMapboxToken = "";
        private const string RoutingUrl = "https://api.routing.yandex.net/v1/route/";
        private const string RoutingMapboxUrl = "https://api.mapbox.com/directions/v5/mapbox/cycling/";
        private const string RoutingMode = "driving";
        private readonly MemoryCache _cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);


        public RoutingService(HttpClient http)
        {
            //DotEnv.Load()
            httpClient = http;
            _cache = AppCache.Cache;

            RoutingApiKey = Environment.GetEnvironmentVariable("RoutingApiKey");
            RoutingMapboxToken = Environment.GetEnvironmentVariable("RoutingMapboxToken");
        }


        public async Task<RouteInfo> GetRouteAsync(
            double from_Lat,
            double from_Lon,
            string from_Address,
            double to_Lat,
            double to_Lon,
            string to_Address,
            string mode = RoutingMode,
            bool useMapbox = false)
        {
            var from = new RoutePoint { Lat = from_Lat, Address = from_Address, Lon = from_Lon };
            var to = new RoutePoint { Lat = to_Lat, Address = to_Address, Lon = to_Lon };
            if (!IsValidPoint(from) || !IsValidPoint(to))
                throw new ArgumentException("Не заданы обе точки маршрута");

            string waypoints = string.Join("|", FormatCoord(from), FormatCoord(to));
            string cacheKey = $"route:{waypoints}".ToLowerInvariant();

            if (_cache.TryGet(cacheKey, out RouteInfo cachedResult))
            {
                Debug.WriteLine($"Использовал кэш {cacheKey}");
                return cachedResult;
            }

            RouteInfo info = useMapbox
                ? await BuildRouteViaMapboxAsync(from, to)
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

        private async Task<RouteInfo> BuildRouteViaMapboxAsync(RoutePoint from, RoutePoint to)
        {
            from = new RoutePoint { Lat = from.Lat, Lon = from.Lon, Address = from.Address };
            to = new RoutePoint { Lat = to.Lat, Lon = to.Lon, Address = to.Address };
            //string coords = $"{ from.Lon.ToString(CultureInfo.InvariantCulture)},{from.Lat.ToString(CultureInfo.InvariantCulture)};" +
            //        $"{to.Lon.ToString(CultureInfo.InvariantCulture)},{to.Lat.ToString(CultureInfo.InvariantCulture)}";
            string coords = $"{FormatCoord(from)};{FormatCoord(to)}";
            string url = $"{RoutingMapboxUrl}{coords}?geometries=geojson&access_token={RoutingMapboxToken}";

            using var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"HTTP {(int)response.StatusCode}");

            var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            JsonElement root = result.RootElement;

            if (!root.TryGetProperty("routes", out var route) ||
                !route[0].TryGetProperty("geometry", out var geometry) ||
                !geometry.TryGetProperty("coordinates", out var coordinates))
                throw new InvalidOperationException("Маршрут не найден");
            var dur = route[0].GetProperty("duration").ToString();
            if (!double.TryParse(dur, CultureInfo.InvariantCulture, out double durSec))
                throw new InvalidOperationException("time Error");

            if (!double.TryParse(route[0].GetProperty("distance").ToString(), CultureInfo.InvariantCulture, out double durMet))
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

                        info.Coordinates.Add(new[] { point[0], point[1] });
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
