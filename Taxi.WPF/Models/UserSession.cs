using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.WPF.Services;

namespace Taxi.WPF.Models
{
    public class UserSession
    {
        public UserDTO User { get; set; }
        public TokenStore access_token { get; set; } = new TokenStore();

    }
}
