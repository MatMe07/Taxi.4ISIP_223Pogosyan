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
using Taxi.WPF.ViewModels.ProfileViewModels;

namespace Taxi.WPF.Views.ProfilePages
{
    /// <summary>
    /// Логика взаимодействия для ProfileEditPage.xaml
    /// </summary>
    public partial class ProfileEditPage : Page
    {
        public ProfileEditPage(ProfileEditViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext  = viewModel;
        }
    }
}
