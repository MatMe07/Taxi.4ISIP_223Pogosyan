using Taxi.API.Server.Models;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Services
{
    public class PaymentsService
    {
        private readonly TaxiBaseContext _context;
        public PaymentsService(TaxiBaseContext context)
        {
            _context = context;
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
