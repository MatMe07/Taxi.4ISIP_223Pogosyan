using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.SharedHub
{
    public interface IClientContract : IGeneralContract
    {
        //void ResponseNewOrder(bool isCreated, int orderId);
        Task AcceptOrder();
    }
}
