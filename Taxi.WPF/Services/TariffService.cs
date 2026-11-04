using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;

namespace Taxi.WPF.Services
{
    public class TariffService
    {
        private readonly HttpClient _http;
        public TariffService(HttpClient http)
        {
            _http = http;
        }


        public async Task<List<TariffDTO>> GetAllTariffsAsync()
        {
            var response = await _http.GetFromJsonAsync<List<TariffDTO>>("api/tariff/GetAllCurrentTariffs");
            return response ?? [];
        }
    }
}
