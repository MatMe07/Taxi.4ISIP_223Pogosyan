using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Models;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.Services
{
    class ConfigurationService : IConfigurationService
    {

        public AppSettings Settings { get; private set; }

        public ConfigurationService()
        { 
            Settings = new AppSettings();  
        }

        public void Load()
        {
            AppCache.Cache.TryGet(nameof(AppSettings), out AppSettings appSettings);
            AppSettings.Instance = appSettings;


        }

        public void ChangeSettings(AppSettings appSettings)
        {
            Settings = appSettings ??
                throw new ArgumentNullException(nameof(appSettings));
            AppCache.Cache.
        }



    }
}
