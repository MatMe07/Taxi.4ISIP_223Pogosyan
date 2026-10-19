using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Taxi.WPF.Commands;
using Taxi.WPF.Services;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.ViewModels.ProfileViewModels
{
    public class ProfileEditViewModel : INotifyPropertyChanged
    {

        private RelayCommand _openChangePasswordCommand;
        public RelayCommand OpenChangePasswordCommand
        {
            get
            {
                return _openChangePasswordCommand ??
                    (_openChangePasswordCommand = new RelayCommand(opj =>
                    {
                        IsChangePasswordOpen = true;
                    }));

            }
        }

        public string PasswordCurrent { get; set; }
        public string PasswordNew { get; set; }
        public string PasswordNewConfirm { get; set; }




        private bool CheckFilledfields()
        {
            if (!string.IsNullOrWhiteSpace(PasswordCurrent) ||
                !string.IsNullOrWhiteSpace(PasswordNew) ||
                !string.IsNullOrWhiteSpace(PasswordNewConfirm)
                )
            {
                var res = MessageBox.Show("Вы действительно хотите выйти? Данные потеряются!", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
                return res == MessageBoxResult.No;
            }
            return false;
        }

        private bool _isChangePasswordOpen = false;
        public bool IsChangePasswordOpen
        {
            get { return _isChangePasswordOpen; }
            set
            {
                        Debug.WriteLine(AuthService);

                _isChangePasswordOpen = value;
                OnPropertyChanged(nameof(IsChangePasswordOpen));
            }
        }

        public Action ClearPasswords {  get; set; }

        private RelayCommand _closeChangePasswordCommand;
        public RelayCommand CloseChangePasswordCommand
        {
            get
            {
                return _closeChangePasswordCommand ??
                    (_closeChangePasswordCommand = new RelayCommand(obj =>
                    {
                        var checkFiled= CheckFilledfields();
                        if (!checkFiled) ClearPasswords();
                        IsChangePasswordOpen = checkFiled;
                    }));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private MainViewModel MainViewModel;
        private AuthService AuthService;
        public ProfileEditViewModel(IAuthService authService)
        {
            AuthService = authService as AuthService;
            //MainViewModel = mainView;
        }
    }
}
