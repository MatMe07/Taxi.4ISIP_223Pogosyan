using Accessibility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.Core.Models.Auth;
using Taxi.WPF.Models;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        //private readonly IUserSessionService _userSession;
        private readonly ITokenStore _tokenStore;
        public AuthService(HttpClient http, ITokenStore tokenStore)
        {
            _http = http;
            //_userSession = userSession;
            _tokenStore = tokenStore;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest loginRequest)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", loginRequest);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result?.User == null)
                return result;
            var newUsSes = new UserSession
            {
                User = result.User
            };
            newUsSes.access_token.SetToken(result.access_token);
            _tokenStore.SetToken(result.access_token);
            AppSettings.Instance.userSession = newUsSes;

            
            return result;
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(string curPassword,string newPassword, string newConfirmPassword)
        {
            //_http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AppSettings.Instance.LoginResponse.access_token);

            var changeRequest = new ChangePasswordRequst
            {
                PasswordCurrent = curPassword,
                PasswordNew = newPassword,
                PasswordNewConfirm=newConfirmPassword
            };
            string js = JsonSerializer.Serialize(changeRequest);
            Debug.WriteLine(js);
            var response = await _http.PostAsJsonAsync("api/auth/changePassword", changeRequest);
            //response.EnsureSuccessStatusCode();
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
