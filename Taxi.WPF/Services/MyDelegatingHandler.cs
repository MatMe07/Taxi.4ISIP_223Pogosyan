using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Models;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.Services
{
    public class MyDelegatingHandler : DelegatingHandler
    {
        private readonly ITokenStore _tokenStore;
        public MyDelegatingHandler(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_tokenStore.AccesToken) && request.Headers.Authorization is null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStore.AccesToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
