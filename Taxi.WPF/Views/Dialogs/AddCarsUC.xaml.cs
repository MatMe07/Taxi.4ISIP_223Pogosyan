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
using Taxi.Core.Models.DB;
using Taxi.WPF.ViewModels.DialogsVM;

namespace Taxi.WPF.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для AddCarsUC.xaml
    /// </summary>
    public partial class AddCarsUC : UserControl
    {
        public AddCarsUC(AddCarViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += (_, _) => viewModel.Load();
        }
    }
}
