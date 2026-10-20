using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        public bool IsAuth => userSession?.access_token != null;

        public string Theme { get; set; } = "light";
        public UserSession userSession { get; set; }

        public DateTimeOffset LastSessionDate { get; set; }
    }
}
