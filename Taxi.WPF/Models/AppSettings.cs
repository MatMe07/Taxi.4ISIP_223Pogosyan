using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Auth;

namespace Taxi.WPF.Models
{
    public class AppSettings : INotifyPropertyChanged
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
        public bool IsAuth => userSession?.access_token?.AccesToken != null;

        private string _theme= "light";
        public string Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                if (value != _theme)
                {
                    _theme = value;
                    OnPropertyChanged(nameof(Theme));
                }
            }
        }

        public UserSession userSession { get; set; }

        public DateTimeOffset LastSessionDate { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
