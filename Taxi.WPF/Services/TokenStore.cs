using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.Services
{
    public class TokenStore : ITokenStore
    {
        public string? AccesToken { get; private set; }

        public void Clear()
        {
            AccesToken = null;
        }

        public void SetToken(string? token)
        {
            AccesToken = token;
        }
    }
}
