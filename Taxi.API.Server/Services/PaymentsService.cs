using Taxi.API.Server.Models;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Services
{
    public class PaymentsService : BaseService
    {
        public PaymentsService(TaxiBaseContext context) : base(context)
        {

        }

        public IEnumerable<PaymentMethodDTO> GetPaymentMethods()
        {
            return _context.PaymentMethods.Select(p => new PaymentMethodDTO { 
                Id=p.Id,
                Name=p.Name,
                PaymentIconPath = p.PaymentIconPath,
            });
        }
    }
}
