using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.Core.Models.Order;

namespace Taxi.Core.SharedHub
{
    public interface IDriverContract : IGeneralContract
    {
        Task NewOrder(GetDriverRequest driverRequest);
        
    }
}
