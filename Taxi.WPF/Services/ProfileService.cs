using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Models;

namespace Taxi.WPF.Services
{
    public class ProfileService
    {
        private readonly HttpClient _http;
        public ProfileService(HttpClient http)
        {
            _http = http;
        }

        public async Task ChangePaymentMethod(int idClientProfile, int idPaymentMethod)
        {
            var content = new { idClientProfile };
            var response = await _http.PatchAsJsonAsync($"api/profile/changePaymentMethod?idClientProfile={idClientProfile}&idPaymentMethod={idPaymentMethod}", content);
            var result = await response.Content.ReadAsStringAsync();
            AppSettings.Instance.userSession.User.ClientProfile.DefaultPaymentMethodName = result;
            AppSettings.Instance.userSession.User.ClientProfile.DefaultPaymentMethodId = idPaymentMethod;
            ConfigurationService.ChangeSettings();
        }
    }
}
