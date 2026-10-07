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

        private const string RoutingMode = "driving";
        private readonly WpfMemoryCache _cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);


        public RoutingService(HttpClient http)
        {
            //DotEnv.Load()
            httpClient = http;
            _cache = AppCache.Cache;
        }

        private async Task<RouteInfo> GetRouteInfoAsync(RoutePoint from,
            RoutePoint to,
            string mode = RoutingMode,
            bool useMapbox = false)
        {
            string url = $"/getRoute?from_Lat={from.Lat}&from_Lon={from.Lon}&from_Address={from.Address}&" +
                $"to_Lat={to.Lat}&to_Lon={to.Lon}&to_Address={to.Address}&mode={mode}&useMapbox={useMapbox}";
            var pointInfo = await httpClient.GetFromJsonAsync<RouteInfo>(url);
            return pointInfo;
        }


        public async Task<RouteInfo> GetRouteAsync(
            RoutePoint from,
            RoutePoint to,
            string mode = RoutingMode,
            bool useMapbox = false)
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

            RouteInfo info = await GetRouteInfoAsync(from, to, mode, useMapbox);

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
