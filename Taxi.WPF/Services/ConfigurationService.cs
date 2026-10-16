using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Taxi.WPF.Models;
using Taxi.WPF.Services.Interfaces;
using Windows.ApplicationModel.VoiceCommands;

namespace Taxi.WPF.Services
{
    public class ConfigurationService : IConfigurationService
    {

        //public AppSettings Settings { get; private set; }

        public ConfigurationService()
        { 
            //Settings = new AppSettings();  
        }

        public void Load()
        {
            AppCache.Cache.TryGet(nameof(AppSettings), out AppSettings appSettings);
            AppSettings.Instance = appSettings;
            ChangeThems();
        }
        
        public void ChangeThems(string them=null)
        {
            string style = AppSettings.Instance.Theme;

            var uri = new Uri("Styles/Thems/" + style + ".xaml", UriKind.Relative);

            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            Application.Current.Resources.Clear();

            Application.Current.Resources.MergedDictionaries.Add(resourceDict);

            AppSettings.Instance.Theme = them;
            ChangeSettings();
        }

        public async void ChangeSettings()
        {
            //AppSettings.Instance = appSettings ??
            //    throw new ArgumentNullException(nameof(appSettings));
            await AppCache.Cache.UpdateAsync(nameof(AppSettings), AppSettings.Instance);

        }



    }
}
