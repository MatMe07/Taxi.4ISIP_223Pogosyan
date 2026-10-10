using dotenv.net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.Core.Models.Map;

namespace Taxi.src.Taxi.Map.Services
{
    public class GeocodeService
    {
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);


        private readonly HttpClient _http;
        private readonly WpfMemoryCache _cache;

        public GeocodeService(HttpClient http)
        {
            _http = http;
            _cache = AppCache.Cache;
        }

        private async Task<RoutePoint> GetRoutePointAsync(string address)
        {
            string url = $"/geocode?address={address}";
            var point = await _http.GetFromJsonAsync<RoutePoint>(url);
            return point;
        }

        public async Task<RoutePoint> GeocodeAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            if (_cache.TryGet(address.ToLowerInvariant(), out RoutePoint cachedResult))
            {
                Debug.WriteLine("Использовал кэш");
                return cachedResult;
            }

            var result = await GetRoutePointAsync(address);

            _cache.Set(
                key: address.ToLowerInvariant(),
                value: result,
                CacheExpiration
            );
            Debug.WriteLine($"Кэшировал - {result}");

            return result;

        }

    }
}
