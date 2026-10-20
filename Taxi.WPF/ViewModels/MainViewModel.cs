using System.ComponentModel;
using System.Windows.Controls;
using Taxi.WPF.Commands;
using Taxi.WPF.Models;
using Taxi.WPF.Services;
using Taxi.WPF.Views;

namespace Taxi.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string btnContent;
        public string BtnContent
        {
            get
            {
                return btnContent;
            }
            set
            {
                btnContent = value;
                OnPropertyChanged(nameof(BtnContent));
            }
        }
        public ConfigurationService _configurationSerivce;

        private int _rowFrame = 1;
        public int RowFrame
        {
            get { return _rowFrame; }
            set
            {
                _rowFrame = value;
                OnPropertyChanged(nameof(RowFrame));
            }
        }
        public Frame frameMain;

        private RelayCommand _commandOpenProfile;
        public RelayCommand CommandOpenProfile
        {
            get
            {
                return _commandOpenProfile ??
                    (_commandOpenProfile = new RelayCommand(obj =>
                    {
                        RowFrame = 1;
                        frameMain.Navigate(profilePage);
                    }
                    ));
            }
        }
        private bool _isBlurVisible = false;
        public bool IsBlurVisible
        {
            get { return _isBlurVisible; }
            set
            {
                _isBlurVisible = value;
                OnPropertyChanged(nameof(IsBlurVisible));
            }
        }

        public void ShowBlur(bool blur)
        {
            _isBlurVisible = blur;
        }

        public void LoadNextPageRoleId()
        {
            switch (AppSettings.Instance.userSession.User.RoleId)
            {
                case 1:
                    {
                        RowFrame = 0;
                        frameMain.Navigate(mapPage);
                        break;
                    }
                case 2:
                    {
                        RowFrame = 0;
                        frameMain.Navigate(mapPage);

                        break;
                    }
                case 3:
                    {
                        frameMain.Navigate(managerAutoPage);

                        break;
                    }
                case 4:
                    {
                        frameMain.Navigate(mapPage);

                        break;
                    }
            }
        }

        private MapPage mapPage;
        private ProfilePage profilePage;
        private ManagerAutoPage managerAutoPage;


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


        public MainViewModel(MapPage mapPage, ConfigurationService configuration, ProfilePage profilePage, ManagerAutoPage managerAutoPage)
        {
            this.profilePage = profilePage;
            this.mapPage = mapPage;
            _configurationSerivce = configuration;
            this.managerAutoPage = managerAutoPage;
        }

    }
}
