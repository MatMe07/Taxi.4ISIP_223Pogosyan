using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.Core.Models.Map;

namespace Taxi.WPF.Services
{
    public class TariffService
    {
        private readonly HttpClient _http;
        public TariffService(HttpClient http)
        {
            _http = http;
        }


        public async Task<IEnumerable<TariffDTO>> GetAllTariffsAsync(RouteInfo routeInfo, bool isRoute)
        {
            TariffRequest request = new() {
                routeInfo = routeInfo,
                IsRoute = isRoute
            };
            var response = await _http.PostAsJsonAsync("api/tariff/GetAllCurrentTariffs", request);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<TariffDTO>>();
            return result ?? [];
        }
    }
}
