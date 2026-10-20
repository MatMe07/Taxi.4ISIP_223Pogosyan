using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Taxi.Core.Models.DB;
using Taxi.WPF.Models;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.Services
{
    public class ManagerService : IUserBaseService
    {
        HttpClient _httpClient;
        public ManagerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/vehicle/getAllVehicles");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Instance.LoginResponse.access_token);
            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<List<VehicleDTO>>();
            return result;
             
        }

    }
}
