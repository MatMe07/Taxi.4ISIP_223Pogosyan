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
using Taxi.WPF.Views.ManagerPages;

namespace Taxi.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для ManagerAutoPage.xaml
    /// </summary>
    public partial class ManagerAutoPage : Page
    {
        public ManagerAutoPage(DataCarsPage dataCars)
        {
            InitializeComponent();
            frameVehicles.Navigate(dataCars);
        }
    }
}
