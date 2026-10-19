using Accessibility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Auth;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest loginRequest)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", loginRequest);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            //if (result == null)
            //    throw new InvalidCastException("LoginResponse error");
            return result;
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(LoginResponse loginRequest,string curPassword,string newPassword)
        {
            var changeRequest = new ChangePasswordRequst
            {
                loginResponse = loginRequest,
                PasswordCurrent = curPassword,
                PasswordNew = newPassword
            };
            var response = await _http.PostAsJsonAsync("api/auth/changePassword", changeRequest);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ChangePasswordResponse>();
            return result;
        }

        public async Task<RegisterResponse> RegisterNewUserAsync(RegisterRequest registerRequest)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", registerRequest);
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
            //if (result == null)
            //    throw new InvalidCastException("RegisterNewUser error");
            return result;
        }

    }
}
