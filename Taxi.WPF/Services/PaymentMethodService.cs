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
    public class PaymentMethodService
    {
        private readonly HttpClient _http;
        public PaymentMethodService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<PaymentMethodDTO>> GetPaymentMethods()
        {
            var response = await _http.GetFromJsonAsync<IEnumerable< PaymentMethodDTO>>("api/payments/allPaymentsMehtods");
            return response;
        }
    }
}
