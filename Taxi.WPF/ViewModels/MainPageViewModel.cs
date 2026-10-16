using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Commands;
using Taxi.WPF.Models;
using Taxi.WPF.Services;

namespace Taxi.WPF.ViewModels
{
    public class MainPageViewModel
    {
        public ConfigurationService _configurationSerivce;
        private RelayCommand _btnThemsCommand;

        public RelayCommand BtnThemsCommand
        {
            get
            {
                return _btnThemsCommand ??
                    (_btnThemsCommand = new RelayCommand(obj =>
                    {
                        string style = AppSettings.Instance.Theme == "light" ? "dark" : "light";
                        AppSettings.Instance.Theme = style;
                        _configurationSerivce.ChangeThems();

                    }));
            }
        }
        public MainPageViewModel()
        {
            
        }
    }
}
