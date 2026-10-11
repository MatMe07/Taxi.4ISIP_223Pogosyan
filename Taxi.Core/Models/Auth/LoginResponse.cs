using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Auth
{
    public record LoginResponse(string access_token, string token_type);

}
