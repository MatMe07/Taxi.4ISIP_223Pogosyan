using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models;

namespace Taxi.WPF.Services
{
    public class RideService
    {
        private readonly HttpClient _http;
        public RideService(HttpClient http)
        {
            _http = http;
        }


    }
}
