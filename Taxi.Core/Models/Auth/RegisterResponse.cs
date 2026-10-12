using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Auth
{
    public record RegisterResponse(string login, string fio);
}
