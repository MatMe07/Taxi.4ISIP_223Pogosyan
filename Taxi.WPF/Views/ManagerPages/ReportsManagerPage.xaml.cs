using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для ReportsManagerPage.xaml
    /// </summary>
    public partial class ReportsManagerPage : Page
    {
        public ReportsManagerPage(ReportsManagerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += async (_, _) => await viewModel.Load();
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDatePicker.DisplayDateStart = StartDatePicker.SelectedDate;
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDatePicker.DisplayDateEnd = EndDatePicker.SelectedDate;
        }
    }
}
