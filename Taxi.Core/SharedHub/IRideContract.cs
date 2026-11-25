using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;

namespace Taxi.Core.SharedHub
{
    public interface IRideContract : IDriverContract, IClientContract
    {
        //Task DriverAssigned(int driverId);

        //Task New

        //Task NewOrder(int orderID);
        //Task RideCompleted(RideDto summary);
    }
}
