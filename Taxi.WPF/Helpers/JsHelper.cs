using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Taxi.src.Taxi.Map.Helpers
{
    public static class JsHelper
    {
        public static string JsStr(string s) => JsonSerializer.Serialize(s ?? string.Empty);

        public static string JsNum(double d) => d.ToString(CultureInfo.InvariantCulture);

        public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
