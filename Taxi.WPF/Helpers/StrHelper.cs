using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.WPF.Helpers
{
    public static class StrHelper
    {
        public static bool CheckTxt(string text)
        {
            return !string.IsNullOrEmpty(text) || !string.IsNullOrWhiteSpace(text);
        }
    }
}
