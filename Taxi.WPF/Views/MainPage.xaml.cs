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
using Taxi.WPF.Services;
using Taxi.WPF.ViewModels;

namespace Taxi.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(MainPageViewModel viewModel,ConfigurationService configuration)
        {
            viewModel._configurationSerivce = configuration;
            this.DataContext = viewModel;
            InitializeComponent();

        }
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
