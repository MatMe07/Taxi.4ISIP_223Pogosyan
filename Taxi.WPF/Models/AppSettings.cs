using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Auth;

namespace Taxi.WPF.Models
{
    class AppSettings
    {
        private static AppSettings _instance;
        public static AppSettings Instance
        {
            get
            {
                _instance ??= new AppSettings();
                return _instance;
            }
        }


        public string Theme { get; set; } = "Light";
        public string LoginUser { get; set; } = string.Empty;
        public RegisterResponse? RegisterResponse { get; set; }
        public LoginResponse? LoginResponse { get; set; }

        public DateTimeOffset LastSessionDate { get; set; }
    }
}
