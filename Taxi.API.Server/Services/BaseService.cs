using Taxi.API.Server.Models;

namespace Taxi.API.Server.Services
{
    public class BaseService
    {
        protected readonly TaxiBaseContext _context;

        public BaseService(TaxiBaseContext context)
        {
            _context = context;
        }
    }
}
