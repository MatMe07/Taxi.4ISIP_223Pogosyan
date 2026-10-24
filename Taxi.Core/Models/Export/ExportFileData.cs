using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Export
{
    public class ExportFileData
    {
        public string fileName { get; set; }

        public List<string> ColumnNames { get; set; }

        public List<DataEarnings> DataEarnings { get; set; }
    }
}
