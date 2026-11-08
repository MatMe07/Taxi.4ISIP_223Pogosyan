using Microsoft.EntityFrameworkCore;
using Taxi.API.Server.Models;

namespace Taxi.API.Server.Services
{
    public class ProfileService
    {
        private readonly TaxiBaseContext _context;
        public ProfileService(TaxiBaseContext taxiBaseContext)
        {
            this._context = taxiBaseContext;
        }


        public async Task<string> ChangePaymentMethod(int idClientProfile, int idPaymentMethod)
        {
            var clientProfile = await _context.ClientProfiles.FirstOrDefaultAsync(p=>p.Id == idClientProfile);
            if (clientProfile == null) return "";
            var payMethods = await _context.PaymentMethods.FirstOrDefaultAsync(p=>p.Id == idPaymentMethod);

            clientProfile.DefaultPaymentMethod = payMethods;

            await _context.SaveChangesAsync();
            return payMethods.Name;
        }


    }
}
