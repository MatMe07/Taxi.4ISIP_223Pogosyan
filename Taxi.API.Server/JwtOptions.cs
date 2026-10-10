using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Taxi.API.Server
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        public string Issuer { get; set; } = "TaxiJWT";
        public string Audience { get; set; } = "TaxiApiJWT";
        public string Key { get; set; } = default;
        public int LifetimeMinutes { get; set; } = 60;

        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));


    }
}
