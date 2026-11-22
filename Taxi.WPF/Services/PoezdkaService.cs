using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.Core.Models;
using Taxi.Core.Models.DB;
using static System.Net.WebRequestMethods;

namespace Taxi.WPF.Services
{
    public class PoezdkaService
    {
        HttpClient _httpClient;

        public PoezdkaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<PoezdkaDTO>> GetAllPoezdkasAsync()
        {
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/poezdka/getAllPoezdka");

            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Instance.LoginResponse.access_token);
            //var response = await _httpClient.SendAsync(request);
            var response = await _httpClient.GetFromJsonAsync<List<PoezdkaDTO>>("api/poezdka/getAllPoezdka");
            //Debug.WriteLine(response.Content.ReadFromJsonAsync<VehicleDTO>());
            return response ?? [];
        }

        public async Task<RideResponse> TryToConnectAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<RideResponse>("api/poezdka/tryToConnect");
            return response ?? new RideResponse { message = "Error", isAllowed = false };
        }
    }
}
