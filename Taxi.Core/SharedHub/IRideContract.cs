using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;

namespace Taxi.Core.SharedHub
{
    public interface IRideContract
    {
        Task ReceiveMessage(string user, string message);
        Task DriverAssigned(int driverId);
        //Task RideCompleted(RideDto summary);
    }
}
