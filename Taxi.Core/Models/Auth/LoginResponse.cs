using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Auth
{
    public class LoginResponse {
        public string access_token {  get; set; }
        public string token_type { get; set; }

        public bool IsCaptchaRequired { get; set; }

        public bool IsSuccess { get; set; }

        public Guid CaptchaId { get; set; }

        public string CaptchaImageBase64 { get; set; } = string.Empty;

        public string message {  get; set; }

    }

}
