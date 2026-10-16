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
using Taxi.WPF.ViewModels;
using Taxi.WPF.Converters;
using Taxi.WPF.Services;

namespace Taxi.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage(AuthorizationViewModel viewModel, ConfigurationService configuration)
        {
            InitializeComponent();
            viewModel.ClearPasswords = ClearPasswords;
            this.DataContext = viewModel;


        }

        public void ClearPasswords()
        {
            PasswordBoxLogin.Clear();
            PasswordBoxRegister.Clear();
            PasswordBoxRegisterConfirm.Clear();
        }

        private void PasswordBoxConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((AuthorizationViewModel)this.DataContext).txtConfirmPasswordRegister = ((PasswordBox)sender).Password;
            }
        }

        private void PasswordBoxRegister_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((AuthorizationViewModel)this.DataContext).txtPasswordRegister = ((PasswordBox)sender).Password;
            }
        }

        private void PasswordBoxLogin_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((AuthorizationViewModel)this.DataContext).txtPasswordLogin = ((PasswordBox)sender).Password;
            }
        }
    }
}
