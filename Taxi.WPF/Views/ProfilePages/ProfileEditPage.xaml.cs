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
using Taxi.WPF.ViewModels.ProfileViewModels;
using Windows.ApplicationModel.VoiceCommands;

namespace Taxi.WPF.Views.ProfilePages
{
    /// <summary>
    /// Логика взаимодействия для ProfileEditPage.xaml
    /// </summary>
    public partial class ProfileEditPage : Page
    {
        private ProfileEditViewModel viewModel;
        public ProfileEditPage(ProfileEditViewModel viewModel)
        {
            viewModel.ClearPasswords = ClearPasswordBoxes;
            this.viewModel = viewModel;
            InitializeComponent();
            this.DataContext  = this.viewModel;
            Loaded += (_, _) => viewModel.Load();

        }

        public void ClearPasswordBoxes()
        {
            passwordCur.Clear();
            passwordNew.Clear();
            passwordNewConfirm.Clear();
        }

        private void PasswordBoxNEW_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                viewModel.PasswordNew = ((PasswordBox)sender).Password;
            }
        }

        private void PasswordBoxCurrent_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                viewModel.PasswordCurrent = ((PasswordBox)sender).Password;
            }
        }

        private void PasswordBoxNewConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                viewModel.PasswordNewConfirm = ((PasswordBox)sender).Password;
            }
        }
    }
}
