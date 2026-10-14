using System.Text.Json.Serialization;

namespace Taxi.Core.Models.Auth
{
    public class LoginRequest
    {
        public string login { get; set; }
        public string password { get; set; }

        public Guid captchaId { get; set; }
        public string captchaText { get; set; } = string.Empty;
    }
}
