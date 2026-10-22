using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;

namespace Taxi.WPF.Services
{
    public class VehiclesServices
    {
        HttpClient _httpClient;
        public VehiclesServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/vehicle/getAllVehicles");

            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Instance.LoginResponse.access_token);
            var response = await _httpClient.SendAsync(request);
            //Debug.WriteLine(response.Content.ReadFromJsonAsync<VehicleDTO>());
            string rawJson = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(rawJson);
            var result = JsonSerializer.Deserialize<List<VehicleDTO>>(rawJson);
            return result ?? [];
        }

        public async Task<List<VehicleClassDTO>> GetAllVehicleClassesAsync()
        {
            var response = await _httpClient.GetAsync("api/vehicle/getAllClasses");
            var result = await response.Content.ReadFromJsonAsync<List<VehicleClassDTO>>();
            return result ?? [];
        } 
    }
}
