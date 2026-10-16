using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Taxi.src.Taxi.Map;
using Taxi.WPF.Models;
using Taxi.WPF.Services;
using Taxi.WPF.ViewModels;

namespace Taxi.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private MapPage mapPage;
        public MainPage(MainPageViewModel viewModel, MapPage mapPage, ConfigurationService configuration)
        {
            viewModel._configurationSerivce = configuration;
            this.DataContext = viewModel;
            InitializeComponent();
            this.mapPage = mapPage;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppSettings.Instance.IsSign) return;
            switch (AppSettings.Instance.LoginResponse.RoleId)
            {
                case 1:
                    {
                        frameMain.Navigate(mapPage);
                        break;
                    }
                case 2:
                    {
                        frameMain.Navigate(mapPage);

                        break;
                    }
                case 3:
                    {
                        frameMain.Navigate(mapPage);

                        break;
                    }
                case 4:
                    {
                        frameMain.Navigate(mapPage);

                        break;
                    }
            }
        }
    }
}
