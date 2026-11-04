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
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.Services
{
    public class GeocodeService : IGeocodeService
    {
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);


        private readonly HttpClient _http;
        private readonly WpfMemoryCache _cache;

        public GeocodeService(HttpClient http)
        {
            _http = http;
            _cache = AppCache.Cache;
        }

        private async Task<RoutePoint> GetRoutePointAsync(string address, string uri)
        {
            string url = $"api/geo/GetAddressGeocode?address={address}&uri={uri}";
            var point = await _http.GetFromJsonAsync<RoutePoint>(url);
            return point;
        }

        private async Task<RoutePoint> GetAddressFromRoutePointAsync(RoutePoint address_coord)
        {
            //string stJs = JsonSerializer.Serialize(address_coord);
            //string url = $"api/geo/GetAddressFromGeocode?address={address_coord}";
            var point = await _http.PostAsJsonAsync("api/geo/GetAddressFromGeocode", address_coord);
            return point.Content.ReadFromJsonAsync<RoutePoint>().Result;
        }

        public async Task<RoutePoint> GeocodeAddressAsync(object addressOrCoord, string uri)
        {
            string address = "";
            bool coordIs = false;
            RoutePoint addresssPoint = new RoutePoint();
            if (addressOrCoord is string) address = addressOrCoord.ToString();
            else if (addressOrCoord is List<double> coords)
            {
                address = JsonSerializer.Serialize(coords);
                //$"{coords[1].ToString(CultureInfo.InvariantCulture)},{coords[0].ToString(CultureInfo.InvariantCulture)}";
                coordIs = true;
                addresssPoint.Lon = coords[0];
                addresssPoint.Lat = coords[1];
                //address = JsonSerializer.Serialize(addressOrCoord as List<double>);
            }

            if (string.IsNullOrWhiteSpace(address))
                return null;

            if (_cache.TryGet( coordIs ? "userGeo":  address.ToLowerInvariant(), out RoutePoint cachedResult, !coordIs))
            {
                Debug.WriteLine("[GeocodeAddressAsync:wpf]Использовал кэш");
                return cachedResult;
            }


             var result = coordIs ? await GetAddressFromRoutePointAsync(addresssPoint) : await GetRoutePointAsync(address, uri);

            _cache.Set(
                key: coordIs ? "userGeo" : address.ToLowerInvariant(),
                value: result,
                CacheExpiration
            );
            Debug.WriteLine($"[GeocodeAddressAsync:wpf]Кэшировал - {result}");

            return result;

        }

        //public async Task<RoutePoint> GeocodeFromCoordAsync(List<double> address)
        //{
        //    if (address == null || address.Count == 0)
        //        return null;

        //    if (_cache.TryGet(address.ToString().ToLowerInvariant(), out RoutePoint cachedResult))
        //    {
        //        Debug.WriteLine("Использовал кэш");
        //        return cachedResult;
        //    }

        //    var result = await GetAddressFromRoutePointAsync(address);

        //    _cache.Set(
        //        key: address.ToString().ToLowerInvariant(),
        //        value: result,
        //        CacheExpiration
        //    );
        //    Debug.WriteLine($"Кэшировал - {result}");

        //    return result;

        //}

    }
}
