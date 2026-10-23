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
    public class UsersServices
    {
        private readonly HttpClient _httpClient;

        public UsersServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync("api/users/getAll");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
            return result ?? new List<UserDTO>();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/users/delete/{id}");
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateUserAsync(UserDTO user)
        {
            var response = await _httpClient.PatchAsJsonAsync("api/users/update", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            var response = await _httpClient.GetAsync("api/users/getAllRoles");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<RoleDTO>>();
            return result ?? new List<RoleDTO>();
        }
    }
}
