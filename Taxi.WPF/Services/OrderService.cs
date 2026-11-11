using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Order;
using Taxi.WPF.Models;

namespace Taxi.WPF.Services
{
    public class OrderService
    {
        private readonly HttpClient _http;
        public OrderService(HttpClient http)
        {
            _http = http;
        }


        public async Task<(bool, int?)> CreateNewOrderAsync(int clientId,int tariffId, string AddressOtkuda, string AddressKuda, int PaymentMethodId, decimal PaymentAmount)
        {
            var orderReq = new OrderRequest
            {
                ClientId=clientId,
                TariffId= tariffId,
                AddressOtkuda=AddressOtkuda,
                AddressKuda=AddressKuda,
                PaymentMethodId=PaymentMethodId,
                PaymentAmount=PaymentAmount
            };
            var response = await _http.PostAsJsonAsync("api/order/createNewOrder", orderReq);
            var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
            return (result.isCreated, result.orderId);
        }

        public async Task CancelOrderAsync(int orderId)
        {
            _ = await _http.PatchAsync($"api/order/closeOrder?orderId={orderId}", null);

        }
    }
}
