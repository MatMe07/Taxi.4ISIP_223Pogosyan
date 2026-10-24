using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Export
{
    public class ExportPDFRequest : ExportFileData
    {
        public string TtleFile { get; set; }
        public string Period { get; set; }
        public string ClassVehicle { get; set; }

        public string WhoSform { get; set; }

        public string isCreated { get; set; }
    }
}
