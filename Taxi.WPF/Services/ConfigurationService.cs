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
        private readonly IUserSessionService _userSession;
        private readonly ITokenStore _tokenStore;


        public ConfigurationService(IUserSessionService userSession, ITokenStore token) 
        { 
            //Settings = new AppSettings();  
            _userSession = userSession;
            _tokenStore = token;

        }

        public void Load()
        {
            AppCache.Cache.TryGet(nameof(AppSettings), out AppSettings appSettings);
            if (appSettings == null) return;
            AppSettings.Instance = appSettings;
            _tokenStore.SetToken(appSettings.userSession.access_token.AccesToken);
            _userSession.SetSession(appSettings.userSession);
            ChangeThems();
        }
        
        public void ChangeThems(string them=null)
        {
            string style = AppSettings.Instance.Theme;

            var uri = new Uri("Styles/Thems/" + style + ".xaml", UriKind.Relative);

            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            Application.Current.Resources.Clear();

            Application.Current.Resources.MergedDictionaries.Add(resourceDict);

            AppSettings.Instance.Theme = style;
            ChangeSettings();
        }

        public void ChangeSettings()
        {
            //AppSettings.Instance = appSettings ??
                //throw new ArgumentNullException(nameof(appSettings));
            AppCache.Cache.Set(nameof(AppSettings), AppSettings.Instance);

        }



    }
}
