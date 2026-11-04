using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.src.Taxi.Map.Helpers;
using Taxi.WPF.Commands;

namespace Taxi.WPF.Services
{
    public class WebMessages
    {

        public static string SendToWeb(string commandType, object data)
        {
            var command = new WebCommand { Type = commandType, Payload = data };
            string json = JsonSerializer.Serialize(command, JsHelper.JsonOptions);
            return json;
        }
    }
}
