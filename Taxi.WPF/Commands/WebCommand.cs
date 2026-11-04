using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.WPF.Commands
{
    public class WebCommand
    {
        public string Type { get; set; } 
        public object Payload { get; set; }
    }
}
