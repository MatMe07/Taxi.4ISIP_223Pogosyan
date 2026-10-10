using dotenv.net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.Core.Models.Map;

namespace Taxi.API.Server.Services
{
    public class GeocodeService
    {
        private string GeocodeApiKey = "";

        private string GeocodeUrl;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);


        private readonly HttpClient _http;
        private readonly MemoryCache _cache;

        public GeocodeService(HttpClient http)
        {
            _http = http;
            _cache = AppCache.Cache;

            GeocodeUrl = Environment.GetEnvironmentVariable("GeocodeUrl");
            GeocodeApiKey = Environment.GetEnvironmentVariable("GeocodeApiKey");
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

            string url = GeocodeUrl + "?apikey=" + GeocodeApiKey + "&geocode=" + Uri.EscapeDataString(address) + "&format=json&results=1";
            using (HttpResponseMessage response = await _http.GetAsync(url))
            using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                Debug.WriteLine((int)response.StatusCode);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Геокодер: HTTP " + (int)response.StatusCode);

                JsonElement members = doc.RootElement.GetProperty("response").GetProperty("GeoObjectCollection").GetProperty("featureMember");
                if (members.GetArrayLength() == 0)
                    return null;

                JsonElement obj = members[0].GetProperty("GeoObject");
                string[] pos = obj.GetProperty("Point").GetProperty("pos").GetString().Split(' ');

                var result = new RoutePoint
                {
                    Lat = double.Parse(pos[1], CultureInfo.InvariantCulture),
                    Lon = double.Parse(pos[0], CultureInfo.InvariantCulture),
                    Address = address
                };
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
}
