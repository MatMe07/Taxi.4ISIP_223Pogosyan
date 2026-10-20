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
using Taxi.WPF.ViewModels.ManagerViewModels;

namespace Taxi.WPF.Views.ManagerPages
{
    /// <summary>
    /// Логика взаимодействия для DataCarsPage.xaml
    /// </summary>
    public partial class DataCarsPage : Page
    {
        public DataCarsPage(DataCarsViewModel dataCarsView)
        {
            InitializeComponent();
            DataContext = dataCarsView;
            Loaded += (_, _) => dataCarsView.Load();
        }
    }
}
