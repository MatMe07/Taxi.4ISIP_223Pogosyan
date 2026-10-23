using System.ComponentModel;
using System.Windows;
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
                        if (!CheckUserAuth()) return;
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

        private Page _mainPage;


        private bool CheckUserAuth()
        {
            if (!AppSettings.Instance.IsAuth)
            {
                MessageBox.Show("Сначала войдите в систему!!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void LoadNextPageRoleId()
        {
            if (!CheckUserAuth()) return;
            switch (AppSettings.Instance.userSession.User.RoleId)
            {
                case 1:
                    {
                        RowFrame = 0;
                        _mainPage = mapPage;
                        break;
                    }
                case 2:
                    {
                        RowFrame = 0;
                        //frameMain.Navigate(mapPage);

                        break;
                    }
                case 3:
                    {
                        //frameMain.Navigate(managerAutoPage);
                        _mainPage = managerAutoPage;
                        break;
                    }
                case 4:
                    {
                        //frameMain.Navigate(mapPage);
                        _mainPage = administratorPage;
                        break;
                    }
            }
            frameMain.Navigate(_mainPage);

        }

        private MapPage mapPage;
        private ProfilePage profilePage;
        private ManagerAutoPage managerAutoPage;
        private AdministratorPage administratorPage;


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
        private RelayCommand _btnIconClickCommand;

        public RelayCommand BtnIconClickCommand
        {
            get
            {
                return _btnIconClickCommand ??
                    (_btnIconClickCommand = new RelayCommand(obj =>
                    {
                        LoadNextPageRoleId();
                    }));
            }
        }


        public MainViewModel(MapPage mapPage, ConfigurationService configuration,
            ProfilePage profilePage, ManagerAutoPage managerAutoPage,
            AdministratorPage administratorPage)
        {
            this.profilePage = profilePage;
            this.mapPage = mapPage;
            _configurationSerivce = configuration;
            this.managerAutoPage = managerAutoPage;
            this.administratorPage = administratorPage;
        }

    }
}
