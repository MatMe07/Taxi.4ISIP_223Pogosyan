using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Auth;

namespace Taxi.WPF.Models
{
    public class AppSettings
    {
        private static AppSettings _instance;
        public static AppSettings Instance
        {
            get
            {
                _instance ??= new AppSettings();
                return _instance;
            }
            set
            {
                if (_instance != null)
                {
                    throw new InvalidOperationException("AppSettings уже есть");
                }
                _instance = value;
            }
        }


        public string Theme { get; set; } = "light";
        public string LoginUser { get; set; } = string.Empty;

        public LoginResponse? LoginResponse { get; set; }

        public bool IsSign {  get; set; } = false;

        public DateTimeOffset LastSessionDate { get; set; }
    }
}
